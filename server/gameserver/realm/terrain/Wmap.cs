#region

using Ionic.Zlib;
using LoESoft.Core;
using LoESoft.GameServer.realm.entity;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace LoESoft.GameServer.realm.terrain
{
    public enum TileRegion : byte
    {
        None,
        Spawn,
        Realm_Portals,
        Store_1,
        Store_2,
        Store_3,
        Store_4,
        Store_5,
        Store_6,
        Vault,
        Loot,
        Defender,
        Hallway,
        Enemy,
        Hallway_1,
        Hallway_2,
        Hallway_3,
        Store_7,
        Store_8,
        Store_9,
        Gifting_Chest,
        Store_10,
        Store_11,
        Store_12,
        Store_13,
        Store_14,
        Store_15,
        Store_16,
        Store_17,
        Store_18,
        Store_19,
        Store_20,
        Store_21,
        Store_22,
        Store_23,
        Store_24,
        PetRegion,
        Outside_Arena,
        Item_Spawn_Point,
        Arena_Central_Spawn,
        Arena_Edge_Spawn,
        Store_25,
        Store_26,
        Store_27,
        Store_28,
        Store_29,
        Store_30,
        QuestMonsterRegion,
        Store_32,
        Store_33,
        Store_34,
        Store_35,
        Store_36,
        Store_37,
        Store_38,
        Store_39,
        Store_40
    }

    public enum WmapTerrain : byte
    {
        None,
        Mountains,
        HighSand,
        HighPlains,
        HighForest,
        MidSand,
        MidPlains,
        MidForest,
        LowSand,
        LowPlains,
        LowForest,
        ShoreSand,
        ShorePlains,
        BeachTowels
    }

    public struct WmapTile
    {
        public byte Elevation;
        public string Name;
        public int ObjId;
        public ushort ObjType;
        public TileRegion Region;
        public WmapTerrain Terrain;
        public ushort TileId;
        public bool Spawned;
        public byte UpdateCount;
        public ObjectDesc ObjDesc;
        public TileDesc TileDesc;

        public ObjectDef ToDef(int x, int y)
        {
            List<KeyValuePair<StatsType, object>> stats = new List<KeyValuePair<StatsType, object>>();
            if (!string.IsNullOrEmpty(Name))
                foreach (string item in Name.Split(';'))
                {
                    string[] kv = item.Split(':');
                    switch (kv[0])
                    {
                        case "name":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.NAME_STAT, kv[1]));
                            break;

                        case "size":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.SIZE_STAT, Utils.FromString(kv[1])));
                            break;

                        case "eff":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.CONDITION_STAT, Utils.FromString(kv[1])));
                            break;

                        case "conn":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.CONNECT_STAT,
                                Utils.FromString(kv[1])));
                            break;

                        case "hp":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.HP_STAT, Utils.FromString(kv[1])));
                            break;

                        case "mcost":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.MERCHANDISE_PRICE_STAT,
                                Utils.FromString(kv[1])));
                            break;

                        case "mcur":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.MERCHANDISE_CURRENCY_STAT,
                                Utils.FromString(kv[1])));
                            break;

                        case "mtype":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.MERCHANDISE_TYPE_STAT,
                                Utils.FromString(kv[1])));
                            break;

                        case "mcount":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.MERCHANDISE_COUNT_STAT,
                                Utils.FromString(kv[1])));
                            break;

                        case "mtime":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.MERCHANDISE_MINS_LEFT_STAT,
                                Utils.FromString(kv[1])));
                            break;

                        case "stars":
                            stats.Add(new KeyValuePair<StatsType, object>(StatsType.MERCHANDISE_RANK_REQ_STAT,
                                Utils.FromString(kv[1])));
                            break;
                            //case "nstar":
                            //    entity.Stats[StatsType.NameChangerStar] = Utils.FromString(kv[1]); break;
                    }
                }
            return new ObjectDef
            {
                ObjectType = ObjType,
                Stats = new ObjectStatusData
                {
                    Id = ObjId,
                    Position = new Position
                    {
                        X = x + 0.5f,
                        Y = y + 0.5f
                    },
                    Stats = stats.ToArray()
                }
            };
        }

        public WmapTile Clone()
        {
            return new WmapTile
            {
                UpdateCount = (byte)(UpdateCount + 1),
                TileId = TileId,
                Name = Name,
                ObjType = ObjType,
                Terrain = Terrain,
                Region = Region,
                ObjId = ObjId,
                TileDesc = TileDesc,
                ObjDesc = ObjDesc
            };
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class Wmap : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Wmap));

        private readonly EmbeddedData data;
        public HashSet<Tuple<IntPoint, TileRegion>> Regions { get; private set; } // not the best idea to make public
        private Tuple<IntPoint, ushort, string>[] entities;
        private WmapTile[,] tilesOriginal;
        private WmapTile[,] tiles;

        public Wmap(EmbeddedData data)
        {
            Regions = new HashSet<Tuple<IntPoint, TileRegion>>();
            this.data = data;
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public WmapTile this[int x, int y]
        {
            get
            {
                try
                {
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                        return tiles[x, y];
                    else
                        return new WmapTile();
                }
                catch { return new WmapTile(); }
            }
            set { tiles[x, y] = value; }
        }

        public int Load(Stream stream, int idBase)
        {
            var ver = stream.ReadByte();
            if (ver < 0 || ver > 2)
                throw new NotSupportedException("WMap version " + ver);

            using (var rdr = new BinaryReader(new ZlibStream(stream, CompressionMode.Decompress)))
            {
                var dict = new List<WmapTile>();
                var c = rdr.ReadInt16();
                for (var i = 0; i < c; i++)
                {
                    var tile = new WmapTile
                    {
                        TileId = rdr.ReadUInt16()
                    };
                    tile.TileDesc = data.Tiles[tile.TileId];
                    var obj = rdr.ReadString();
                    tile.ObjType = string.IsNullOrEmpty(obj) ? (ushort)0 : data.IdToObjectType[obj];
                    tile.Name = rdr.ReadString();
                    tile.Terrain = (WmapTerrain)rdr.ReadByte();
                    tile.Region = (TileRegion)rdr.ReadByte();
                    if (ver == 1)
                        tile.Elevation = rdr.ReadByte();
                    data.ObjectDescs.TryGetValue(tile.ObjType, out tile.ObjDesc);
                    dict.Add(tile);
                }

                Width = rdr.ReadInt32();
                Height = rdr.ReadInt32();
                tilesOriginal = new WmapTile[Width, Height];
                tiles = new WmapTile[Width, Height];

                var enCount = 0;
                var entities = new List<Tuple<IntPoint, ushort, string>>();
                for (var y = 0; y < Height; y++)
                    for (var x = 0; x < Width; x++)
                    {
                        var tile = dict[rdr.ReadInt16()].Clone();
                        tile.UpdateCount = 1;
                        if (ver == 2)
                            tile.Elevation = rdr.ReadByte();

                        if (tile.Region != 0)
                            Regions.Add(new Tuple<IntPoint, TileRegion>(
                                new IntPoint(x, y), tile.Region));

                        var desc = tile.ObjDesc;
                        if (tile.ObjType != 0 && (desc == null || !desc.Static || desc.Enemy))
                        {
                            entities.Add(new Tuple<IntPoint, ushort, string>(new IntPoint(x, y), tile.ObjType, tile.Name));
                            if (desc == null || !(desc.Enemy && desc.Static))
                                tile.ObjType = 0;
                        }

                        if (tile.ObjType != 0 && (desc == null || !(desc.Enemy && desc.Static)))
                        {
                            enCount++;
                            tile.ObjId = idBase + enCount;
                        }

                        tilesOriginal[x, y] = tile;
                        tiles[x, y] = tile;
                    }

                this.entities = entities.ToArray();
                return enCount;
            }
        }

        private bool isGuildMerchant(ushort objId)
        {
            return objId == 1846 || objId == 1847 || objId == 1848;
        }

        public IEnumerable<Entity> InstantiateEntities()
        {
            foreach (Tuple<IntPoint, ushort, string> i in entities)
            {
                Entity entity = Entity.Resolve(i.Item2);
                entity.Move(i.Item1.X + 0.5f, i.Item1.Y + 0.5f);
                if (i.Item3 != null)
                    foreach (string item in i.Item3.Split(';'))
                    {
                        string[] kv = item.Split(':');
                        switch (kv[0])
                        {
                            case "name":
                                entity.Name = kv[1];
                                break;

                            case "size":
                                entity.Size = Utils.FromString(kv[1]);
                                break;

                            case "eff":
                                entity.ConditionEffects = (ConditionEffects)Utils.FromString(kv[1]);
                                break;

                            case "conn":
                                (entity as ConnectedObject).Connection =
                                    ConnectionInfo.Infos[(uint)Utils.FromString(kv[1])];
                                break;
                            //case "mtype":
                            //    (entity as Merchants).custom = true;
                            //    (entity as Merchants).mType = Utils.FromString(kv[1]);
                            //    break;
                            //case "mcount":
                            //    entity.Stats[StatsType.MerchantRemainingCount] = Utils.FromString(kv[1]); break;    NOT NEEDED FOR NOW
                            //case "mtime":
                            //    entity.Stats[StatsType.MerchantRemainingMinute] = Utils.FromString(kv[1]); break;
                            case "mcost":
                                (entity as SellableObject).Price = Utils.FromString(kv[1]);
                                break;

                            case "mcur":
                                (entity as SellableObject).Currency = (CurrencyType)Utils.FromString(kv[1]);
                                break;

                            case "stars":
                                (entity as SellableObject).RankReq = Utils.FromString(kv[1]);
                                break;
                                //case "nstar":
                                //    entity.Stats[StatsType.NameChangerStar] = Utils.FromString(kv[1]); break;
                        }
                    }
                yield return entity;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            entities = null;
            tiles = null;
        }

        public bool Contains(IntPoint p)
        {
            return Contains(p.X, p.Y);
        }

        public bool Contains(int x, int y)
        {
            return (x >= 0 && x <= Width) && (y >= 0 && y <= Height);
        }
    }
}