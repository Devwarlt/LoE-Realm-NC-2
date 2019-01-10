#region

using LoESoft.GameServer.networking.outgoing;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    public partial class Player
    {
        private IEnumerable<Entity> GetNewEntities()
        {
            var newEntities = new List<Entity>();

            try { Owner.Players.Where(i => clientEntities.Add(i.Value)).Select(_ => { newEntities.Add(_.Value); return _; }).ToList(); }
            catch { }

            try { Owner.PlayersCollision.HitTest(X, Y, SIGHTRADIUS).OfType<Decoy>().Where(i => clientEntities.Add(i)).Select(_ => { newEntities.Add(_); return _; }).ToList(); }
            catch { }

            try
            {
                Owner.EnemiesCollision.HitTest(X, Y, SIGHTRADIUS).Where(_ => MathsUtils.DistSqr(_.X, _.Y, X, Y) <= SIGHTRADIUS * SIGHTRADIUS).Select(_ =>
                {
                    if (_ is Container)
                    {
                        var owner = (_ as Container).BagOwners?.Length == 1 ? (_ as Container).BagOwners[0] : null;

                        if (owner != null && owner != AccountId)
                            return _;

                        if (owner == AccountId)
                            if ((LootDropBoost || LootTierBoost) && (_.ObjectType != 0x500 || _.ObjectType != 0x506))
                                (_ as Container).BoostedBag = true;
                    }

                    if (visibleTiles.ContainsKey(new IntPoint((int)_.X, (int)_.Y)))
                        if (clientEntities.Add(_))
                            newEntities.Add(_);

                    return _;
                }).ToList();
            }
            catch { }

            try
            {
                if (Quest != null && clientEntities.Add(Quest) && (Quest as Enemy).HP >= 0)
                    newEntities.Add(Quest);
            }
            catch { }

            return newEntities;
        }

        private IEnumerable<int> GetRemovedEntities()
        {
            var removedEntities = new List<int>();

            try
            {
                clientEntities.Where(entity =>
                    !(entity is Player && entity.Owner != null) &&
                    ((MathsUtils.DistSqr(entity.X, entity.Y, X, Y) > SIGHTRADIUS * SIGHTRADIUS &&
                    !(entity is GameObject && (entity as GameObject).Static) && entity != Quest) ||
                    entity.Owner == null))
                .Select(clientEntity =>
                {
                    removedEntities.Add(clientEntity.Id);

                    return clientEntities;
                }).ToList();
            }
            catch { }

            return removedEntities;
        }

        private IEnumerable<ObjectDef> GetNewStatics(int xBase, int yBase)
        {
            var world = GameServer.Manager.GetWorld(Owner.Id);
            var ret = new List<ObjectDef>();
            try
            {
                blocksight = world.Dungeon ? Sight.RayCast(this, SIGHTRADIUS) : Sight.GetSightCircle(SIGHTRADIUS);
                blocksight.Where(_ =>
                {
                    var x = _.X + xBase;
                    var y = _.Y + yBase;
                    var t = Owner.Map[x, y];

                    return !(x < 0 || x >= Owner.Map.Width || y < 0 || y >= Owner.Map.Height || t.ObjId == 0 || t.ObjType == 0 || !clientStatic.Add(new IntPoint(x, y)) || t.ObjDesc == null);
                }).Select(_ =>
                {
                    var x = _.X + xBase;
                    var y = _.Y + yBase;
                    var t = Owner.Map[x, y];
                    var d = t.ToDef(x, y);
                    var c = t.ObjDesc.Class;

                    if (c == "ConnectedWall" || c == "CaveWall")
                        if (d.Stats.Stats.Count(__ => __.Key == StatsType.CONNECT_STAT && __.Value != null) == 0)
                            d.Stats.Stats = new KeyValuePair<StatsType, object>[] {
                            new KeyValuePair<StatsType, object>(StatsType.CONNECT_STAT,
                            (int)ConnectionComputer.Compute((xx, yy) => Owner.Map[x + xx, y + yy].ObjType == t.ObjType).Bits)
                        };

                    ret.Add(d);

                    return _;
                }).ToList();
            }
            catch { }

            return ret;
        }

        private IEnumerable<IntPoint> GetRemovedStatics(int xBase, int yBase)
        {
            try
            {
                return clientStatic.Where(_ =>
                {
                    var x = _.X - xBase;
                    var y = _.Y - yBase;
                    var t = Owner.Map[x, y];

                    return (x * x + y * y > SIGHTRADIUS * SIGHTRADIUS || t.ObjType == 0) && t.ObjId != 0;
                }).ToList();
            }
            catch { }

            return new List<IntPoint>();
        }

        public void HandleUpdate(RealmTime time)
        {
            var world = GameServer.Manager.GetWorld(Owner.Id);
            var sendEntities = new HashSet<Entity>(GetNewEntities());
            var tilesUpdate = new List<UPDATE.TileData>(APPOX_AREA_OF_SIGHT);

            blocksight = world.Dungeon ? Sight.RayCast(this, SIGHTRADIUS) : Sight.GetSightCircle(SIGHTRADIUS);

            try
            {
                blocksight.Where(_ =>
                {
                    var x = _.X + (int)X;
                    var y = _.Y + (int)Y;

                    return !(x < 0 || x >= Owner.Map.Width || y < 0 || y >= Owner.Map.Height || tiles[x, y] >= Owner.Map[x, y].UpdateCount);
                }).Select(_ =>
                {
                    var x = _.X + (int)X;
                    var y = _.Y + (int)Y;
                    var t = Owner.Map[x, y];

                    if (!visibleTiles.ContainsKey(new IntPoint(x, y)))
                        visibleTiles[new IntPoint(x, y)] = true;

                    tilesUpdate.Add(new UPDATE.TileData
                    {
                        X = (short)x,
                        Y = (short)y,
                        Tile = t.TileId
                    });

                    tiles[x, y] = t.UpdateCount;

                    return _;
                }).ToList();
            }
            catch { }

            var dropEntities = GetRemovedEntities().Distinct().ToArray();

            clientEntities.RemoveWhere(_ => Array.IndexOf(dropEntities, _.Id) != -1);

            lastUpdate.Keys.Where(i => !clientEntities.Contains(i)).ToList().Select(_ =>
            {
                lastUpdate.TryRemove(_, out int __);
                return _;
            }).ToList();

            sendEntities.Select(_ => { lastUpdate[_] = _.UpdateCount; return _; }).ToList();

            var newStatics = GetNewStatics((int)X, (int)Y);
            var removeStatics = GetRemovedStatics((int)X, (int)Y);
            var removedIds = new List<int>();

            if (!world.Dungeon)
                removeStatics.ToArray().Select(_ =>
                {
                    removedIds.Add(Owner.Map[_.X, _.Y].ObjId);
                    clientStatic.Remove(_);

                    return _;
                }).ToList();

            if (sendEntities.Count > 0 || tilesUpdate.Count > 0 || dropEntities.Length > 0 || newStatics.ToArray().Length > 0 || removedIds.Count > 0)
            {
                Client.SendMessage(new UPDATE()
                {
                    Tiles = tilesUpdate.ToArray(),
                    NewObjects = sendEntities.Select(_ => _.ToDefinition()).Concat(newStatics.ToArray()).ToArray(),
                    RemovedObjectIds = dropEntities.Concat(removedIds).ToArray()
                });
                AwaitUpdateAck(time.TotalElapsedMs);
            }
        }

        private void HandleNewTick(RealmTime time)
        {
            var sendEntities = new List<Entity>();

            try
            {
                clientEntities.Where(i => i?.UpdateCount > lastUpdate[i]).Select(_ =>
                {
                    sendEntities.Add(_);
                    lastUpdate[_] = _.UpdateCount;

                    return _;
                }).ToList();
            }
            catch (Exception) { }

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