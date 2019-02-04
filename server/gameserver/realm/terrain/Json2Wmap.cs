#region

using Ionic.Zlib;
using LoESoft.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace LoESoft.GameServer.realm.terrain
{
    public class Json2Wmap
    {
        private struct obj
        {
#pragma warning disable CS0649 // Field 'Json2Wmap.obj.name' is never assigned to, and will always have its default value null
            public string name;
#pragma warning restore CS0649 // Field 'Json2Wmap.obj.name' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'Json2Wmap.obj.id' is never assigned to, and will always have its default value null
            public string id;
#pragma warning restore CS0649 // Field 'Json2Wmap.obj.id' is never assigned to, and will always have its default value null
        }

        private struct loc
        {
#pragma warning disable CS0649 // Field 'Json2Wmap.loc.ground' is never assigned to, and will always have its default value null
            public string ground;
#pragma warning restore CS0649 // Field 'Json2Wmap.loc.ground' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'Json2Wmap.loc.objs' is never assigned to, and will always have its default value null
            public obj[] objs;
#pragma warning restore CS0649 // Field 'Json2Wmap.loc.objs' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'Json2Wmap.loc.regions' is never assigned to, and will always have its default value null
            public obj[] regions;
#pragma warning restore CS0649 // Field 'Json2Wmap.loc.regions' is never assigned to, and will always have its default value null
        }

        private struct json_dat
        {
#pragma warning disable CS0649 // Field 'Json2Wmap.json_dat.data' is never assigned to, and will always have its default value null
            public byte[] data;
#pragma warning restore CS0649 // Field 'Json2Wmap.json_dat.data' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'Json2Wmap.json_dat.width' is never assigned to, and will always have its default value 0
            public int width;
#pragma warning restore CS0649 // Field 'Json2Wmap.json_dat.width' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'Json2Wmap.json_dat.height' is never assigned to, and will always have its default value 0
            public int height;
#pragma warning restore CS0649 // Field 'Json2Wmap.json_dat.height' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'Json2Wmap.json_dat.dict' is never assigned to, and will always have its default value null
            public loc[] dict;
#pragma warning restore CS0649 // Field 'Json2Wmap.json_dat.dict' is never assigned to, and will always have its default value null
        }

        public static void Convert(EmbeddedData data, string from, string to)
        {
            byte[] x = Convert(data, File.ReadAllText(from));
            File.WriteAllBytes(to, x);
        }

        public static byte[] Convert(EmbeddedData data, string json)
        {
            json_dat obj = JsonConvert.DeserializeObject<json_dat>(json);
            byte[] dat = ZlibStream.UncompressBuffer(obj.data);

            Dictionary<short, TerrainTile> tileDict = new Dictionary<short, TerrainTile>();
            for (int i = 0; i < obj.dict.Length; i++)
            {
                loc o = obj.dict[i];
                tileDict[(short)i] = new TerrainTile
                {
                    TileId = o.ground == null ? (ushort)0xff : data.IdToTileType[o.ground],
                    TileObj = o.objs == null ? null : o.objs[0].id,
                    Name = o.objs == null ? "" : o.objs[0].name ?? "",
                    Terrain = TerrainType.None,
                    Region = o.regions == null ? TileRegion.None : (TileRegion)Enum.Parse(typeof(TileRegion), o.regions[0].id.Replace(' ', '_'))
                };
            }

            TerrainTile[,] tiles = new TerrainTile[obj.width, obj.height];
            using (NReader rdr = new NReader(new MemoryStream(dat)))
                for (int y = 0; y < obj.height; y++)
                    for (int x = 0; x < obj.width; x++)
                    {
                        tiles[x, y] = tileDict[rdr.ReadInt16()];
                    }
            return WorldMapExporter.Export(tiles);
        }
    }
}