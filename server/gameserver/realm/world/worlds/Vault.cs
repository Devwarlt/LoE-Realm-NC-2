#region

using LoESoft.Core;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class Vault : World, IDungeon
    {
        private readonly ConcurrentDictionary<Tuple<Container, int>, int> _vaultChests = new ConcurrentDictionary<Tuple<Container, int>, int>();

        private readonly bool isLimbo;
        private Client psr;
        public string AccountId { get; private set; }
        private DbVault dbVault;

        public Vault(bool isLimbo, Client psr = null)
        {
            Id = (int)WorldID.VAULT_ID;
            Name = "Vault";
            Background = 2;
            this.psr = psr;
            this.isLimbo = isLimbo;
            ShowDisplays = true;
            SafePlace = true;
            if (psr != null)
                AccountId = psr.Account != null ? psr.Account.AccountId : "-1";
            else
                AccountId = "-1";
        }

        public string PlayerOwnerName { get; private set; }

        protected override void Init()
        {
            if (!(IsLimbo = isLimbo))
            {
                LoadMap("vault", MapType.Wmap);
                if (psr != null)
                    Init(psr);
                else
                    Init(null);
            }
        }

        private void Init(Client psr)
        {
            if (psr == null)
                return;
            AccountId = psr.Account.AccountId;
            PlayerOwnerName = psr.Account.Name;

            List<IntPoint> vaultChestPosition = new List<IntPoint>();
            List<IntPoint> giftChestPosition = new List<IntPoint>();
            IntPoint spawn = new IntPoint(0, 0);

            int w = Map.Width;
            int h = Map.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    WmapTile tile = Map[x, y];
                    if (tile.Region == TileRegion.Spawn)
                        spawn = new IntPoint(x, y);
                    else if (tile.Region == TileRegion.Vault)
                        vaultChestPosition.Add(new IntPoint(x, y));
                    else if (tile.Region == TileRegion.Gifting_Chest)
                        giftChestPosition.Add(new IntPoint(x, y));
                }
            vaultChestPosition.Sort((x, y) => Comparer<int>.Default.Compare(
                (x.X - spawn.X) * (x.X - spawn.X) + (x.Y - spawn.Y) * (x.Y - spawn.Y),
                (y.X - spawn.X) * (y.X - spawn.X) + (y.Y - spawn.Y) * (y.Y - spawn.Y)));

            if (psr.Account.Gifts != null)
            {
                List<GiftChest> giftChests = new List<GiftChest>();
                GiftChest c = new GiftChest
                {
                    Items = new List<Item>(8)
                };
                bool wasLastElse = false;
                int[] gifts = psr.Account.Gifts.ToArray();
                gifts.Shuffle();
                for (int i = 0; i < gifts.Count(); i++)
                {
                    if (GameServer.Manager.GameData.Items.ContainsKey((ushort)gifts[i]))
                    {
                        if (c.Items.Count < 8)
                        {
                            c.Items.Add(GameServer.Manager.GameData.Items[(ushort)gifts[i]]);
                            wasLastElse = false;
                        }
                        else
                        {
                            giftChests.Add(c);
                            c = new GiftChest
                            {
                                Items = new List<Item>(8)
                            };
                            c.Items.Add(GameServer.Manager.GameData.Items[(ushort)gifts[i]]);
                            wasLastElse = true;
                        }
                    }
                }
                if (!wasLastElse)
                    giftChests.Add(c);

                foreach (GiftChest chest in giftChests)
                {
                    if (giftChestPosition.Count == 0)
                        break;
                    while (chest.Items.Count < 8)
                        chest.Items.Add(null);
                    OneWayContainer con = new OneWayContainer(0x0744, null, false);
                    List<Item> inv = chest.Items;
                    for (int j = 0; j < 8; j++)
                        con.Inventory[j] = inv[j];
                    con.Move(giftChestPosition[0].X + 0.5f, giftChestPosition[0].Y + 0.5f);
                    EnterWorld(con);
                    giftChestPosition.RemoveAt(0);
                }
            }
            dbVault = new DbVault(psr.Account);
            for (int i = 0; i < psr.Account.VaultCount; i++)
            {
                if (vaultChestPosition.Count == 0)
                    break;
                Container con = new Container(0x0504, null, false);
                var inv = dbVault[i].Select(_ => _ == -1 ? null : (GameServer.Manager.GameData.Items.ContainsKey((ushort)_) ? GameServer.Manager.GameData.Items[(ushort)_] : null)).ToArray();
                for (int j = 0; j < 8; j++)
                    con.Inventory[j] = inv[j];
                con.Move(vaultChestPosition[0].X + 0.5f, vaultChestPosition[0].Y + 0.5f);
                EnterWorld(con);
                vaultChestPosition.RemoveAt(0);

                _vaultChests[Tuple.Create(con, i)] = con.UpdateCount;
            }

            foreach (IntPoint i in giftChestPosition)
            {
                GameObject x = new GameObject(0x0743, null, true, false, false);
                x.Move(i.X + 0.5f, i.Y + 0.5f);
                EnterWorld(x);
            }

            foreach (IntPoint i in vaultChestPosition)
            {
                SellableObject x = new SellableObject(0x0505);
                x.Move(i.X + 0.5f, i.Y + 0.5f);
                EnterWorld(x);
            }
        }

        public void AddChest(Entity original)
        {
            Container con = new Container(0x0504, null, false);
            int index = GameServer.Manager.Database.CreateChest(dbVault);
            var inv = dbVault[index].Select(_ => _ == -1 ? null : (GameServer.Manager.GameData.Items.ContainsKey((ushort)_) ? GameServer.Manager.GameData.Items[(ushort)_] : null)).ToArray();
            for (int j = 0; j < 8; j++)
                con.Inventory[j] = inv[j];
            con.Move(original.X, original.Y);
            LeaveWorld(original);
            EnterWorld(con);

            _vaultChests[Tuple.Create(con, index)] = con.UpdateCount;
        }

        public override World GetInstance(Client psr) => GameServer.Manager.AddWorld(new Vault(false, psr));

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (var i in _vaultChests)
            {
                if (i.Key.Item1.UpdateCount > i.Value)
                {
                    if (i.Key.Item1.UpdateCount > i.Value)
                    {
                        dbVault[i.Key.Item2] = i.Key.Item1.Inventory.Take(8).Select(_ => _ == null ? -1 : _.ObjectType).ToArray();
                        dbVault.FlushAsync();
                        _vaultChests[i.Key] = i.Key.Item1.UpdateCount;
                    }
                }
            }
        }

        private class GiftChest
        {
            public List<Item> Items { get; set; }
        }

        public void Reload(Client client)
        {
            psr = client;
            Init();
        }
    }
}