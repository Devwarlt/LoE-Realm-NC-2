#region

using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm
{
    public class PortalMonitor
    {
        private readonly Dictionary<int, Portal> _portals;
        private readonly World _world;
        private readonly RealmManager _manager;
        private readonly Random _rand;
        private readonly object _worldLock;

        public PortalMonitor(RealmManager manager, World world)
        {
            _manager = manager;
            _world = world;
            _portals = new Dictionary<int, Portal>();
            _rand = new Random();
            _worldLock = new object();
        }

        public Position GetRandPosition()
        {
            var x = 0;
            var y = 0;
            var realmPortalRegions = _world.Map.Regions.Where(t => t.Item2 == TileRegion.Realm_Portals).ToArray();

            if (realmPortalRegions.Length > _portals.Count)
            {
                Tuple<IntPoint, TileRegion> sRegion;
                do sRegion = realmPortalRegions.ElementAt(_rand.Next(0, realmPortalRegions.Length));
                while (_portals.Values.Any(p => p.X == sRegion.Item1.X + 0.5f && p.Y == sRegion.Item1.Y + 0.5f));

                x = sRegion.Item1.X;
                y = sRegion.Item1.Y;
            }
            return new Position() { X = x, Y = y };
        }

        public void AddRealm(World world)
        {
            using (TimedLock.Lock(_worldLock))
            {
                var pos = GetRandPosition();
                var portal = new Portal(0x0712, null)
                {
                    Size = 80,
                    WorldInstance = world,
                    Name = world.Name
                };
                portal.Move(pos.X + 0.5f, pos.Y + 0.5f);

                _world.EnterWorld(portal);
                _portals.Add(world.Id, portal);
            }
        }

        public bool RemoveWorld(World world)
        {
            if (_world == null)
                return false;

            using (TimedLock.Lock(_worldLock))
            {
                var portal = _portals.FirstOrDefault(p => p.Value.WorldInstance == world);

                if (portal.Value == null)
                    return false;

                _world.LeaveWorld(portal.Value);
                _portals.Remove(portal.Key);

                return world.Delete();
            }
        }

        public World GetRandomRealm()
        {
            using (TimedLock.Lock(_worldLock))
            {
                var worlds = _portals.Values.ToArray();

                if (worlds.Length == 0)
                    return GameServer.Manager.Worlds[(int)WorldID.NEXUS_ID];

                return worlds[Environment.TickCount % worlds.Length].WorldInstance;
            }
        }

        public void Tick(RealmTime t)
        {
            if (_world == null)
                return;

            using (TimedLock.Lock(_worldLock))
            {
                foreach (var p in _portals.Values)
                {
                    if (p.WorldInstance == null || p.WorldInstance.Deleted)
                        continue;

                    var count = p.WorldInstance.Players.Count;
                    var updatedCount = p.WorldInstance.Name + $" ({count}/{p.WorldInstance.MaxPlayersCount})";

                    if (p.Name.Equals(updatedCount))
                        continue;

                    p.Name = updatedCount;
                }
            }
        }
    }
}