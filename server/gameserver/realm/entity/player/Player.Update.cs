#region

using LoESoft.GameServer.networking.outgoing;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    public partial class Player
    {
        public readonly int SIGHTSQUARED = SIGHTRADIUS * SIGHTRADIUS;

        public void HandleUpdate(RealmTime time)
        {
            var tilesUpdate = new HashSet<UPDATE.TileData>();
            var removedIds = new List<int>();

            var world = GameServer.Manager.GetWorld(Owner.Id);
            var newEntites = GetNewEntites();
            var newStatics = GetNewStatics(GameServer.Manager.GetWorld(Owner.Id), (int)X, (int)Y);
            var droppedEntities = GetRemovedEntities().Distinct().ToList();
            var droppedStatics = GetRemovedStatics((int)X, (int)Y);

            blocksight = world.Dungeon ? Sight.RayCast(this, SIGHTRADIUS) : Sight.GetSightCircle(SIGHTRADIUS);

            foreach (var i in blocksight.ToArray())
            {
                int x = i.X + (int)X;
                int y = i.Y + (int)Y;

                if (!(x < 0 || x >= Owner.Map.Width || y < 0 || y >= Owner.Map.Height || tiles[x, y] >= Owner.Map[x, y].UpdateCount))
                {
                    var tile = Owner.Map[x, y];
                    var point = new IntPoint(x, y);

                    if (!visibleTiles.ContainsKey(point))
                        visibleTiles[point] = true;

                    tilesUpdate.Add(new UPDATE.TileData
                    {
                        X = (short)x,
                        Y = (short)y,
                        Tile = tile.TileId
                    });

                    tiles[x, y] = tile.UpdateCount;
                }
            }

            clientEntities.RemoveWhere(_ =>
            {
                if (droppedEntities.Contains(_.Id))
                    if (droppedEntities[droppedEntities.IndexOf(_.Id)] != -1)
                    {
                        if (lastUpdate.ContainsKey(_))
                            lastUpdate.TryRemove(_, out int dropped);
                        return true;
                    }
                return false;
            });

            foreach (var i in newEntites)
                lastUpdate[i] = i.UpdateCount;

            if (!world.Dungeon)
                foreach (var i in droppedStatics)
                {
                    removedIds.Add(Owner.Map[i.X, i.Y].ObjId);
                    clientStatic.Remove(i);
                }

            if (newEntites.Count() > 0 || tilesUpdate.Count > 0 || droppedEntities.Count() > 0 || newStatics.Count() > 0 ||
                removedIds.Count() > 0)
            {
                Client.SendMessage(new UPDATE()
                {
                    Tiles = tilesUpdate.ToArray(),
                    NewObjects = newEntites.Select(_ => _.ToDefinition()).Concat(newStatics.ToArray()).ToArray(),
                    RemovedObjectIds = droppedEntities.Concat(removedIds).ToArray()
                });
            }
        }

        private IEnumerable<Entity> GetNewEntites()
        {
            var newEntities = new HashSet<Entity>();

            foreach (var i in Owner.Players.Where(_ => clientEntities.Add(_.Value)))
                newEntities.Add(i.Value);
            foreach (var i in Owner.PlayersCollision.HitTest(X, Y, SIGHTRADIUS).OfType<Decoy>().Where(_ => clientEntities.Add(_)))
                newEntities.Add(i);
            foreach (var i in Owner.EnemiesCollision.HitTest(X, Y, SIGHTRADIUS).Where(_ => MathsUtils.DistSqr(_.X, _.Y, X, Y) <= SIGHTRADIUS * SIGHTRADIUS))
            {
                if (i is Container contianer)
                {
                    var owner = contianer.BagOwners?.Length == 1 ? contianer.BagOwners[0] : null;

                    if (owner != null && owner != AccountId)
                        break;

                    if (owner == AccountId && (LootDropBoost || LootTierBoost) && (i.ObjectType != 0x500 || i.ObjectType != 0x506))
                        contianer.BoostedBag = true;
                }

                if (visibleTiles.ContainsKey(new IntPoint((int)i.X, (int)i.Y)))
                    if (clientEntities.Add(i))
                        newEntities.Add(i);
            }

            return newEntities;
        }

        private IEnumerable<int> GetRemovedEntities()
            => clientEntities.ToList().Where(_ => !(_ is Player && _.Owner != null)
            && ((MathsUtils.DistSqr(_.X, _.Y, X, Y) > SIGHTSQUARED && !(_ is GameObject && ((GameObject)_).Static) && _ != Quest) ||
            _.Owner != null)).Select(e => e.Id).ToList();

        private HashSet<ObjectDef> GetNewStatics(World world, int xBase, int yBase)
        {
            var set = new HashSet<ObjectDef>();

            blocksight = world.Dungeon ? Sight.RayCast(this, SIGHTRADIUS) : Sight.GetSightCircle(SIGHTRADIUS);

            foreach (var i in blocksight.ToArray())
            {
                var x = i.X + xBase;
                var y = i.Y + yBase;
                var tile = Owner.Map[x, y];

                if (!(x < 0 || x >= Owner.Map.Width || y < 0 || y >= Owner.Map.Height || tile.ObjId == 0 || tile.ObjType == 0
                    || !clientStatic.Add(new IntPoint(x, y)) || tile.ObjDesc == null))
                {
                    var def = tile.ToDef(x, y);
                    var tclass = tile.ObjDesc.Class;

                    if (tclass == "ConnectedWall" || tclass == "CaveWall")
                        if (def.Stats.Stats.Count(_ => _.Key == StatsType.CONNECT_STAT && _.Value != null) == 0)
                            def.Stats.Stats = new KeyValuePair<StatsType, object>[]
                            {
                                new KeyValuePair<StatsType, object>(StatsType.CONNECT_STAT,  (int)ConnectionComputer.Compute(
                                    (cx, cy) => Owner.Map[cx + x, cy +y].ObjType == tile.ObjType).Bits)
                            };

                    set.Add(def);
                }
            }

            return set;
        }

        private IEnumerable<IntPoint> GetRemovedStatics(int xBase, int yBase) => clientStatic.ToList().Where(_ =>
        {
            var x = _.X - xBase;
            var y = _.Y - yBase;
            var tile = Owner.Map[x, y];

            return (x * x + y * y > SIGHTSQUARED || tile.ObjType == 0) && tile.ObjId != 0;
        });

        private void HandleNewTick(RealmTime time)
        {
            var sendEntities = new HashSet<Entity>();

            foreach (var i in clientEntities.Where(_ => (lastUpdate.ContainsKey(_))))
            {
                if (i?.UpdateCount > lastUpdate[i])
                {
                    sendEntities.Add(i);
                    lastUpdate[i] = i.UpdateCount;
                }
            }

            if (Quest != null && (!lastUpdate.ContainsKey(Quest) || Quest.UpdateCount > lastUpdate[Quest]))
            {
                sendEntities.Add(Quest);
                lastUpdate[Quest] = Quest.UpdateCount;
            }

            Client.SendMessage(new NEWTICK()
            {
                TickId = tickId++,
                TickTime = time.ElapsedMsDelta,
                Statuses = sendEntities.Select(_ => _.ExportStats()).ToArray()
            });

            blocksight.Clear();
        }
    }
}