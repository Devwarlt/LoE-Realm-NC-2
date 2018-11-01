#region

using LoESoft.GameServer.realm.terrain;
using System.Runtime.InteropServices;

#endregion

namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class DreamIsle : MapSetPiece
    {
        public override int Size => 30;

        internal string mapName = "dreamisle";

        public unsafe override void RenderSetPiece(World world, IntPoint pos)
        {
            var h = GCHandle.Alloc(world);
            var ptr = GCHandle.ToIntPtr(h);

            var mapHandle = GCHandle.Alloc(new Wmap(GameServer.Manager.GameData));

            LoadJson(ptr.ToPointer(), mapName, &pos, GCHandle.ToIntPtr(mapHandle).ToPointer());

            var map = (mapHandle.Target as Wmap);

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    try
                    {
                        if (map[x, y].TileId != 0 && map[x, y].TileId != 255)
                        {
                            var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                            tile.TileId = map[x, y].TileId;
                            tile.ObjType = map[x, y].ObjType;
                            if (tile.ObjType != 0)
                                tile.ObjId = world.GetNextEntityId();
                            world.Map[x + pos.X, y + pos.Y] = tile;
                        }
                    }
                    catch { }
                }
            }
        }
    }
}