#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Heal : Behavior
    {
        private readonly double range;
        private readonly int amount;
        private readonly string group;
        private Cooldown coolDown;

        protected bool MaxHpSet { get; private set; }
        protected int MaxHP { get; private set; }

        public Heal(
            double range,
            int amount,
            string group = null,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.range = (float)range;
            this.amount = amount;
            this.group = group;
            this.coolDown = coolDown.Normalize();
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool = (int)state;

            if (cool <= 0)
            {
                if (group == null)
                {
                    Enemy entity = host as Enemy;

                    if (!MaxHpSet)
                    {
                        MaxHP = entity.HP;
                        MaxHpSet = true;
                    }

                    int newHp = amount;

                    if (entity.HP + amount > MaxHP)
                        newHp = MaxHP - entity.HP;

                    if (newHp != entity.HP && newHp > 0)
                    {
                        int n = newHp;
                        entity.HP += newHp;
                        entity.UpdateCount++;
                        entity.Owner.BroadcastMessage(new SHOWEFFECT
                        {
                            EffectType = EffectType.Heal,
                            TargetId = entity.Id,
                            Color = new ARGB(0xffffffff)
                        }, null);
                        entity.Owner.BroadcastMessage(new NOTIFICATION
                        {
                            ObjectId = entity.Id,
                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + n + "\"}}",
                            Color = new ARGB(0xff00ff00)
                        }, null);
                    }
                }
                else
                {
                    foreach (Enemy entity in host.GetNearestEntitiesByGroup(range, group).OfType<Enemy>())
                    {
                        if (!MaxHpSet)
                        {
                            MaxHP = entity.HP;
                            MaxHpSet = true;
                        }

                        int newHp = amount;

                        if (entity.HP + amount > MaxHP)
                            newHp = MaxHP - entity.HP;

                        if (newHp != entity.HP && newHp > 0)
                        {
                            int n = newHp;
                            entity.HP += newHp;
                            entity.UpdateCount++;
                            entity.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Heal,
                                TargetId = entity.Id,
                                Color = new ARGB(0xffffffff)
                            }, null);
                            entity.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Line,
                                TargetId = host.Id,
                                PosA = new Position { X = entity.X, Y = entity.Y },
                                Color = new ARGB(0xffffffff)
                            }, null);
                            entity.Owner.BroadcastMessage(new NOTIFICATION
                            {
                                ObjectId = entity.Id,
                                Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + n + "\"}}",
                                Color = new ARGB(0xff00ff00)
                            }, null);
                        }
                    }
                }
                cool = coolDown.Next(Random);
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}