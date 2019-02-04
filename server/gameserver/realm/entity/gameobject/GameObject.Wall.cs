#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using System.Xml.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    partial class Wall : GameObject
    {
        public Wall(ushort objType, XElement node)
            : base(objType, GetHP(node), true, false, true) { }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if (!Vulnerable || !(projectile.ProjectileOwner is Player))
                return true;

            var prevHp = HP;
            var dmg = (int)StatsManager.GetDefenseDamage(this, projectile.Damage, ObjectDesc.Defense);

            HP -= dmg;

            Owner.BroadcastMessage(new DAMAGE
            {
                TargetId = Id,
                Effects = 0,
                Damage = (ushort)dmg,
                Killed = !CheckHP(),
                BulletId = projectile.ProjectileId,
                ObjectId = projectile.ProjectileOwner.Id
            }, HP <= 0 && !IsOneHit(dmg, prevHp) ? null : projectile.ProjectileOwner as Player);

            return true;
        }
    }
}