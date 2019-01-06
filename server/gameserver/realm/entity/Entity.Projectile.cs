#region

using System.Collections.Concurrent;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    public class Projectile : Entity
    {
        public Projectile(ProjectileDesc desc)
            : base(GameServer.Manager.GameData.IdToObjectType[desc.ObjectId])
            => ProjDesc = desc;

        public static ConcurrentDictionary<int, bool> ProjectileCache = new ConcurrentDictionary<int, bool>();

        public static void Add(int id) => ProjectileCache.TryAdd(id, false);

        public static void Remove(int id) => ProjectileCache.TryRemove(id, out bool val);

        public Entity ProjectileOwner { get; set; }
        public new byte ProjectileId { get; set; }
        public short Container { get; set; }
        public int Damage { get; set; }
        public long BeginTime { get; set; }
        public Position BeginPos { get; set; }
        public float Angle { get; set; }
        public ProjectileDesc ProjDesc { get; set; }

        public void Destroy() => Owner.LeaveWorld(this);

        public bool IsValidType(Entity entity) =>
            (entity is Enemy
            && !ProjDesc.MultiHit)
            || (entity is GameObject
            && (entity as GameObject).Static
            && !(entity is Wall)
            && !ProjDesc.PassesCover);

        public void ForceHit(Entity entity, RealmTime time, bool killed)
        {
            if (entity == null)
                return;

            if (!Owner.Entities.ContainsKey(entity.Id))
                return;

            if (!ProjectileCache.ContainsKey(ProjectileId))
                Add(ProjectileId);
            else
                return;

            Move(entity.X, entity.Y);

            if (entity.HitByProjectile(this, time))
                if (IsValidType(entity))
                    Remove(ProjectileId);
                else
                {
                    Remove(ProjectileId);
                    Destroy();
                }

            UpdateCount++;
        }
    }
}