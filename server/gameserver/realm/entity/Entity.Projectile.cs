#region

using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

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

        private bool _once { get; set; }
        private Timer _lifetime { get; set; }
        private List<string> _damagedPlayers { get; set; } = new List<string>();

        public override void Tick(RealmTime time)
        {
            if (!_once)
            {
                _once = true;
                _lifetime = new Timer(ProjDesc.LifetimeMS) { AutoReset = false };
                _lifetime.Elapsed += delegate { Destroy(); };
                _lifetime.Start();
            }

            if (ProjectileOwner is Enemy)
            {
                if (ProjDesc.MultiHit)
                {
                    var players = Array.ConvertAll((this as Entity).GetNearestEntities(1).ToArray(), player => (Player)player);

                    if (players.Length != 0)
                        players.Where(player => player != null).Where(player => !_damagedPlayers.Contains(player.Name))
                            .Select(player =>
                        {
                            _damagedPlayers.Add(player.Name);

                            if (player.Client.Socket.Connected)
                            {
                                if (ProjDesc.Effects.Length != 0)
                                    foreach (var effect in ProjDesc.Effects)
                                        if (effect.Target == 1)
                                            continue;
                                        else
                                            player.ApplyConditionEffect(effect);

                                player.ForceHit(Damage, ProjectileOwner, ProjDesc.ArmorPiercing);
                            }
                            return player;
                        }).ToArray();
                }
                else
                {
                    if ((this as Entity).GetNearestEntity(1, true) is Player player)
                        if (player.Client.Socket.Connected)
                        {
                            ProjectileOwner.Owner.RemoveProjectileFromId(ProjectileOwner.Id, ProjectileId);

                            if (ProjDesc.Effects.Length != 0)
                                foreach (var effect in ProjDesc.Effects)
                                    if (effect.Target == 1)
                                        continue;
                                    else
                                        player.ApplyConditionEffect(effect);

                            player.ForceHit(Damage, ProjectileOwner, ProjDesc.ArmorPiercing);
                        }
                }
            }

            base.Tick(time);
        }

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