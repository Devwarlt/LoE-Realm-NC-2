#region

using LoESoft.Core.models;
using LoESoft.GameServer.logic;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.terrain;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    public class Enemy : Character
    {
        public static ConcurrentDictionary<int, List<KeyValuePair<bool, INonSkippableState>>> StoredTransitions =
            new ConcurrentDictionary<int, List<KeyValuePair<bool, INonSkippableState>>>();

        private readonly bool stat;
        private DamageCounter counter;
        private float bleeding;
        private Position? pos;
        private bool Npc { get; set; }
        //public bool CheckDeath { get; set; }
        private bool Done { get; set; }

        public Enemy(ushort objType, bool npc)
            : base(objType, new wRandom())
        {
            npc = Npc;
            stat = ObjectDesc.MaxHP == 0;
            counter = new DamageCounter(this);
            LootState = "";
            Name = ObjectDesc.ObjectId;
            Done = false;
        }

        public DamageCounter DamageCounter
        { get { return counter; } }

        public WmapTerrain Terrain
        { get; set; }

        public int AltTextureIndex
        { get; set; }

        public string LootState
        { get; set; }

        public Position SpawnPoint
        { get { return pos ?? new Position { X = X, Y = Y }; } }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.ALT_TEXTURE_STAT] = (AltTextureIndex != -1) ? AltTextureIndex : 0; //maybe?
            stats[StatsType.HP_STAT] = HP;
            base.ExportStats(stats);
        }

        public override void Init(World owner)
        {
            base.Init(owner);
            if (ObjectDesc.StasisImmune)
                ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.StasisImmune,
                    DurationMS = -1
                });
        }

        public void Death(RealmTime time)
        {
            State state = CurrentState;

            if (state == null)
            {
                Log.Warn($"Enemy '{Name}' doesn't have any behavior declaration.");
                counter.Dispose();
            }
            else
            {
                counter.Death(time);
                state.OnDeath(new BehaviorEventArgs(this, time));
            }

            Owner.LeaveWorld(this);
        }

        public void SetDamageCounter(DamageCounter counter, Enemy enemy)
        {
            this.counter = counter;
            this.counter.UpdateEnemy(enemy);
        }

        public int Damage(Player from, RealmTime time, int dmg, bool noDef, params ConditionEffect[] effs)
        {
            if (stat)
                return 0;
            if (HasConditionEffect(ConditionEffectIndex.Invincible))
                return 0;
            if (!HasConditionEffect(ConditionEffectIndex.Paused) &&
                !HasConditionEffect(ConditionEffectIndex.Stasis))
            {
                int def = ObjectDesc.Defense;
                if (noDef)
                    def = 0;
                dmg = (int) StatsManager.GetDefenseDamage(this, dmg, def);
                int effDmg = dmg;
                if (effDmg > HP)
                    effDmg = HP;
                if (!HasConditionEffect(ConditionEffectIndex.Invulnerable))
                    HP -= dmg;
                ApplyConditionEffect(effs);
                if (from != null)
                {
                    Owner?.BroadcastMessage(new DAMAGE
                    {
                        TargetId = Id,
                        Effects = 0,
                        Damage = (ushort) dmg,
                        Killed = HP <= 0,
                        BulletId = 0,
                        ObjectId = from.Id
                    }, null);

                    counter?.HitBy(from, time, null, dmg);
                }
                else
                {
                    Owner?.BroadcastMessage(new DAMAGE
                    {
                        TargetId = Id,
                        Effects = 0,
                        Damage = (ushort) dmg,
                        Killed = HP <= 0,
                        BulletId = 0,
                        ObjectId = -1
                    }, null);
                }

                if (HP <= 0)
                    //CheckDeath = true;
                    Death(time);

                UpdateCount++;
                return effDmg;
            }
            return 0;
        }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if (stat)
                return false;
            if (HasConditionEffect(ConditionEffectIndex.Invincible))
                return false;
            if (projectile.ProjectileOwner is Player &&
                !HasConditionEffect(ConditionEffectIndex.Paused) &&
                !HasConditionEffect(ConditionEffectIndex.Stasis))
            {
                var prevHp = HP;
                var dmg = (int) StatsManager.GetDefenseDamage(this, projectile.Damage, projectile.ProjDesc.ArmorPiercing ? 0 : ObjectDesc.Defense);
                if (!HasConditionEffect(ConditionEffectIndex.Invulnerable))
                    HP -= dmg;
                foreach (ConditionEffect effect in projectile.ProjDesc.Effects.Where(effect => (effect.Effect != ConditionEffectIndex.Stunned || !ObjectDesc.StunImmune) && (effect.Effect != ConditionEffectIndex.Paralyzed || !ObjectDesc.ParalyzedImmune) && (effect.Effect != ConditionEffectIndex.Dazed || !ObjectDesc.DazedImmune)))
                    ApplyConditionEffect(effect);

                Owner.BroadcastMessage(new DAMAGE
                {
                    TargetId = Id,
                    Effects = projectile.ConditionEffects,
                    Damage = (ushort) dmg,
                    Killed = HP <= 0,
                    BulletId = projectile.ProjectileId,
                    ObjectId = projectile.ProjectileOwner.Id
                }, HP <= 0 && !IsOneHit(dmg, prevHp) ? null : projectile.ProjectileOwner as Player);

                counter.HitBy(projectile.ProjectileOwner as Player, time, projectile, dmg);

                if (HP <= 0)
                    //CheckDeath = true;
                    Death(time);

                UpdateCount++;
                return true;
            }
            return false;
        }

        public override void Tick(RealmTime time)
        {
            if (pos == null)
                pos = new Position { X = X, Y = Y };

            /*if (CheckDeath)
            {
                try
                {
                    if (StoredTransitions.ContainsKey(Id))
                    {
                        if (!Done)
                        {
                            List<KeyValuePair<bool, INonSkippableState>> transitions = StoredTransitions[Id];

                            for (int i = 0; i < transitions.Count; i++)
                            {
                                if (transitions[i].Key)
                                    transitions.RemoveAt(i);
                                else
                                {
                                    if (transitions[i].Value.DoneStorage)
                                    {
                                        HP = transitions[i].Value.StoreHP;
                                        transitions[i] = new KeyValuePair<bool, INonSkippableState>(true, transitions[i].Value);
                                        UpdateCount++;
                                        Done = false;
                                        CheckDeath = false;
                                        break;
                                    }
                                }
                            }

                            if (transitions.Count == 0
                                || transitions == null
                                || StoredTransitions[Id] == null
                                || !StoredTransitions.ContainsKey(Id))
                            {
                                Death(time);
                                Done = true;
                                return;
                            }
                            else
                                StoredTransitions[Id] = transitions;
                        }
                    }
                    else
                    {
                        List<Transition> currentTransitions = CurrentState.Transitions.ToList();
                        List<KeyValuePair<bool, INonSkippableState>> transitions = new List<KeyValuePair<bool, INonSkippableState>>();

                        int i = 0;
                        foreach (Transition transition in CurrentState.Transitions)
                        {
                            if (transition is INonSkippableState)
                            {
                                INonSkippableState nonSkippableState = transition as INonSkippableState;
                                nonSkippableState.Skip = true;

                                transitions.Add(new KeyValuePair<bool, INonSkippableState>(false, nonSkippableState));

                                currentTransitions[i] = nonSkippableState as Transition; // safe update
                            }
                            i++;
                        }

                        if (transitions.Count == 0)
                        {
                            Death(time);
                            Done = true;
                            return;
                        }
                        else
                        {
                            StoredTransitions[Id] = transitions;
                            CurrentState.Transitions = currentTransitions;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    Death(time);
                    return;
                }
            }*/

            if (!stat && HasConditionEffect(ConditionEffectIndex.Bleeding))
            {
                if (bleeding > 1)
                {
                    HP -= (int) bleeding;
                    bleeding -= (int) bleeding;
                    UpdateCount++;
                }
                bleeding += 28 * (time.ElapsedMsDelta / 1000f);
            }

            base.Tick(time);
        }
    }
}