#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic.loot
{
    public struct LootDef
    {
        public readonly Item Item;
        public readonly double Probabilty;
        public readonly string LootState;
        public readonly bool Shared;
        public readonly bool WhiteBag;

        public LootDef(Item item, double probabilty, string lootState, bool shared = true, bool whiteBag = false)
        {
            Item = item;
            Probabilty = probabilty;
            LootState = lootState;
            Shared = shared;
            WhiteBag = whiteBag;
        }
    }

    public class Loot : List<ILootDef>
    {
        private static readonly Random rand = new Random();

        public Loot(params ILootDef[] lootDefs) //For independent loots(e.g. chests)
        {
            AddRange(lootDefs);
        }

        public IEnumerable<Item> GetLoots(int min, int max) //For independent loots(e.g. chests)
        {
            var consideration = new List<LootDef>();

            foreach (var i in this)
                i.Populate(null, null, rand, "", consideration);

            var retCount = rand.Next(min, max);

            foreach (var i in consideration)
            {
                if (rand.NextDouble() < i.Probabilty)
                {
                    yield return i.Item;
                    retCount--;
                }
                if (retCount == 0)
                    yield break;
            }
        }

        public void Handle(Enemy enemy, RealmTime time)
        {
            try
            {
                if (enemy == null)
                    return;

                if (enemy.Owner == null)
                    return;

                var consideration = new List<LootDef>();
                var sharedLoots = new List<Item>();

                foreach (var i in this)
                    i.Populate(enemy, null, rand, i.Lootstate, consideration);

                foreach (var i in consideration)
                {
                    if (!i.Shared)
                        continue;
                    if (i.LootState == enemy.LootState || i.LootState == null)
                        if (rand.NextDouble() <= i.Probabilty)
                            sharedLoots.Add(i.Item);
                }

                var dats = enemy.DamageCounter.GetPlayerData();
                var loots = enemy.DamageCounter.GetPlayerData().ToDictionary(d => d.Item1, d => (IList<Item>)new List<Item>());

                foreach (var loot in sharedLoots.Where(item => item.BagType > 1))
                    loots[dats[rand.Next(dats.Length)].Item1].Add(loot);

                foreach (var dat in dats)
                {
                    consideration.Clear();

                    foreach (var i in this)
                        i.Populate(enemy, dat, rand, i.Lootstate, consideration);

                    var playerLoot = loots[dat.Item1];

                    foreach (var i in consideration)
                    {
                        if (i.LootState == enemy.LootState || i.LootState == null)
                        {
                            var prob = dat.Item1.LootDropBoost ? i.Probabilty * 1.5 : i.Probabilty;
                            var chance = rand.NextDouble();

                            if (chance <= prob)
                            {
                                if (dat.Item1.LootTierBoost)
                                    playerLoot.Add(IncreaseTier(GameServer.Manager, i.Item, consideration));
                                else
                                    playerLoot.Add(i.Item);

                                if (i.WhiteBag)
                                    GameServer.Manager.ClientManager.Values.Select(client =>
                                    {
                                        client.Client.SendMessage(new TEXT()
                                        {
                                            BubbleTime = 0,
                                            Stars = -1,
                                            Name = "@ANNOUNCEMENT",
                                            Text = $" {dat.Item1.Name} dropped a white bag item '{i.Item.DisplayId}' with {chance * 100}% chance!",
                                            NameColor = 0x123456,
                                            TextColor = 0x123456
                                        });

                                        return client;
                                    }).ToList();
                            }
                        }
                    }
                }

                AddBagsToWorld(enemy, sharedLoots, loots);
            }
            catch (IndexOutOfRangeException) { return; }
        }

        private Item IncreaseTier(RealmManager manager, Item item, List<LootDef> consideration)
        {
            if (item.SlotType == 10)
                return item;

            var tier = manager.GameData.Items
                 .Where(i => item.SlotType == i.Value.SlotType)
                 .Where(i => i.Value.Tier >= item.Tier + 3)
                 .Where(i => consideration.Select(_ => _.Item).Contains(i.Value))
                 .Select(i => i.Value).ToArray();

            return tier.Length > 0 ? tier[rand.Next(1, tier.Length)] : item;
        }

        private void AddBagsToWorld(Enemy enemy, IList<Item> shared, IDictionary<Player, IList<Item>> soulbound)
        {
            var pub = new List<Player>(); //only people not getting soulbound

            foreach (var i in soulbound)
            {
                if (i.Value.Count > 0)
                    ShowBags(enemy, i.Value, i.Key);
                else
                    pub.Add(i.Key);
            }

            if (pub.Count > 0 && shared.Count > 0)
                ShowBags(enemy, shared, null);
        }

        private void ShowBags(Enemy enemy, IEnumerable<Item> loots, params Player[] owners)
        {
            var ownerIds = owners?.Select(x => x.AccountId).ToArray();
            var bagType = 0;
            var items = new Item[8];
            var idx = 0;

            foreach (var i in loots)
            {
                if (i.BagType > bagType)
                    bagType = i.BagType;

                items[idx] = i;
                idx++;

                if (idx == 8)
                {
                    ShowBag(enemy, ownerIds, bagType, items);

                    bagType = 0;
                    items = new Item[8];
                    idx = 0;
                }
            }

            if (idx > 0)
                ShowBag(enemy, ownerIds, bagType, items);
        }

        private static void ShowBag(Enemy enemy, string[] owners, int bagType, Item[] items)
        {
            ushort bag = 0x500;

            switch (bagType)
            {
                case 0:
                    bag = 0x500;
                    break;

                case 1:
                    bag = 0x506;
                    break;

                case 2:
                    bag = 0x503;
                    break;

                case 3:
                    bag = 0x508;
                    break;

                case 4:
                    bag = 0x509;
                    break;

                case 5:
                    bag = 0x050B;
                    break;

                case 6:
                    bag = 0x050C;
                    break;

                case 7:
                    bag = 0xfff;
                    break;
            }

            if (owners == null)
            {
                var container = new Container(bag, bagType == 6 ? 5 * 60 * 1000 : 30 * 1000, true);

                for (int j = 0; j < 8; j++)
                    container.Inventory[j] = items[j];

                container.BagOwners = null;
                container.Move(
                    enemy.X + (float)((rand.NextDouble() * 2 - 1) * 0.5),
                    enemy.Y + (float)((rand.NextDouble() * 2 - 1) * 0.5));
                container.Size = 80;

                enemy.Owner.EnterWorld(container);
            }
            else
                foreach (var owner in owners)
                {
                    var container = new Container(bag, bagType == 6 ? 5 * 60 * 1000 : 30 * 1000, true);

                    for (int j = 0; j < 8; j++)
                        container.Inventory[j] = items[j];

                    container.BagOwners = new string[] { owner };
                    container.Move(
                        enemy.X + (float)((rand.NextDouble() * 2 - 1) * 0.5),
                        enemy.Y + (float)((rand.NextDouble() * 2 - 1) * 0.5));
                    container.Size = 80;

                    enemy.Owner.EnterWorld(container);
                }
        }
    }
}