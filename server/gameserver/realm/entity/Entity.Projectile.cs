#region

#endregion

namespace LoESoft.GameServer.realm.entity
{
    public class Projectile : Entity
    {
        public Projectile(ProjectileDesc desc)
            : base(GameServer.Manager.GameData.IdToObjectType[desc.ObjectId])
            => ProjDesc = desc;

        public Entity ProjectileOwner { get; set; }
        public new byte ProjectileId { get; set; }
        public short Container { get; set; }
        public int Damage { get; set; }
        public long BeginTime { get; set; }
        public Position BeginPos { get; set; }
        public float Angle { get; set; }
        public ProjectileDesc ProjDesc { get; set; }
        public int EntityId { get; set; }
        public string ObjectId { get; set; }
        public string DisplayId { get; set; }
        public bool IsAbility { get; set; }

        private const long SafeLifetimeMS = 30000; // to avoid instantly projectile dispose

        public void Destroy() => Owner.LeaveWorld(this);

        public static bool IsValidType(ProjectileDesc desc) => !desc.MultiHit && !desc.PassesCover;

        public static bool IsValidType(Projectile projectile, Entity entity) =>
            (entity is Enemy
            && !projectile.ProjDesc.MultiHit)
            || (entity is GameObject
            && (entity as GameObject).Static
            && !(entity is Wall)
            && !projectile.ProjDesc.PassesCover);

        public override void Tick(RealmTime time)
        {
            if (time.TotalElapsedMs - BeginTime >= ProjDesc.LifetimeMS + SafeLifetimeMS)
                Destroy();

            base.Tick(time);
        }

        public void ForceHit(Entity entity, RealmTime time, bool killed)
        {
            if (entity == null)
                return;

            if (!Owner.Entities.ContainsKey(entity.Id))
                return;

            if (!Owner.Projectiles.ContainsKey(this))
                Owner.Projectiles.TryAdd(this, null);

            Move(entity.X, entity.Y);

            if (entity.HitByProjectile(this, time))
                if (IsValidType(this, entity))
                    Destroy();

            UpdateCount++;
        }
    }
}