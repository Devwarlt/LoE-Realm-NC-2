#region

using LoESoft.Core;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class Graveyard : MapSetPiece
    {
        private static readonly string Floor = "Grass";
        private static readonly string WallA = "Grey Wall";
        private static readonly string WallB = "Destructible Grey Wall";
        private static readonly string Cross = "Cross";

        private static readonly Loot chest = new Loot(
            new TierLoot(4, ItemType.Weapon, BagType.None),
            new TierLoot(5, ItemType.Weapon, BagType.None),
            new TierLoot(6, ItemType.Weapon, BagType.None),
            new TierLoot(3, ItemType.Armor, BagType.None),
            new TierLoot(4, ItemType.Armor, BagType.None),
            new TierLoot(5, ItemType.Armor, BagType.None),
            new TierLoot(1, ItemType.Ability, BagType.None),
            new TierLoot(2, ItemType.Ability, BagType.None),
            new TierLoot(3, ItemType.Ability, BagType.None),
            new TierLoot(1, ItemType.Ring, BagType.None),
            new TierLoot(2, ItemType.Ring, BagType.None),
            new TierLoot(1, ItemType.Potion, BagType.None)
            );

        private readonly Random rand = new Random();

        public override int Size => 34;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            int[,] t = new int[23, 35];

            for (int x = 0; x < 23; x++) //Floor
                for (int y = 0; y < 35; y++)
                    t[x, y] = rand.Next() % 3 == 0 ? 0 : 1;

            for (int y = 0; y < 35; y++) //Perimeters
                t[0, y] = t[22, y] = 2;
            for (int x = 0; x < 23; x++)
                t[x, 0] = t[x, 34] = 2;

            List<IntPoint> pts = new List<IntPoint>();
            for (int y = 0; y < 11; y++) //Crosses
                for (int x = 0; x < 7; x++)
                {
                    if (rand.Next() % 3 > 0)
                        t[2 + 3 * x, 2 + 3 * y] = 4;
                    else
                        pts.Add(new IntPoint(2 + 3 * x, 2 + 3 * y));
                }

            for (int x = 0; x < 23; x++) //Corruption
                for (int y = 0; y < 35; y++)
                {
                    if (t[x, y] == 1 || t[x, y] == 0 || t[x, y] == 4)
                        continue;
                    double p = rand.NextDouble();
                    if (p < 0.1)
                        t[x, y] = 1;
                    else if (p < 0.4)
                        t[x, y]++;
                }
            //Boss & Chest
            IntPoint pt = pts[rand.Next(0, pts.Count)];
            t[pt.X, pt.Y] = 5;
            t[pt.X + 1, pt.Y] = 6;

            int r = rand.Next(0, 4);
            for (int i = 0; i < r; i++) //Rotation
                t = SetPieces.RotateCW(t);
            int w = t.GetLength(0), h = t.GetLength(1);

            EmbeddedData dat = GameServer.Manager.GameData;
            for (int x = 0; x < w; x++) //Rendering
                for (int y = 0; y < h; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Floor];
                        tile.ObjType = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Floor];
                        tile.ObjType = dat.IdToObjectType[WallA];
                        if (tile.ObjId == 0)
                            tile.ObjId = world.GetNextEntityId();
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Floor];
                        world.Map[x + pos.X, y + pos.Y] = tile;
                        Entity wall = Entity.Resolve(dat.IdToObjectType[WallB]);
                        wall.Move(x + pos.X + 0.5f, y + pos.Y + 0.5f);
                        world.EnterWorld(wall);
                    }
                    else if (t[x, y] == 4)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Floor];
                        tile.ObjType = dat.IdToObjectType[Cross];
                        if (tile.ObjId == 0)
                            tile.ObjId = world.GetNextEntityId();
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 5)
                    {
                        Container container = new Container(0x0501, null, false);
                        Item[] items = chest.GetLoots(3, 8).ToArray();
                        for (int i = 0; i < items.Length; i++)
                            container.Inventory[i] = items[i];
                        container.Move(pos.X + x + 0.5f, pos.Y + y + 0.5f);
                        world.EnterWorld(container);
                    }
                    else if (t[x, y] == 6)
                    {
                        Entity mage = Entity.Resolve("Deathmage");
                        mage.Move(pos.X + x, pos.Y + y);
                        world.EnterWorld(mage);
                    }
                }
        }
    }
}