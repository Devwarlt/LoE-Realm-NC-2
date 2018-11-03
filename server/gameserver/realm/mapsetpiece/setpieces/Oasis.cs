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
    internal class Oasis : MapSetPiece
    {
        private static readonly string Floor = "Light Grass";
        private static readonly string Water = "Shallow Water";
        private static readonly string Tree = "Palm Tree";

        private static readonly Loot chest = new Loot(
            new TierLoot(5, ItemType.Weapon, BagType.None),
            new TierLoot(6, ItemType.Weapon, BagType.None),
            new TierLoot(7, ItemType.Weapon, BagType.None),
            new TierLoot(4, ItemType.Armor, BagType.None),
            new TierLoot(5, ItemType.Armor, BagType.None),
            new TierLoot(6, ItemType.Armor, BagType.None),
            new TierLoot(2, ItemType.Ability, BagType.None),
            new TierLoot(3, ItemType.Ability, BagType.None),
            new TierLoot(1, ItemType.Ring, BagType.None),
            new TierLoot(2, ItemType.Ring, BagType.None),
            new TierLoot(1, ItemType.Potion, BagType.None)
            );

        private readonly Random rand = new Random();

        public override int Size => 30;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            int outerRadius = 13;
            int waterRadius = 10;
            int islandRadius = 3;
            List<IntPoint> border = new List<IntPoint>();

            int[,] t = new int[Size, Size];
            for (int y = 0; y < Size; y++) //Outer
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size / 2.0);
                    double dy = y - (Size / 2.0);
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= outerRadius)
                        t[x, y] = 1;
                }

            for (int y = 0; y < Size; y++) //Water
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size / 2.0);
                    double dy = y - (Size / 2.0);
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= waterRadius)
                    {
                        t[x, y] = 2;
                        if (waterRadius - r < 1)
                            border.Add(new IntPoint(x, y));
                    }
                }

            for (int y = 0; y < Size; y++) //Island
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size / 2.0);
                    double dy = y - (Size / 2.0);
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= islandRadius)
                    {
                        t[x, y] = 1;
                        if (islandRadius - r < 1)
                            border.Add(new IntPoint(x, y));
                    }
                }

            HashSet<IntPoint> trees = new HashSet<IntPoint>();
            while (trees.Count < border.Count * 0.5)
                trees.Add(border[rand.Next(0, border.Count)]);

            foreach (IntPoint i in trees)
                t[i.X, i.Y] = 3;

            EmbeddedData dat = GameServer.Manager.GameData;
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
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
                        tile.TileId = dat.IdToTileType[Water];
                        tile.ObjType = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Floor];
                        tile.ObjType = dat.IdToObjectType[Tree];
                        tile.Name = "size:" + (rand.Next() % 2 == 0 ? 120 : 140);
                        if (tile.ObjId == 0)
                            tile.ObjId = world.GetNextEntityId();
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }

            Entity giant = Entity.Resolve("Oasis Giant");
            giant.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(giant);

            Container container = new Container(0x0501, null, false);
            Item[] items = chest.GetLoots(5, 8).ToArray();
            for (int i = 0; i < items.Length; i++)
                container.Inventory[i] = items[i];
            container.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(container);
        }
    }
}