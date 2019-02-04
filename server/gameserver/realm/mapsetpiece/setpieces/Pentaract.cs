#region

using LoESoft.Core;
using LoESoft.GameServer.realm.terrain;
using System;

#endregion

namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class Pentaract : MapSetPiece
    {
        private static readonly string Floor = "Scorch Blend";

        private static readonly byte[,] Circle =
        {
            {0, 0, 1, 1, 1, 0, 0},
            {0, 1, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1},
            {0, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 0, 0}
        };

        public override int Size => 41;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            int[,] t = new int[41, 41];

            for (int i = 0; i < 5; i++)
            {
                double angle = (360 / 5 * i) * (float)Math.PI / 180;
                int x_ = (int)(Math.Cos(angle) * 15 + 20 - 3);
                int y_ = (int)(Math.Sin(angle) * 15 + 20 - 3);

                for (int x = 0; x < 7; x++)
                    for (int y = 0; y < 7; y++)
                    {
                        t[x_ + x, y_ + y] = Circle[x, y];
                    }
                t[x_ + 3, y_ + 3] = 2;
            }
            t[20, 20] = 3;

            EmbeddedData data = GameServer.Manager.GameData;
            for (int x = 0; x < 40; x++)
                for (int y = 0; y < 40; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = data.IdToTileType[Floor];
                        tile.ObjType = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = data.IdToTileType[Floor];
                        tile.ObjType = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;

                        Entity penta = Entity.Resolve(0x0d5e);
                        penta.Move(pos.X + x + .5f, pos.Y + y + .5f);
                        world.EnterWorld(penta);
                    }
                    else if (t[x, y] == 3)
                    {
                        Entity penta = Entity.Resolve("Pentaract");
                        penta.Move(pos.X + x + .5f, pos.Y + y + .5f);
                        world.EnterWorld(penta);
                    }
                }
        }
    }
}