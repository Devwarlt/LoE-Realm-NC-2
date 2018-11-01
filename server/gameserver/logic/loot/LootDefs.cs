#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic.loot
{
    public interface ILootDef
    {
        string Lootstate { get; set; }

        void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            );
    }

    public class ProcessWhiteBag : ILootDef
    {
        private readonly bool eventChest;
        private readonly ILootDef[] loot;

        public string Lootstate { get; set; }

        public ProcessWhiteBag(bool eventChest = false, params ILootDef[] loot)
        {
            this.eventChest = eventChest;
            this.loot = loot;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData == null)
                return;

            Tuple<Player, int>[] enemyData = enemy.DamageCounter.GetPlayerData();

            int damageData = GetDamageData(enemyData);
            double enemyHP = enemy.ObjectDesc.MaxHP;

            if (damageData >= enemyHP * .2)
            {
                double chance = eventChest ? .01 : .05;
                double rng = rnd.NextDouble();

                if (rng <= chance)
                    foreach (ILootDef i in loot)
                        i.Populate(enemy, playerData, rnd, Lootstate, lootDefs);
            }
        }

        private int GetDamageData(IEnumerable<Tuple<Player, int>> data)
        {
            List<int> damages = data.Select(_ => _.Item2).ToList();
            int totalDamage = 0;
            for (int i = 0; i < damages.Count; i++)
                totalDamage += damages[i];
            return totalDamage;
        }
    }

    public class PinkBag : ILootDef
    {
        private readonly ItemType itemType;
        private readonly byte tier;

        public string Lootstate { get; set; }

        public PinkBag(ItemType itemType, byte tier)
        {
            this.itemType = itemType;
            this.tier = tier;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData == null)
                return;

            double rng = rnd.NextDouble();
            double probability = .15 * Settings.WOTMG_RATE; // 15%

            ILootDef[] pinkbag = new ILootDef[] { new Drops(new TierLoot(tier, itemType, probability)) };

            if (rng <= probability)
                pinkbag[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public class PurpleBag : ILootDef
    {
        private readonly ItemType itemType;
        private readonly byte tier;

        public string Lootstate { get; set; }

        public PurpleBag(ItemType itemType, byte tier)
        {
            this.itemType = itemType;
            this.tier = tier;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData == null)
                return;

            double rng = rnd.NextDouble();
            double probability = .15 * Settings.WOTMG_RATE; // 15%

            ILootDef[] pinkbag = new ILootDef[] { new Drops(new TierLoot(tier, itemType, probability)) };

            if (rng <= probability * Settings.WOTMG_RATE)
                pinkbag[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public enum EggType : int
    {
        TIER_0 = 0,
        TIER_1 = 4,
        TIER_2 = 8,
        TIER_3 = 12,
        TIER_4 = 16,
        TIER_5 = 20,
        TIER_6 = 24,
        TIER_7 = 28
    }

    public class EggBasket : ILootDef
    {
        private readonly EggType rarity;

        public string Lootstate { get; set; }

        public EggBasket(EggType rarity)
        {
            this.rarity = rarity;
        }

        public EggBasket(EggType[] rarity)
        {
            this.rarity = rarity[new Random().Next(0, rarity.Length)];
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData != null)
                return;

            Item[] candidates = GameServer.Manager.GameData.Items
                .Where(item => item.Value.SlotType == 9000)
                .Where(item => item.Value.MinStars <= (int) rarity)
                .Select(item => item.Value)
                .ToArray();

            Item onlyOne = candidates[new Random().Next(0, candidates.Length)];

            double rng = rnd.NextDouble();
            double probability = 0;

            if (onlyOne.MinStars < 8)
                probability = .15; // 15%
            else if (onlyOne.MinStars >= 8 && onlyOne.MinStars < 12)
                probability = .05; // 5%
            else if (onlyOne.MinStars >= 12 && onlyOne.MinStars < 20)
                probability = .01; // 1%
            else
                probability = .0025; // 0.25%

            probability *= Settings.WOTMG_RATE;

            ILootDef[] eggbasket = new ILootDef[] { new Drops(new ItemLoot(onlyOne.DisplayId, probability)) };

            if (rng <= probability * Settings.WOTMG_RATE)
                eggbasket[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public class CyanBag : ILootDef
    {
        private readonly string itemName;
        private readonly ItemType itemType;
        private readonly byte tier;
        private readonly bool setByTier;

        public string Lootstate { get; set; }

        public CyanBag(string itemName)
        {
            this.itemName = itemName;
            setByTier = false;
        }

        public CyanBag(string[] itemName)
        {
            this.itemName = itemName[new Random().Next(0, itemName.Length)];
            setByTier = false;
        }

        public CyanBag(ItemType itemType, byte tier)
        {
            this.itemType = itemType;
            this.tier = tier;
            setByTier = true;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData == null)
                return;

            double rng = rnd.NextDouble();
            double probability = .01 * Settings.WOTMG_RATE; // 1%

            ILootDef[] cyanbag = !setByTier ? new ILootDef[] { new Drops(new ItemLoot(itemName, probability)) } :
                new ILootDef[] { new Drops(new TierLoot(tier, itemType, probability)) };

            if (rng <= probability * Settings.WOTMG_RATE)
                cyanbag[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public sealed class Potions
    {
        public readonly static string POTION_OF_LIFE = "Potion of Life";
        public readonly static string POTION_OF_MANA = "Potion of Mana";
        public readonly static string POTION_OF_ATTACK = "Potion of Attack";
        public readonly static string POTION_OF_DEFENSE = "Potion of Defense";
        public readonly static string POTION_OF_SPEED = "Potion of Speed";
        public readonly static string POTION_OF_DEXTERITY = "Potion of Dexterity";
        public readonly static string POTION_OF_VITALITY = "Potion of Vitality";
        public readonly static string POTION_OF_WISDOM = "Potion of Wisdom";
    }

    public sealed class GreaterPotions
    {
        public readonly static string GREATER_POTION_OF_LIFE = "Greater Potion of Life";
        public readonly static string GREATER_POTION_OF_MANA = "Greater Potion of Mana";
        public readonly static string GREATER_POTION_OF_ATTACK = "Greater Potion of Attack";
        public readonly static string GREATER_POTION_OF_DEFENSE = "Greater Potion of Defense";
        public readonly static string GREATER_POTION_OF_SPEED = "Greater Potion of Speed";
        public readonly static string GREATER_POTION_OF_DEXTERITY = "Greater Potion of Dexterity";
        public readonly static string GREATER_POTION_OF_VITALITY = "Greater Potion of Vitality";
        public readonly static string GREATER_POTION_OF_WISDOM = "Greater Potion of Wisdom";
    }

    public class BlueBag : ILootDef
    {
        private readonly string itemName;
        private readonly string[] itemNames;
        private readonly bool alwaysDrop;
        private readonly bool[] alwaysDrops;
        private readonly bool single;

        public string Lootstate { get; set; }

        public BlueBag(string itemName, bool alwaysDrop = false)
        {
            this.itemName = itemName;
            this.alwaysDrop = alwaysDrop;
            single = true;
        }

        public BlueBag(string[] itemNames, bool[] alwaysDrops)
        {
            this.itemNames = itemNames;
            this.alwaysDrops = alwaysDrops;
            single = false;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData == null)
                return;

            double rng = rnd.NextDouble();
            double probability = !alwaysDrop ? .25 * Settings.WOTMG_RATE : 1; // 25% (always drop: 100%)

            if (!single)
            {
                List<KeyValuePair<string, bool>> itemNamesDrops = new List<KeyValuePair<string, bool>>();

                if (itemNames.Length != alwaysDrops.Length)
                    return;

                for (int i = 0; i < itemNames.Length; i++)
                    itemNamesDrops.Add(new KeyValuePair<string, bool>(itemNames[i], alwaysDrops[i]));

                List<ILootDef> validateItems = new List<ILootDef>();

                foreach (KeyValuePair<string, bool> itemNamesDrop in itemNamesDrops)
                    validateItems.Add(new MostDamagers(3, new ItemLoot(itemNamesDrop.Key, !itemNamesDrop.Value ? .25 : 1)));

                ILootDef[] bluebags = validateItems.ToArray();

                if (rng <= probability * Settings.WOTMG_RATE)
                    foreach (var i in bluebags)
                        i.Populate(enemy, playerData, rnd, Lootstate, lootDefs);

                return;
            }

            ILootDef[] bluebag = new ILootDef[] { new MostDamagers(3, new ItemLoot(itemName, probability)) };

            if (rng <= probability * Settings.WOTMG_RATE)
                bluebag[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public class WhiteBag : ILootDef
    {
        private readonly bool eventChest;
        private readonly string itemName;

        public string Lootstate { get; set; }

        public WhiteBag(string itemName, bool eventChest = false)
        {
            this.eventChest = eventChest;
            this.itemName = itemName;
        }

        public WhiteBag(string[] itemName, bool eventChest = false)
        {
            this.eventChest = eventChest;
            this.itemName = itemName[new Random().Next(0, itemName.Length)];
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerData,
            Random rnd,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerData == null)
                return;

            List<Tuple<Player, int>> candidates = enemy.DamageCounter.GetPlayerData().ToList();
            double rng = rnd.NextDouble();
            double probability = 0;

            int playersCount = candidates.Count;
            int mostDamagers = 0;
            int enemyHP = (int) enemy.ObjectDesc.MaxHP;

            if (playersCount == 1)
                probability = .0001;
            else if (playersCount > 1 && playersCount <= 3)
                probability = .0001 + .0001 * playersCount;
            else
            {
                probability = .0002 + .000005 * playersCount;

                if (playersCount >= 30)
                    mostDamagers = playersCount / 10;
            }

            probability *= Settings.WOTMG_RATE;

            ILootDef[] whitebag = mostDamagers >= 3 ?
                new ILootDef[] { new MostDamagers(mostDamagers, new ItemLoot(itemName, probability * (eventChest ? .8 : 1))) } :
                new ILootDef[] { new Drops(new ItemLoot(itemName, probability * (eventChest ? .8 : 1))) };

            if (rng <= probability * Settings.WOTMG_RATE)
                whitebag[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public class ItemLoot : ILootDef
    {
        private readonly string item;
        private readonly double probability;

        public string Lootstate { get; set; }

        public ItemLoot(string item, double probability)
        {
            this.item = item;
            this.probability = probability;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;

            if (playerDat != null)
                return;

            EmbeddedData dat = GameServer.Manager.GameData;

            try
            {
                lootDefs.Add(new LootDef(dat.Items[dat.IdToObjectType[item]], probability, lootState));
            }
            catch (KeyNotFoundException)
            {
                Log.Error($"Item '{item}', wasn't added in loot list of entity '{enemy.Name}', because doesn't contains in assets.");
            }
        }
    }

    public class LootState : ILootDef
    {
        private readonly ILootDef[] children;

        public string Lootstate { get; set; }

        public LootState(string subState, params ILootDef[] lootDefs)
        {
            children = lootDefs;
            Lootstate = subState;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            foreach (ILootDef i in children)
                i.Populate(enemy, playerDat, rand, Lootstate, lootDefs);
        }
    }

    public enum ItemType
    {
        Weapon,
        Ability,
        Armor,
        Ring,
        Potion,
        Any
    }

    public enum EggRarity
    {
        Egg_0To13Stars = 0,
        Egg_14To27Stars = 14,
        Egg_28To41Stars = 28,
        Egg_42To48Stars = 42,
        Egg_49Stars = 49
    }

    public class EggLoot : ILootDef
    {
        private readonly EggRarity rarity;
        private readonly double probability;

        public string Lootstate { get; set; }

        public EggLoot(EggRarity rarity, double probability)
        {
            this.probability = probability;
            this.rarity = rarity;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;
            if (playerDat != null)
                return;
            Item[] candidates = GameServer.Manager.GameData.Items
                .Where(item => item.Value.SlotType == 9000)
                .Where(item => item.Value.MinStars <= (int) rarity)
                .Select(item => item.Value)
                .ToArray();
            foreach (Item i in candidates)
                lootDefs.Add(new LootDef(i, probability / candidates.Length, lootState));
        }
    }

    public class TierLoot : ILootDef
    {
        public static readonly int[] WeaponSlotType = { 1, 2, 3, 8, 17, 24 };
        public static readonly int[] AbilitySlotType = { 4, 5, 11, 12, 13, 15, 16, 18, 19, 20, 21, 22, 23, 25 };
        public static readonly int[] ArmorSlotType = { 6, 7, 14 };
        public static readonly int[] RingSlotType = { 9 };
        public static readonly int[] PotionSlotType = { 10 };
        private readonly double probability;

        private readonly byte tier;
        private readonly int[] types;

        public string Lootstate { get; set; }

        public TierLoot(byte tier, ItemType type, double probability = 0)
        {
            this.tier = tier;
            switch (type)
            {
                case ItemType.Weapon:
                    types = WeaponSlotType;
                    break;

                case ItemType.Ability:
                    types = AbilitySlotType;
                    break;

                case ItemType.Armor:
                    types = ArmorSlotType;
                    break;

                case ItemType.Ring:
                    types = RingSlotType;
                    break;

                case ItemType.Potion:
                    types = PotionSlotType;
                    break;

                default:
                    throw new NotSupportedException(type.ToString());
            }
            this.probability = probability != 0 ? probability : ReturnProbability(type, tier);
        }

        private double ReturnProbability(ItemType type, byte tier)
        {
            double _tier = tier + 1;
            double probability = 0;
            switch (type)
            {
                case ItemType.Weapon:
                case ItemType.Armor:
                    probability = 5 / (_tier * 10);
                    break;

                case ItemType.Ability:
                case ItemType.Ring:
                    probability = 2.5 / (_tier * 10);
                    break;
            }
            return probability;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;
            if (playerDat != null)
                return;
            Item[] candidates = GameServer.Manager.GameData.Items
                .Where(item => Array.IndexOf(types, item.Value.SlotType) != -1)
                .Where(item => item.Value.Tier == tier)
                .Select(item => item.Value)
                .ToArray();
            foreach (Item i in candidates)
                lootDefs.Add(new LootDef(i, probability / candidates.Length, lootState));
        }
    }

    public class Threshold : ILootDef
    {
        private readonly ILootDef[] children;
        private readonly double threshold;

        public string Lootstate { get; set; }

        public Threshold(double threshold, params ILootDef[] children)
        {
            this.threshold = threshold;
            this.children = children;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;
            if (playerDat != null && playerDat.Item2 / enemy.ObjectDesc.MaxHP >= threshold)
            {
                foreach (ILootDef i in children)
                    i.Populate(enemy, null, rand, lootState, lootDefs);
            }
        }
    }

    public class Drops : ILootDef
    {
        private readonly ILootDef[] children;

        public string Lootstate { get; set; }

        public Drops(params ILootDef[] children)
        {
            this.children = children;
        }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            Lootstate = lootState;
            if (playerDat != null)
            {
                foreach (ILootDef i in children)
                    i.Populate(enemy, null, rand, lootState, lootDefs);
            }
        }
    }

    internal class MostDamagers : ILootDef
    {
        private readonly ILootDef[] loots;
        private readonly int amount;

        public MostDamagers(int amount, params ILootDef[] loots)
        {
            this.amount = amount;
            this.loots = loots;
        }

        public string Lootstate { get; set; }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            var data = enemy.DamageCounter.GetPlayerData();
            var mostDamage = GetMostDamage(data);
            foreach (var loot in mostDamage.Where(pl => pl.Equals(playerDat)).SelectMany(pl => loots))
                loot.Populate(enemy, null, rand, lootState, lootDefs);
        }

        private IEnumerable<Tuple<Player, int>> GetMostDamage(IEnumerable<Tuple<Player, int>> data)
        {
            var damages = data.Select(_ => _.Item2).ToList();
            var len = damages.Count < amount ? damages.Count : amount;
            for (var i = 0; i < len; i++)
            {
                var val = damages.Max();
                yield return data.FirstOrDefault(_ => _.Item2 == val);
                damages.Remove(val);
            }
        }
    }

    public class OnlyOne : ILootDef
    {
        private readonly ILootDef[] loots;

        public OnlyOne(params ILootDef[] loots)
        {
            this.loots = loots;
        }

        public string Lootstate { get; set; }

        public void Populate(
            Enemy enemy,
            Tuple<Player, int> playerDat,
            Random rand,
            string lootState,
            IList<LootDef> lootDefs
            )
        {
            loots[rand.Next(0, loots.Length)].Populate(enemy, playerDat, rand, lootState, lootDefs);
        }
    }

    public static class LootTemplates
    {
        public static ILootDef[] DefaultEggLoot(EggRarity maxRarity)
        {
            switch (maxRarity)
            {
                case EggRarity.Egg_0To13Stars:
                    return new ILootDef[1] { new EggLoot(EggRarity.Egg_0To13Stars, 0.1) };

                case EggRarity.Egg_14To27Stars:
                    return new ILootDef[2] { new EggLoot(EggRarity.Egg_0To13Stars, 0.1), new EggLoot(EggRarity.Egg_14To27Stars, 0.05) };

                case EggRarity.Egg_28To41Stars:
                    return new ILootDef[3] { new EggLoot(EggRarity.Egg_0To13Stars, 0.1), new EggLoot(EggRarity.Egg_14To27Stars, 0.05), new EggLoot(EggRarity.Egg_28To41Stars, 0.01) };

                case EggRarity.Egg_42To48Stars:
                    return new ILootDef[4] { new EggLoot(EggRarity.Egg_0To13Stars, 0.1), new EggLoot(EggRarity.Egg_14To27Stars, 0.05), new EggLoot(EggRarity.Egg_28To41Stars, 0.01), new EggLoot(EggRarity.Egg_42To48Stars, 0.001) };

                default:
                    throw new InvalidOperationException("Not a valid Egg Rarity");
            }
        }

        public static ILootDef[] StatIncreasePotionsLoot()
        {
            return new ILootDef[]
            {
                new OnlyOne(
                    new ItemLoot("Potion of Defense", 1),
                    new ItemLoot("Potion of Attack", 1),
                    new ItemLoot("Potion of Speed", 1),
                    new ItemLoot("Potion of Vitality", 1),
                    new ItemLoot("Potion of Wisdom", 1),
                    new ItemLoot("Potion of Dexterity", 1)
                )
            };
        }
    }
}