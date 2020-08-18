#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using LoESoft.GameServer.networking.outgoing;
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
        BagType BagType { get; set; }

        void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs);
    }

    public class LootBagRate
    {
        public const double PINK_BAG = 0.025;
        public const double PURPLE_BAG = 0.015;
        public const double CYAN_BAG = 0.005;
        public const double WHITE_BAG = 0.0001;
    }

    public enum BagType
    {
        Pink,
        Purple,
        Cyan,
        Blue,
        White,
        None
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

    public class LootBoosters
    {
        private Enemy _enemy { get; set; }
        private Tuple<Player, int> _playerData { get; set; }
        private Random _rnd { get; set; }
        private string _lootState { get; set; }
        private IList<LootDef> _lootDefs { get; set; }
        private List<Player> PlayersData { get; set; }
        private double Chance { get; set; }
        private int EnemyHP { get; set; }
        private int Players { get; set; }
        private int PlayersBelowLvl20 { get; set; }
        private int PlayersMaxed { get; set; }

        public LootBoosters(double chance, Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            _enemy = enemy;
            _playerData = playerData;
            _rnd = rnd;
            _lootState = lootState;
            _lootDefs = lootDefs;

            var players = _enemy.DamageCounter.GetPlayerData().ToList();

            PlayersData = players.Where(player => player.Item1 != null).Select(player => player.Item1).ToList();
            Chance = chance;
            EnemyHP = _enemy.HP;
            Players = players.Count;
            PlayersBelowLvl20 = players.Where(playerCache => playerCache.Item1.Level < 20).Count();
            PlayersMaxed = players.Where(playerCache =>
                playerCache.Item1.Stats[0] == playerCache.Item1.ObjectDesc.MaxHitPoints &&
                playerCache.Item1.Stats[1] == playerCache.Item1.ObjectDesc.MaxMagicPoints &&
                playerCache.Item1.Stats[2] == playerCache.Item1.ObjectDesc.MaxAttack &&
                playerCache.Item1.Stats[3] == playerCache.Item1.ObjectDesc.MaxDefense &&
                playerCache.Item1.Stats[4] == playerCache.Item1.ObjectDesc.MaxSpeed &&
                playerCache.Item1.Stats[5] == playerCache.Item1.ObjectDesc.MaxHpRegen &&
                playerCache.Item1.Stats[6] == playerCache.Item1.ObjectDesc.MaxMpRegen &&
                playerCache.Item1.Stats[7] == playerCache.Item1.ObjectDesc.MaxDexterity).Count();
        }

        private double GetEnemyHPBoost
            => Math.Log10(EnemyHP);

        private double GetPlayersBoost
            => 1 / (Players * 5);

        private double GetPlayersBelowLvl20Boost
            => 0.01 / PlayersBelowLvl20;

        private double GetPlayersMaxedBoost
            => 0.02 / PlayersMaxed;

        private double GetTotalChance
            => Chance + GetEnemyHPBoost * GetPlayersBoost + GetPlayersBelowLvl20Boost + GetPlayersMaxedBoost;

        public void UpdateLootCache(string objectId, ILootDef[] loot)
            => PlayersData.Select(player =>
            {
                var rng = new Random().NextDouble(0, 100);
                var chance = GetTotalChance;
                var success = rng <= chance;
                var lootCache = player.LootCaches.FirstOrDefault(item => item.ObjectId == objectId);

                if (lootCache != null)
                {
                    if (lootCache.Attempts + 1 >= lootCache.MaxAttempts || success)
                    {
                        loot[0].Populate(_enemy, _playerData, _rnd, _lootState, _lootDefs);

                        var msg = $"{player.Name} dropped a white bag item '{objectId}' (on {lootCache.Attempts} of {lootCache.MaxAttempts} attempt{(lootCache.MaxAttempts > 1 ? "s" : "")})!";

                        Log.Info(msg);

                        PlayersData.Where(target => target != null).Select(target =>
                        {
                            target.Client.SendMessage(new TEXT()
                            {
                                ObjectId = -1,
                                BubbleTime = 10,
                                Stars = 70,
                                Name = "NPC Gazer",
                                Admin = 0,
                                Recipient = target.Name,
                                Text = msg.ToSafeText(),
                                CleanText = "",
                                NameColor = 0x123456,
                                TextColor = 0x123456
                            });

                            return target;
                        }).ToList();

                        LootCache.Utils.UpdateTotal(player.LootCaches, objectId);
                    }
                    else
                    {
                        Log.Info($"{player.Name} failed to obtain a white bag item '{objectId}' (on {lootCache.Attempts} of {lootCache.MaxAttempts} attempt{(lootCache.MaxAttempts > 1 ? "s" : "")})!");

                        LootCache.Utils.UpdateAttempts(player.LootCaches, objectId);
                        LootCache.Utils.UpdateMaxAttempts(player.LootCaches, objectId, (int)(100 / chance));
                    }
                }
                else
                {
                    if (success)
                    {
                        loot[0].Populate(_enemy, _playerData, _rnd, _lootState, _lootDefs);

                        var msg = $"{player.Name} dropped a white bag item '{objectId}' (on 1st attempt)!";

                        Log.Info(msg);

                        PlayersData.Where(target => target != null).Select(target =>
                        {
                            target.Client.SendMessage(new TEXT()
                            {
                                ObjectId = -1,
                                BubbleTime = 10,
                                Stars = 70,
                                Name = "NPC Gazer",
                                Admin = 0,
                                Recipient = target.Name,
                                Text = msg.ToSafeText(),
                                CleanText = "",
                                NameColor = 0x123456,
                                TextColor = 0x123456
                            });

                            return target;
                        }).ToList();
                    }
                    else
                    {
                        Log.Info($"{player.Name} failed to obtain a white bag item '{objectId}' (on 1st attempt)!");

                        player.LootCaches.Add(new LootCache()
                        {
                            ObjectId = objectId,
                            Attempts = 1,
                            MaxAttempts = (int)(100 / chance),
                            AttemptsBase = (int)(100 / chance)
                        });
                    }
                }

                return player;
            }).ToList();
    }

    public class ProcessWhiteBag : ILootDef
    {
        private readonly bool eventChest;
        private readonly ILootDef[] loot;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

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

    public class EggBasket : ILootDef
    {
        private readonly EggType rarity;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

        public EggBasket(EggType rarity)
        {
            this.rarity = rarity;
        }

        public EggBasket(EggType[] rarity)
        {
            this.rarity = rarity[Environment.TickCount % rarity.Length];
        }

        public void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            Lootstate = lootState;

            var candidates = GameServer.Manager.GameData.Items
                .Where(item => item.Value.SlotType == 9000)
                .Where(item => item.Value.MinStars <= (int)rarity)
                .Select(item => item.Value)
                .ToArray();

            var onlyOne = candidates[Environment.TickCount % candidates.Length];

            double probability = 0;

            if (onlyOne.MinStars < 8)
                probability = .15; // 15%
            else if (onlyOne.MinStars >= 8 && onlyOne.MinStars < 12)
                probability = .05; // 5%
            else if (onlyOne.MinStars >= 12 && onlyOne.MinStars < 20)
                probability = .01; // 1%
            else
                probability = .0025; // 0.25%

            var eggBasket = new[]
            {
                new Drops(
                    new ItemLoot(onlyOne.ObjectType, LootUtils.GetProbability(enemy, probability), false)
                    )
            };
            eggBasket[0].Populate(enemy, playerData, rnd, lootState, lootDefs);
        }
    }

    public class PinkBag : ILootDef
    {
        private readonly ItemType itemType;
        private readonly byte tier;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

        public PinkBag(ItemType itemType, byte tier)
        {
            this.itemType = itemType;
            this.tier = tier;
        }

        public void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            BagType = BagType.Pink;
            Lootstate = lootState;

            var pinkBag = new[]
            {
                new Drops(
                    new TierLoot(tier, itemType, BagType, true, LootUtils.GetProbability(enemy, LootBagRate.PINK_BAG))
                    )
            };
            pinkBag[0].Populate(enemy, playerData, rnd, lootState, lootDefs);
        }
    }

    public class PurpleBag : ILootDef
    {
        private readonly ItemType itemType;
        private readonly byte tier;

        public BagType BagType { get; set; }
        public string Lootstate { get; set; }

        public PurpleBag(ItemType itemType, byte tier)
        {
            this.itemType = itemType;
            this.tier = tier;
        }

        public void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            BagType = BagType.Purple;
            Lootstate = lootState;

            var purpleBag = new[]
            {
                new Drops(
                    new TierLoot(tier, itemType, BagType, false, LootUtils.GetProbability(enemy, LootBagRate.PURPLE_BAG))
                    )
            };
            purpleBag[0].Populate(enemy, playerData, rnd, lootState, lootDefs);
        }
    }

    public class CyanBag : ILootDef
    {
        private readonly string itemName;
        private readonly ItemType itemType;
        private readonly byte tier;
        private readonly bool setByTier;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

        public CyanBag(string itemName)
        {
            this.itemName = itemName;
            setByTier = false;
        }

        public CyanBag(string[] itemName)
        {
            this.itemName = itemName[Environment.TickCount % itemName.Length];
            setByTier = false;
        }

        public CyanBag(ItemType itemType, byte tier)
        {
            this.itemType = itemType;
            this.tier = tier;
            setByTier = true;
        }

        public void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            BagType = BagType.Cyan;
            Lootstate = lootState;

            var cyanBag = !setByTier ?
                new[]
                {
                    new Drops(
                        new ItemLoot(itemName, LootUtils.GetProbability(enemy, LootBagRate.CYAN_BAG), false, false)
                        )
                } : new[]
                {
                    new Drops(
                        new TierLoot(tier, itemType, BagType, false, LootUtils.GetProbability(enemy, LootBagRate.CYAN_BAG))
                    )
                };
            cyanBag[0].Populate(enemy, playerData, rnd, lootState, lootDefs);
        }
    }

    public class BlueBag : ILootDef
    {
        private readonly string itemName;
        private readonly string[] itemNames;
        private readonly bool alwaysDrop;
        private readonly bool[] alwaysDrops;
        private readonly bool single;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

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

        public void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            BagType = BagType.Blue;
            Lootstate = lootState;

            if (single)
            {
                var blueBag = new ILootDef[] { new Drops(new ItemLoot(itemName, alwaysDrop ? 1 : 0.1, false)) };
                blueBag[0].Populate(enemy, playerData, rnd, lootState, lootDefs);
            }
            else
            {
                var blueBag = new List<ILootDef>();

                for (int i = 0; i < itemNames.Length; i++)
                    blueBag.Add(new Drops(new ItemLoot(itemNames[i], alwaysDrops[i] ? 1 : 0.1, false)));

                blueBag.Select(drop => { drop.Populate(enemy, playerData, rnd, lootState, lootDefs); return drop; }).ToList();
            }
        }
    }

    public class WhiteBag : ILootDef
    {
        private readonly bool eventChest;
        private readonly string itemName;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

        public WhiteBag(string itemName, bool eventChest = false)
        {
            this.eventChest = eventChest;
            this.itemName = itemName;
        }

        public WhiteBag(string[] itemName, bool eventChest = false)
        {
            this.eventChest = eventChest;
            this.itemName = itemName[Environment.TickCount % itemName.Length];
        }

        public void Populate(Enemy enemy, Tuple<Player, int> playerData, Random rnd, string lootState, IList<LootDef> lootDefs)
        {
            BagType = BagType.Blue;
            Lootstate = lootState;

            var whitebag = new[]
            {
                new Drops(
                    new ItemLoot(itemName, LootUtils.GetProbability(enemy, LootBagRate.WHITE_BAG, eventChest), false, true)
                    )
            };
            whitebag[0].Populate(enemy, playerData, rnd, Lootstate, lootDefs);
        }
    }

    public static class LootUtils
    {
        public static double GetProbability(Enemy enemy, double probability, bool eventchest = false)
        {
            var players = enemy.DamageCounter.GetPlayerData().ToList().Count;
            var boost = probability * (eventchest ? .8 : 1);

            if (players == 1) boost *= 1.1;
            else if (players == 2) boost *= 1.075;
            else if (players == 3) boost *= 1.05;
            else if (players > 3 && players <= 8) boost *= 1.03;
            else if (players > 8 && players <= 12) boost *= 1.01;
            else boost *= 1;

            return boost;
        }
    }

    public class ItemLoot : ILootDef
    {
        private readonly ushort id;
        private readonly string item;
        private readonly double probability;
        private readonly bool isId;
        private readonly bool whiteBag;

        public string Lootstate { get; set; }
        public bool Shared { get; set; }
        public BagType BagType { get; set; }

        public ItemLoot(ushort id, double probability, bool shared = true, bool whiteBag = false)
        {
            this.id = id;
            this.probability = probability;
            isId = true;
            Shared = shared;
            this.whiteBag = whiteBag;
        }

        public ItemLoot(string item, double probability, bool shared = true, bool whiteBag = false)
        {
            this.item = item;
            this.probability = probability;
            isId = false;
            Shared = shared;
            this.whiteBag = whiteBag;
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

            var dat = GameServer.Manager.GameData;

            try
            { lootDefs.Add(new LootDef(dat.Items[isId ? id : dat.IdToObjectType[item]], probability, lootState, Shared, whiteBag)); }
            catch (KeyNotFoundException) { Log.Error($"Item '{item}', wasn't added in loot list of entity '{enemy.Name}', because doesn't contains in assets."); }
        }
    }

    public class LootState : ILootDef
    {
        private readonly ILootDef[] children;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

        public LootState(string subState, params ILootDef[] lootDefs)
        {
            children = lootDefs;
            Lootstate = subState;
        }

        public void Populate(Enemy enemy, Tuple<Player, int> playerDat, Random rand, string lootState, IList<LootDef> lootDefs)
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
        public BagType BagType { get; set; }

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

            var candidates = GameServer.Manager.GameData.Items
                .Where(item => item.Value.SlotType == 9000)
                .Where(item => item.Value.MinStars <= (int)rarity)
                .Select(item => item.Value)
                .ToArray();

            foreach (var i in candidates)
                lootDefs.Add(new LootDef(i, probability / candidates.Length, Lootstate, false));
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
        private readonly ItemType type;
        private readonly int[] types;

        public string Lootstate { get; set; }
        public bool Shared { get; set; }
        public BagType BagType { get; set; }

        public TierLoot(byte tier, ItemType type, bool shared = true, double probability = 0)
            : this(tier, type, BagType.Pink, shared, probability)
        {
        }

        public TierLoot(byte tier, ItemType type, double val, bool shared = true, double probability = 0)
            : this(tier, type, BagType.Pink, shared, probability)
        {
        }

        public TierLoot(byte tier, ItemType type, BagType bag, bool shared = true, double probability = 0)
        {
            this.tier = tier;
            this.type = type;
            this.probability = probability;

            Shared = shared;

            double bagProbability = 0;

            if (this.probability == 0)
            {
                switch (bag)
                {
                    case BagType.Pink:
                        bagProbability = 0.95;
                        break;

                    case BagType.Purple:
                        bagProbability = 0.9;
                        break;

                    case BagType.Cyan:
                        bagProbability = 0.85;
                        break;

                    case BagType.Blue:
                        bagProbability = 0.8;
                        break;

                    case BagType.White:
                        bagProbability = 0.75;
                        break;

                    case BagType.None:
                    default:
                        bagProbability = 1;
                        break;
                }
            }
            else
                bagProbability = this.probability;

            this.probability = bagProbability * Settings.GetEventRate();

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

            var candidates = GameServer.Manager.GameData.Items
                .Where(item => Array.IndexOf(types, item.Value.SlotType) != -1)
                .Where(item => item.Value.Tier == tier)
                .Select(item => item.Value)
                .ToArray();

            foreach (var i in candidates)
                lootDefs.Add(new LootDef(i, probability / candidates.Length, Lootstate, Shared));
        }
    }

    public class Threshold : ILootDef
    {
        private readonly ILootDef[] children;
        private readonly double threshold;

        public string Lootstate { get; set; }
        public BagType BagType { get; set; }

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

            if (playerDat == null)
                return;

            if (playerDat.Item2 / enemy.ObjectDesc.MaxHP >= threshold)
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
        public BagType BagType { get; set; }

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

            foreach (var i in children)
                i.Populate(enemy, null, rand, lootState, lootDefs);
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
        public BagType BagType { get; set; }

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
        public BagType BagType { get; set; }

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