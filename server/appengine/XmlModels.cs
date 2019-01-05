#region

using LoESoft.AppEngine.account;
using LoESoft.Core;
using LoESoft.Core.config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

#endregion

namespace LoESoft.AppEngine
{
    internal class NewsItem
    {
        public string Icon { get; internal set; }
        public string Title { get; internal set; }
        public string TagLine { get; internal set; }
        public string Link { get; internal set; }
        public DateTime Date { get; internal set; }

        public static NewsItem FromDb(DbNewsEntry entry)
        {
            return new NewsItem()
            {
                Icon = entry.Icon,
                Title = entry.Title,
                TagLine = entry.Text,
                Link = entry.Link,
                Date = entry.Date
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Item",
                    new XElement("Icon", Icon),
                    new XElement("Title", Title),
                    new XElement("TagLine", TagLine),
                    new XElement("Link", Link),
                    new XElement("Date", Date.ToUnixTimestamp())
                );
        }
    }

    internal class GuildMember
    {
        public string name;
        public int rank;
        public int guildFame;
#pragma warning disable CS0649 // Field 'GuildMember.lastSeen' is never assigned to, and will always have its default value 0
        public int lastSeen;
#pragma warning restore CS0649 // Field 'GuildMember.lastSeen' is never assigned to, and will always have its default value 0

        public static GuildMember FromDb(DbAccount acc)
        {
            return new GuildMember()
            {
                name = acc.Name,
                rank = acc.GuildRank,
                guildFame = acc.GuildFame
            };
        }

        public XElement ToXml()
        {
            return new XElement("Member",
                new XElement("Name", name),
                new XElement("Rank", rank),
                new XElement("Fame", guildFame));
        }
    }

    internal class Guild
    {
        public int id;
        public string name;
        public int currentFame;
        public int totalFame;
        public string hallType;
        public List<GuildMember> members;

        public static Guild FromDb(Database db, DbGuild guild)
        {
            var members = (from member in guild.Members
                           select db.GetAccountById(Convert.ToString(member)) into acc
                           where acc != null
                           orderby acc.GuildRank descending,
                                   acc.GuildFame descending,
                                   acc.Name ascending
                           select GuildMember.FromDb(acc)).ToList();

            return new Guild()
            {
                id = guild.Id,
                name = guild.Name,
                currentFame = guild.Fame,
                totalFame = guild.TotalFame,
                hallType = "Guild Hall " + guild.Level,
                members = members
            };
        }

        public XElement ToXml()
        {
            var guild = new XElement("Guild");
            guild.Add(new XAttribute("id", id));
            guild.Add(new XAttribute("name", name));
            guild.Add(new XElement("TotalFame", totalFame));
            guild.Add(new XElement("CurrentFame", currentFame));
            guild.Add(new XElement("HallType", hallType));
            foreach (var member in members)
                guild.Add(member.ToXml());

            return guild;
        }
    }

    internal class GuildIdentity
    {
        public int id;
        public string name;
        public int rank;

        public static GuildIdentity FromDb(DbAccount acc, DbGuild guild)
        {
            return new GuildIdentity()
            {
                id = guild.Id,
                name = guild.Name,
                rank = acc.GuildRank
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Guild",
                    new XAttribute("id", id),
                    new XElement("Name", name),
                    new XElement("Rank", rank)
                );
        }
    }

    internal class ClassStatsEntry
    {
        public ushort ObjectType { get; private set; }
        public int BestLevel { get; private set; }
        public int BestFame { get; private set; }

        public static ClassStatsEntry FromDb(ushort objType, DbClassStatsEntry entry)
        {
            return new ClassStatsEntry()
            {
                ObjectType = objType,
                BestLevel = entry.BestLevel,
                BestFame = entry.BestFame
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("ClassStats",
                    new XAttribute("objectType", ObjectType.To4Hex()),
                    new XElement("BestLevel", BestLevel),
                    new XElement("BestFame", BestFame)
                );
        }
    }

    internal class Stats
    {
        public int BestCharFame { get; private set; }
        public int TotalFame { get; private set; }
        public int Fame { get; private set; }

        private Dictionary<ushort, ClassStatsEntry> entries;

        public ClassStatsEntry this[ushort objType] => entries[objType];

        public static Stats FromDb(DbAccount acc, DbClassStats stats)
        {
            var ret = new Stats()
            {
                TotalFame = acc.TotalFame,
                Fame = acc.Fame,
                entries = new Dictionary<ushort, ClassStatsEntry>(),
                BestCharFame = 0
            };
            foreach (var i in stats.AllKeys)
            {
                var objType = ushort.Parse(i);
                var entry = ClassStatsEntry.FromDb(objType, stats[objType]);
                if (entry.BestFame > ret.BestCharFame)
                    ret.BestCharFame = entry.BestFame;
                ret.entries[objType] = entry;
            }
            return ret;
        }

        public XElement ToXml()
        {
            return
                new XElement("Stats",
                    entries.Values.Select(x => x.ToXml()),
                    new XElement("BestCharFame", BestCharFame),
                    new XElement("TotalFame", TotalFame),
                    new XElement("Fame", Fame)
                );
        }
    }

    internal class Vault
    {
        private int[][] chests { get; set; }

        public int[] this[int index] => chests[index];

        public static Vault FromDb(DbAccount acc, DbVault vault)
        {
            return new Vault()
            {
                chests = Enumerable.Range(1, acc.VaultCount).
                            Select(x => vault[x] ??
                                Enumerable.Repeat(-1, 8).ToArray()).ToArray()
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Vault",
                    chests.Select(x => new XElement("Chest", x.ToCommaSepString()))
                );
        }
    }

    internal class Account
    {
        public int EmpiresCoin { get; private set; }
        public string AccountId { get; private set; }
        public int AccountType { get; private set; }
        public DateTime AccountLifetime { get; private set; }
        public string Name { get; set; }

        public bool NameChosen { get; private set; }
        public bool Converted { get; private set; }
        public bool Admin { get; private set; }
        public bool MapEditor { get; private set; }
        public bool VerifiedEmail { get; private set; }

        public int Credits { get; private set; }
        public int NextCharSlotPrice { get; private set; }
        public uint BeginnerPackageTimeLeft { get; private set; }

        public int[] Gifts { get; private set; }
        public int PetYardType { get; private set; }
        public int FortuneTokens { get; private set; }
        public int IsAgeVerified { get; private set; }

        public Vault Vault { get; private set; }
        public Stats Stats { get; private set; }
        public Guild Guild { get; private set; }

        public static Account FromDb(DbAccount acc)
        {
            return new Account()
            {
                EmpiresCoin = acc.EmpiresCoin,
                AccountId = acc.AccountId,
                AccountType = acc.AccountType,
                AccountLifetime = acc.AccountLifetime,
                Name = acc.Name,
                NameChosen = acc.NameChosen,
                Converted = acc.Converted,
                Admin = acc.AccountType == (int)Core.config.AccountType.DEM_ACCOUNT,
                MapEditor = acc.AccountType == (int)Core.config.AccountType.CM_ACCOUNT,
                VerifiedEmail = acc.Verified,
                Credits = acc.Credits,
                NextCharSlotPrice = 100, // need adjusts
                BeginnerPackageTimeLeft = 604800,
                Gifts = acc.Gifts,
                PetYardType = acc.PetYardType,
                FortuneTokens = acc.FortuneTokens,
                IsAgeVerified = acc.IsAgeVerified,
                Vault = Vault.FromDb(acc, new DbVault(acc)),
                Stats = Stats.FromDb(acc, new DbClassStats(acc)),
                Guild = new Guild()
            };
        }

        public XElement ToXml() =>
            new XElement("Account",
                new XElement("EmpiresCoin", EmpiresCoin),
                new XElement("AccountId", AccountId),
                new XElement("AccountType", AccountType),
                new XElement("AccountLifetime", AccountLifetime),
                new XElement("Name", Name),
                NameChosen ? new XElement("NameChosen", null) : null,
                Converted ? new XElement("Converted", null) : null,
                Admin ? new XElement("Admin", null) : null,
                MapEditor ? new XElement("MapEditor", null) : null,
                VerifiedEmail ? new XElement("VerifiedEmail", null) : null,
                new XElement("Credits", Credits),
                new XElement("FortuneToken", FortuneTokens),
                new XElement("NextCharSlotPrice", NextCharSlotPrice),
                new XElement("BeginnerPackageTimeLeft", BeginnerPackageTimeLeft),
                new XElement("Originating", "None"),
                new XElement("cleanPasswordStatus", 1),
                new XElement("Gifts", Utils.ToCommaSepString(Gifts)),
                Stats.ToXml(),
                Guild.id != 0 ? Guild.ToXml() : null,
                Vault.ToXml(),
                new XElement("IsAgeVerified", IsAgeVerified),
                PetYardType != 0 ? new XElement("PetYardType", PetYardType) : null
            );
    }

    internal class FriendRequests
    {
        public IEnumerable<FriendListEntry> Entries { get; private set; }

        public static FriendRequests FromDb(Database db, DbAccount acc)
        {
            return new FriendRequests
            {
                Entries = acc.FriendRequests.Distinct().Select(_ => FriendListEntry.FromDb(db, db.GetAccountById(_)))
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Requests",
                    Entries.Select(_ => _.ToXml()));
        }
    }

    internal class FriendList
    {
        public IEnumerable<FriendListEntry> Entries { get; private set; }

        public static FriendList FromDb(Database db, DbAccount acc)
        {
            return new FriendList
            {
                Entries = acc.Friends.Distinct().Select(_ => FriendListEntry.FromDb(db, db.GetAccountById(_)))
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Friends",
                    Entries.Select(_ => _.ToXml()));
        }
    }

    internal class FriendListEntry
    {
        public string Name { get; set; }
        public Stats Stats { get; set; }
        public Character Character { get; set; }

        public static FriendListEntry FromDb(Database db, DbAccount acc)
        {
            return new FriendListEntry
            {
                Name = acc.Name,
                Stats = Stats.FromDb(acc, new DbClassStats(acc)),
                Character = Character.FromDb(db.GetAliveCharacter(acc), false)
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Account",
                    new XElement("Character",
                        new XElement("ObjectType", Character.ObjectType),
                        new XElement("Level", Character.Level),
                        new XElement("Exp", Character.Exp),
                        new XElement("CurrentFame", Character.CurrentFame),
                        new XElement("Texture", Character.Texture)),
                    new XElement("Name", Name),
                    new XElement("Stats",
                        Stats.ToXml()));
        }
    }

    internal class ClassAvailabilityList
    {
        private Dictionary<ushort, ClassAvailabilityEntry> entries;
        public ClassAvailabilityEntry this[ushort type] => entries[type];

        public static ClassAvailabilityList FromDb(DbAccount acc, DbClassAvailability ca)
        {
            var ret = new ClassAvailabilityList()
            {
                entries = new Dictionary<ushort, ClassAvailabilityEntry>()
            };
            foreach (var i in ca.AllKeys)
            {
                var type = ushort.Parse(i);
                var entry = ClassAvailabilityEntry.FromDb(ca[type]);
                ret.entries[type] = entry;
            }
            return ret;
        }

        public XElement ToXml()
        {
            return new XElement("ClassAvailabilityList",
                entries.Select(_ => _.Value.ToXml()));
        }
    }

    internal class ClassAvailabilityEntry
    {
        public string Id { get; private set; }
        public string Restricted { get; private set; }

        public static ClassAvailabilityEntry FromDb(DbClassAvailabilityEntry entry)
        {
            return new ClassAvailabilityEntry()
            {
                Id = entry.Id,
                Restricted = entry.Restricted
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("ClassAvailability",
                    new XAttribute("id", Id),
                    new XText(Restricted)
                );
        }
    }

    internal class Character
    {
        public int CharacterId { get; private set; }
        public ushort ObjectType { get; private set; }
        public int Level { get; private set; }
        public int Exp { get; private set; }
        public int CurrentFame { get; private set; }
        public int[] Equipment { get; private set; }
        public int MaxHitPoints { get; private set; }
        public int HitPoints { get; private set; }
        public int MaxMagicPoints { get; private set; }
        public int MagicPoints { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }
        public int Dexterity { get; private set; }
        public int HpRegen { get; private set; }
        public int MpRegen { get; private set; }
        public int Tex1 { get; private set; }
        public int Tex2 { get; private set; }
        public FameStats PCStats { get; private set; }
        public bool Dead { get; private set; }
        public int Pet { get; private set; }
        public int HpPotions { get; private set; }
        public int MpPotions { get; private set; }
        public int Texture { get; private set; }
        public bool XpBoosted { get; private set; }
        public int XpTimer { get; private set; }
        public int LDTimer { get; private set; }
        public int LTTimer { get; private set; }
        public bool HasBackpack { get; private set; }

        public static Character FromDb(DbChar character, bool dead)
        {
            return new Character()
            {
                CharacterId = character.CharId,
                ObjectType = character.ObjectType,
                Level = character.Level,
                Exp = character.Experience,
                CurrentFame = character.Fame,
                Equipment = character.HasBackpack ? character.Items.Concat(character.Backpack).ToArray() : character.Items,
                MaxHitPoints = character.Stats[0],
                MaxMagicPoints = character.Stats[1],
                Attack = character.Stats[2],
                Defense = character.Stats[3],
                Speed = character.Stats[4],
                Dexterity = character.Stats[5],
                HpRegen = character.Stats[6],
                MpRegen = character.Stats[7],
                HitPoints = character.HP,
                MagicPoints = character.MP,
                Tex1 = character.Tex1,
                Tex2 = character.Tex2,
                PCStats = FameStats.Read(character.FameStats),
                Dead = dead,
                Pet = character.Pet,
                HpPotions = character.HealthPotions,
                MpPotions = character.MagicPotions,
                Texture = character.Skin,
                XpBoosted = character.XPBoosted,
                XpTimer = character.XPBoostTimer,
                LDTimer = character.LootDropTimer,
                LTTimer = character.LootTierTimer,
                HasBackpack = character.HasBackpack
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("Char",
                    new XAttribute("id", CharacterId),
                    new XElement("ObjectType", ObjectType),
                    new XElement("Level", Level),
                    new XElement("Exp", Exp),
                    new XElement("CurrentFame", CurrentFame),
                    new XElement("Equipment", Equipment.ToCommaSepString()),
                    new XElement("MaxHitPoints", MaxHitPoints),
                    new XElement("HitPoints", HitPoints),
                    new XElement("MaxMagicPoints", MaxMagicPoints),
                    new XElement("MagicPoints", MagicPoints),
                    new XElement("Attack", Attack),
                    new XElement("Defense", Defense),
                    new XElement("Speed", Speed),
                    new XElement("Dexterity", Dexterity),
                    new XElement("HpRegen", HpRegen),
                    new XElement("MpRegen", MpRegen),
                    new XElement("PCStats", PCStats),
                    new XElement("HealthStackCount", HpPotions),
                    new XElement("MagicStackCount", MpPotions),
                    new XElement("Dead", Dead),
                    null,
                    Pet == 0 ? null : new XElement("NewPet", Pet),
                    Tex1 == 0 ? null : new XElement("Tex1", Tex1),
                    Tex2 == 0 ? null : new XElement("Tex2", Tex2),
                    new XElement("Texture", Texture),
                    new XElement("XpBoosted", XpBoosted ? 1 : 0),
                    new XElement("XpTimer", XpTimer),
                    new XElement("LDTimer", LDTimer),
                    new XElement("LTTimer", LTTimer),
                    new XElement("HasBackpack", HasBackpack ? 1 : 0)
                );
        }
    }

    internal class CharList
    {
        public Character[] Characters { get; private set; }
        public int NextCharId { get; private set; }
        public int MaxNumChars { get; private set; }

        public Account Account { get; private set; }

        public IEnumerable<NewsItem> News { get; private set; }
        public IEnumerable<Settings.APPENGINE.ServerItem> Servers { get; set; }
        public ClassAvailabilityList ClassAvailabilityList { get; private set; }

        public double? Lat { get; set; }
        public double? Long { get; set; }

        private static IEnumerable<NewsItem> GetItems(Database db, DbAccount acc)
        {
            var news = new DbNews(db, 10).Entries
                .Select(x => NewsItem.FromDb(x)).ToArray();
            var chars = db.GetDeadCharacters(acc).Take(10).Select(x =>
            {
                var death = new DbDeath(acc, x);
                return new NewsItem()
                {
                    Icon = "fame",
                    Title = $"Your {AppEngine.GameData.ObjectTypeToId[death.ObjectType]} died at level {death.Level}",
                    TagLine = $"You earned {death.TotalFame} glorious Fame",
                    Link = $"fame:{death.CharId}",
                    Date = death.DeathTime
                };
            });
            return news.Concat(chars).OrderByDescending(x => x.Date);
        }

        public static CharList FromDb(Database db, DbAccount acc)
        {
            return new CharList()
            {
                Characters = db.GetAliveCharacters(acc)
                                .Select(x => Character.FromDb(db.LoadCharacter(acc, x), false))
                                .ToArray(),
                NextCharId = acc.NextCharId,
                MaxNumChars = acc.MaxCharSlot,
                Account = Account.FromDb(acc),
                News = GetItems(db, acc),
                ClassAvailabilityList = ClassAvailabilityList.FromDb(acc, new DbClassAvailability(acc))
            };
        }

        public XElement ToXml(EmbeddedData data, DbAccount acc)
        {
            return
                new XElement("Chars",
                    new XAttribute("nextCharId", NextCharId),
                    new XAttribute("maxNumChars", MaxNumChars),
                    Characters.Select(x => x.ToXml()),
                    Account.ToXml(),
                    new XElement("News",
                        News.Select(x => x.ToXml())
                    ),
                    new XElement("Servers",
                        Servers.Select(x => x.ToXml())
                    ),
                    ClassAvailabilityList.ToXml(),
                    new XElement("OwnedSkins", Utils.GetCommaSepString(acc.OwnedSkins.ToArray())),
                    Lat == null ? null : new XElement("Lat", Lat),
                    Long == null ? null : new XElement("Long", Long),
                    MaxClassLevel.ToXml(data),
                    ItemCosts.ToXml()
                );
        }
    }

    internal class ItemCosts
    {
        public static XElement ToXml()
        {
            purchaseSkin x = new purchaseSkin();
            List<ItemCostItem> x2 = x.Prices.ToList();
            return
                new XElement("ItemCosts",
                    from _ in x2
                    select new XElement("ItemCost",
                               new XAttribute("type", _.Type),
                               new XAttribute("purchasable", _.Puchasable),
                               new XAttribute("expires", _.Expires),
                               new XText(_.Price.ToString())
                               )
                    );
        }
    }

    internal class MaxClassLevel
    {
        public static XElement ToXml(EmbeddedData data)
        {
            return
                new XElement("MaxClassLevelList",
                data.ObjectDescs.Values.Where(_ => _.Class == "Player").Select(_ =>
                    new XElement("MaxClassLevel",
                        new XAttribute("classType", _.ObjectType),
                        new XAttribute("maxLevel", 20))
               ));
        }
    }

    [Serializable]
    public class ItemCostItem
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("purchasable")]
        public int Puchasable { get; set; }

        [XmlAttribute("expires")]
        public int Expires { get; set; }

        [XmlText]
        public int Price { get; set; }
    }

    internal class Fame
    {
        public string Name { get; private set; }
        public Character Character { get; private set; }
        public FameStats Stats { get; private set; }
        public IEnumerable<Tuple<string, string, double>> Bonuses { get; private set; }
        public int TotalFame { get; private set; }

        public bool FirstBorn { get; private set; }
        public DateTime DeathTime { get; private set; }
        public string Killer { get; private set; }

        public static Fame FromDb(DbChar character)
        {
            DbDeath death = new DbDeath(character.Account, character.CharId);
            if (death.IsNull)
                return null;
            var stats = FameStats.Read(character.FameStats);
            return new Fame()
            {
                Name = character.Account.Name,
                Character = Character.FromDb(character, !death.IsNull),
                Stats = stats,
                Bonuses = stats.GetBonuses(AppEngine.GameData, character, death.FirstBorn),
                TotalFame = death.TotalFame,

                FirstBorn = death.FirstBorn,
                DeathTime = death.DeathTime,
                Killer = death.Killer
            };
        }

        private XElement GetCharElem()
        {
            var ret = Character.ToXml();
            ret.Add(new XElement("Account",
                new XElement("Name", Name)
            ));
            return ret;
        }

        public XElement ToXml()
        {
            return
                new XElement("Fame",
                    GetCharElem(),
                    new XElement("BaseFame", Character.CurrentFame),
                    new XElement("TotalFame", TotalFame),

                    new XElement("Shots", Stats.Shots),
                    new XElement("ShotsThatDamage", Stats.ShotsThatDamage),
                    new XElement("SpecialAbilityUses", Stats.SpecialAbilityUses),
                    new XElement("TilesUncovered", Stats.TilesUncovered),
                    new XElement("Teleports", Stats.Teleports),
                    new XElement("PotionsDrunk", Stats.PotionsDrunk),
                    new XElement("MonsterKills", Stats.MonsterKills),
                    new XElement("MonsterAssists", Stats.MonsterAssists),
                    new XElement("GodKills", Stats.GodKills),
                    new XElement("GodAssists", Stats.GodAssists),
                    new XElement("CubeKills", Stats.CubeKills),
                    new XElement("OryxKills", Stats.OryxKills),
                    new XElement("QuestsCompleted", Stats.QuestsCompleted),
                    new XElement("PirateCavesCompleted", Stats.PirateCavesCompleted),
                    new XElement("UndeadLairsCompleted", Stats.UndeadLairsCompleted),
                    new XElement("AbyssOfDemonsCompleted", Stats.AbyssOfDemonsCompleted),
                    new XElement("SnakePitsCompleted", Stats.SnakePitsCompleted),
                    new XElement("SpiderDensCompleted", Stats.SpiderDensCompleted),
                    new XElement("SpriteWorldsCompleted", Stats.SpriteWorldsCompleted),
                    new XElement("LevelUpAssists", Stats.LevelUpAssists),
                    new XElement("MinutesActive", Stats.MinutesActive),
                    new XElement("TombsCompleted", Stats.TombsCompleted),
                    new XElement("TrenchesCompleted", Stats.TrenchesCompleted),
                    new XElement("JunglesCompleted", Stats.JunglesCompleted),
                    new XElement("ManorsCompleted", Stats.ManorsCompleted),
                    Bonuses.Select(x =>
                        new XElement("Bonus",
                            new XAttribute("id", x.Item1),
                            new XAttribute("desc", x.Item2),
                            x.Item3
                        )
                    ),
                    new XElement("CreatedOn", DeathTime.ToUnixTimestamp()),
                    new XElement("KilledBy", Killer)
                );
        }
    }

    internal class FameListEntry
    {
        public string AccountId { get; private set; }
        public int CharId { get; private set; }
        public string Name { get; private set; }
        public ushort ObjectType { get; private set; }
        public int Tex1 { get; private set; }
        public int Tex2 { get; private set; }
        public int[] Equipment { get; private set; }
        public int TotalFame { get; private set; }

        public static FameListEntry FromDb(DbChar character)
        {
            var death = new DbDeath(character.Account, character.CharId);
            return new FameListEntry()
            {
                AccountId = character.Account.AccountId,
                CharId = character.CharId,
                Name = character.Account.Name,
                ObjectType = character.ObjectType,
                Tex1 = character.Tex1,
                Tex2 = character.Tex2,
                Equipment = character.Items,
                TotalFame = death.TotalFame
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("FameListElem",
                    new XAttribute("accountId", AccountId),
                    new XAttribute("charId", CharId),
                    new XElement("Name", Name),
                    new XElement("ObjectType", ObjectType),
                    new XElement("Tex1", Tex1),
                    new XElement("Tex2", Tex2),
                    new XElement("Equipment", Equipment.ToCommaSepString()),
                    new XElement("TotalFame", TotalFame)
                );
        }
    }

    internal class FameList
    {
        public string TimeSpan { get; private set; }
        public IEnumerable<FameListEntry> Entries { get; private set; }

        public static FameList FromDb(Database db, string timeSpan, DbChar character)
        {
            DbLegendTimeSpan span;
            if (timeSpan.EqualsIgnoreCase("week"))
                span = DbLegendTimeSpan.Week;
            else if (timeSpan.EqualsIgnoreCase("month"))
                span = DbLegendTimeSpan.Month;
            else if (timeSpan.EqualsIgnoreCase("all"))
                span = DbLegendTimeSpan.All;
            else
                return null;

            DbLegend legend = new DbLegend(db, span, 20);
            IEnumerable<DbChar> chars;
            if (character == null)
                chars = legend.GetEntries.Select(x =>
                    db.LoadCharacter(x.AccId, x.ChrId));
            else
                chars = legend.GetEntries.Select(x =>
                    db.LoadCharacter(x.AccId, x.ChrId)
                ).Concat(new[] { character }).Take(20);

            return new FameList()
            {
                TimeSpan = timeSpan.ToLower(),
                Entries = chars
                    .Select(x => FameListEntry.FromDb(x))
                    .OrderByDescending(x => x.TotalFame)
            };
        }

        public XElement ToXml()
        {
            return
                new XElement("FameList",
                    new XAttribute("timespan", TimeSpan),
                    Entries.Select(x => x.ToXml())
                );
        }
    }
}