#region

using LoESoft.Core;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class LavaFissure : MapSetPiece
    {
        private static readonly string Lava = "Lava Blend";
        private static readonly string Floor = "Partial Red Floor";

        private static readonly Loot chest = new Loot(
            new TierLoot(7, ItemType.Weapon, BagType.None),
            new TierLoot(8, ItemType.Weapon, BagType.None),
            new TierLoot(9, ItemType.Weapon, BagType.None),
            new TierLoot(6, ItemType.Armor, BagType.None),
            new TierLoot(7, ItemType.Armor, BagType.None),
            new TierLoot(8, ItemType.Armor, BagType.None),
            new TierLoot(2, ItemType.Ability, BagType.None),
            new TierLoot(3, ItemType.Ability, BagType.None),
            new TierLoot(4, ItemType.Ability, BagType.None),
            new TierLoot(2, ItemType.Ring, BagType.None),
            new TierLoot(3, ItemType.Ring, BagType.None),
            new TierLoot(1, ItemType.Potion, BagType.None)
            );

        private readonly Random rand = new Random();

        public override int Size => 40;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            int[,] p = new int[Size, Size];
            const double SCALE = 5.5;
            for (int x = 0; x < Size; x++) //Lava
            {
                double t = (double)x / Size * Math.PI;
                double y1 = t / Math.Sqrt(2) - 2 * Math.Sin(t) / (SCALE * Math.Sqrt(2));
                double y2 = t / Math.Sqrt(2) + Math.Sin(t) / (SCALE * Math.Sqrt(2));
                y1 /= Math.PI / Math.Sqrt(2);
                y2 /= Math.PI / Math.Sqrt(2);

                int y1_ = (int)Math.Ceiling(y1 * Size);
                int y2_ = (int)Math.Floor(y2 * Size);
                for (int i = y1_; i < y2_; i++)
                    p[x, i] = 1;
            }

            for (int x = 0; x < Size; x++) //Floor
                for (int y = 0; y < Size; y++)
                {
                    if (p[x, y] == 1 && rand.Next() % 5 == 0)
                        p[x, y] = 2;
                }

            int r = rand.Next(0, 4); //Rotation
            for (int i = 0; i < r; i++)
                p = SetPieces.RotateCW(p);
            p[20, 20] = 2;

            EmbeddedData dat = GameServer.Manager.GameData;
            for (int x = 0; x < Size; x++) //Rendering
                for (int y = 0; y < Size; y++)
                {
                    if (p[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Lava];
                        tile.ObjType = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (p[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = dat.IdToTileType[Lava];
                        tile.ObjType = dat.IdToObjectType[Floor];
                        if (tile.ObjId == 0)
                            tile.ObjId = world.GetNextEntityId();
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }

            Entity demon = Entity.Resolve("Red Demon");
            demon.Move(pos.X + 20.5f, pos.Y + 20.5f);
            world.EnterWorld(demon);

            Container container = new Container(0x0501, null, false);
            Item[] items = chest.GetLoots(5, 8).ToArray();
            for (int i = 0; i < items.Length; i++)
                container.Inventory[i] = items[i];
            container.Move(pos.X + 20.5f, pos.Y + 20.5f);
            world.EnterWorld(container);
        }
    }
}