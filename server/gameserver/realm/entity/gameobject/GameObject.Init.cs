#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    public partial class GameObject : Entity
    {
        //Stats
        public GameObject(ushort objType, int? lifeTime, bool stat, bool dying, bool hittestable)
            : base(objType, IsInteractive(objType))
        {
            if (Vulnerable = lifeTime.HasValue)
                HP = lifeTime.Value;

            Dying = dying;

            if (objType == 0x01c7)
                Static = true;
            else
                Static = stat;

            Hittestable = hittestable;
        }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if ((Static && !ObjectDesc.Enemy) || !Vulnerable)
                return false;
            if (projectile.ProjectileOwner is Player &&
                !HasConditionEffect(ConditionEffectIndex.Paused) &&
                !HasConditionEffect(ConditionEffectIndex.Stasis) &&
                !HasConditionEffect(ConditionEffectIndex.Invulnerable) &&
                !HasConditionEffect(ConditionEffectIndex.Invincible))
            {
                var def = ObjectDesc.Defense;

                if (projectile.ProjDesc.ArmorPiercing)
                    def = 0;

                var dmg = (int)StatsManager.GetDefenseDamage(this, projectile.Damage, def);

                HP -= dmg;

                foreach (var effect in projectile.ProjDesc.Effects)
                {
                    if (effect.Effect == ConditionEffectIndex.Stunned && ObjectDesc.StunImmune ||
                        effect.Effect == ConditionEffectIndex.Paralyzed && ObjectDesc.ParalyzedImmune ||
                        effect.Effect == ConditionEffectIndex.Dazed && ObjectDesc.DazedImmune)
                        continue;

                    ApplyConditionEffect(effect);
                }

                Owner.BroadcastMessage(new DAMAGE
                {
                    TargetId = Id,
                    Effects = projectile.ConditionEffects,
                    Damage = (ushort)dmg,
                    Killed = HP <= 0,
                    BulletId = projectile.ProjectileId,
                    ObjectId = projectile.ProjectileOwner.Id
                }, projectile.ProjectileOwner as Player);

                if (HP <= 0)
                    Owner.LeaveWorld(this);

                UpdateCount++;

                return true;
            }

            return false;
        }

        public override void Tick(RealmTime time)
        {
            if (Vulnerable && ObjectDesc != null)
            {
                if (Dying)
                {
                    HP -= time.ElapsedMsDelta;
                    UpdateCount++;
                }

                CheckHP();
            }

            base.Tick(time);
        }
    }
}