#region

using BookSleeve;
using LoESoft.Core.config;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace LoESoft.Core
{
    #region RedisObject

    public abstract class RedisObject
    {
        private ConcurrentDictionary<string, KeyValuePair<byte[], bool>> fields;

        protected void Init(Database db, string key)
        {
            Key = key;
            Database = db;
            fields =
                new ConcurrentDictionary<string, KeyValuePair<byte[], bool>>(
                    db
                    .Hashes
                    .GetAll(0, key)
                    .Exec()
                    .ToDictionary(
                        x => x.Key,
                        x => new KeyValuePair<byte[], bool>(x.Value, false)
                    )
                );
        }

        public Database Database { get; private set; }
        public string Key { get; private set; }
        public IEnumerable<string> AllKeys => fields.Keys;
        public bool IsNull => fields.Count == 0;

        protected T GetValue<T>(string key, T def = default(T))
        {
            if (!fields.TryGetValue(key, out KeyValuePair<byte[], bool> val))
                return def;
            if (typeof(T) == typeof(int))
                return (T) (object) int.Parse(Encoding.UTF8.GetString(val.Key));
            else if (typeof(T) == typeof(ushort))
                return (T) (object) ushort.Parse(Encoding.UTF8.GetString(val.Key));
            else if (typeof(T) == typeof(bool))
                return (T) (object) (val.Key[0] != 0);
            else if (typeof(T) == typeof(DateTime))
                return (T) (object) DateTime.FromBinary(BitConverter.ToInt64(val.Key, 0));
            else if (typeof(T) == typeof(byte[]))
                return (T) (object) val.Key;
            else if (typeof(T) == typeof(ushort[]))
            {
                ushort[] ret = new ushort[val.Key.Length / 2];
                Buffer.BlockCopy(val.Key, 0, ret, 0, val.Key.Length);
                return (T) (object) ret;
            }
            else if (typeof(T) == typeof(int[]))
            {
                int[] ret = new int[val.Key.Length / 4];
                Buffer.BlockCopy(val.Key, 0, ret, 0, val.Key.Length);
                return (T) (object) ret;
            }
            else if (typeof(T) == typeof(string))
                return (T) (object) Encoding.UTF8.GetString(val.Key);
            else
                throw new NotSupportedException();
        }

        protected void SetValue<T>(string key, T val)
        {
            byte[] buff;
            if (typeof(T) == typeof(int) || typeof(T) == typeof(ushort) ||
                typeof(T) == typeof(string))
                buff = Encoding.UTF8.GetBytes(val.ToString());
            else if (typeof(T) == typeof(bool))
                buff = new byte[] { (byte) ((bool) (object) val ? 1 : 0) };
            else if (typeof(T) == typeof(DateTime))
                buff = BitConverter.GetBytes(((DateTime) (object) val).ToBinary());
            else if (typeof(T) == typeof(byte[]))
                buff = (byte[]) (object) val;
            else if (typeof(T) == typeof(ushort[]))
            {
                var v = (ushort[]) (object) val;
                buff = new byte[v.Length * 2];
                Buffer.BlockCopy(v, 0, buff, 0, buff.Length);
            }
            else if (typeof(T) == typeof(int[]))
            {
                var v = (int[]) (object) val;
                buff = new byte[v.Length * 4];
                Buffer.BlockCopy(v, 0, buff, 0, buff.Length);
            }
            else
                throw new NotSupportedException();
            fields[key] = new KeyValuePair<byte[], bool>(buff, true);
        }

        private Dictionary<string, byte[]> update;

        public void Flush()
        {
            if (update == null)
                update = new Dictionary<string, byte[]>();
            else
                update.Clear();

            foreach (var i in fields)
                if (i.Value.Value)
                    update.Add(i.Key, i.Value.Key);

            Database.Hashes.Set(0, Key, update);
        }

        public void Flush(RedisConnection conn = null)
        {
            if (update == null)
                update = new Dictionary<string, byte[]>();
            else
                update.Clear();
            foreach (var i in fields)
                if (i.Value.Value)
                    update.Add(i.Key, i.Value.Key);
            (conn ?? Database).Hashes.Set(0, Key, update);
        }

        public void Reload()    //Discard all updates
        {
            if (update != null)
                update.Clear();

            fields =
                new ConcurrentDictionary<string, KeyValuePair<byte[], bool>>(
                    Database
                    .Hashes
                    .GetAll(0, Key)
                    .Exec()
                    .ToDictionary(
                        x => x.Key,
                        x => new KeyValuePair<byte[], bool>(x.Value, false)
                    )
                );
        }
    }

    #endregion

    public class DbLoginInfo
    {
        private Database Db { get; set; }

        internal DbLoginInfo(Database db, string uuid)
        {
            Db = db;
            UUID = uuid;
            var json = db.Hashes.GetString(0, "logins", uuid.ToUpperInvariant()).Exec();
            if (json == null)
                IsNull = true;
            else
                JsonConvert.PopulateObject(json, this);
        }

        [JsonIgnore]
        public string UUID { get; private set; }

        [JsonIgnore]
        public bool IsNull { get; private set; }

        public string Salt { get; set; }
        public string HashedPassword { get; set; }
        public string AccountId { get; set; }

        public void Flush()
        {
            Db.Hashes.Set(0, "logins", UUID.ToUpperInvariant(), JsonConvert.SerializeObject(this));
        }
    }

    public class DbAccount : RedisObject
    {
        internal DbAccount(Database db, string accId)
        {
            AccountId = accId;
            Init(db, "account." + accId);
        }

        public DateTime AccountLifetime
        {
            get { return GetValue("accountLifetime", DateTime.MinValue); }
            set { SetValue("accountLifetime", value); }
        }

        public int AccountType
        {
            get { return GetValue("accountType", (int) config.AccountType.FREE_ACCOUNT); }
            set { SetValue("accountType", value); }
        }

        public string AccountId { get; private set; }

        internal string LockToken { get; set; }

        public string UUID
        {
            get { return GetValue<string>("uuid"); }
            set { SetValue("uuid", value); }
        }

        public string Name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }

        public bool Admin
        {
            get { return GetValue("admin", false); }
            set { SetValue("admin", value); }
        }

        public bool MapEditor
        {
            get { return GetValue("mapEditor", false); }
            set { SetValue("mapEditor", value); }
        }

        public bool NameChosen
        {
            get { return GetValue("nameChosen", false); }
            set { SetValue("nameChosen", value); }
        }

        public bool Verified
        {
            get { return GetValue("verified", Settings.STARTUP.VERIFIED); }
            set { SetValue("verified", value); }
        }

        public bool Converted
        {
            get { return GetValue("converted", false); }
            set { SetValue("converted", value); }
        }

        public string GuildId
        {
            get { return GetValue("guildId", "-1"); }
            set { SetValue("guildId", value); }
        }

        public int GuildRank
        {
            get { return GetValue<int>("guildRank"); }
            set { SetValue("guildRank", value); }
        }

        public int VaultCount
        {
            get { return GetValue<int>("vaultCount"); }
            set { SetValue("vaultCount", value); }
        }

        public int MaxCharSlot
        {
            get { return GetValue("maxCharSlot", Settings.STARTUP.MAX_CHAR_SLOTS); }
            set { SetValue("maxCharSlot", value); }
        }

        public DateTime RegTime
        {
            get { return GetValue<DateTime>("regTime"); }
            set { SetValue("regTime", value); }
        }

        public bool Guest
        {
            get { return GetValue("guest", false); }
            set { SetValue("guest", value); }
        }

        public int Credits
        {
            get { return GetValue("credits", Settings.STARTUP.GOLD); }
            set { SetValue("credits", value); }
        }

        public int Fame
        {
            get { return GetValue("fame", Settings.STARTUP.FAME); }
            set { SetValue("fame", value); }
        }

        public int TotalFame
        {
            get { return GetValue("totalFame", Settings.STARTUP.TOTAL_FAME); }
            set { SetValue("totalFame", value); }
        }

        public int GuildFame
        {
            get { return GetValue<int>("guildFame"); }
            set { SetValue("guildFame", value); }
        }

        public int FortuneTokens
        {
            get { return GetValue("fortuneTokens", Settings.STARTUP.TOKENS); }
            set { SetValue("fortuneTokens", value); }
        }

        public int EmpiresCoin
        {
            get { return GetValue("empiresCoin", Settings.STARTUP.EMPIRES_COIN); }
            set { SetValue("empiresCoin", value); }
        }

        public int NextCharId
        {
            get { return GetValue<int>("nextCharId"); }
            set { SetValue("nextCharId", value); }
        }

        public int[] Gifts
        {
            get { return GetValue<int[]>("gifts"); }
            set { SetValue("gifts", value); }
        }

        public int PetYardType
        {
            get { return GetValue("petYardType", 1); }
            set { SetValue("petYardType", value); }
        }

        public int IsAgeVerified
        {
            get { return GetValue("isAgeVerified", Settings.STARTUP.IS_AGE_VERIFIED); }
            set { SetValue("isAgeVerified", value); }
        }

        public int[] OwnedSkins
        {
            get { return GetValue<int[]>("ownedSkins"); }
            set { SetValue("ownedSkins", value); }
        }

        public int[] PurchasedPackages
        {
            get { return Utils.FromCommaSepString32(GetValue<string>("purchasedPackages")); }
            set { SetValue("purchasedPackages", value.ToCommaSepString()); }
        }

        public int[] PurchasedBoxes
        {
            get { return Utils.FromCommaSepString32(GetValue<string>("PurchasedBoxes")); }
            set { SetValue("PurchasedBoxes", value.ToCommaSepString()); }
        }

        public string[] Friends
        {
            get { return Utils.CommaToArray<string>(GetValue("friends", "1")); }
            set { SetValue("friends", value.ToCommaSepString()); }
        }

        public string[] FriendRequests
        {
            get { return Utils.CommaToArray<string>(GetValue("friendRequests", "1")); }
            set { SetValue("friendRequests", value.ToCommaSepString()); }
        }

        public string AuthToken
        {
            get { return GetValue<string>("authToken"); }
            set { SetValue("authToken", value); }
        }

        public bool Muted
        {
            get { return GetValue("muted", false); }
            set { SetValue("muted", value); }
        }

        public bool Banned
        {
            get { return GetValue("banned", false); }
            set { SetValue("banned", value); }
        }

        public int[] Locked
        {
            get { return GetValue<int[]>("locked"); }
            set { SetValue("locked", value); }
        }

        public int[] Ignored
        {
            get { return GetValue<int[]>("ignored"); }
            set { SetValue("ignored", value); }
        }
    }

    public struct DbClassAvailabilityEntry
    {
        public string Id { get; set; }
        public string Restricted { get; set; }
    }

    public struct DbClassStatsEntry
    {
        public int BestLevel { get; set; }
        public int BestFame { get; set; }
    }

    public class DbClassAvailability : RedisObject
    {
        public DbAccount Account { get; private set; }

        public DbClassAvailability(DbAccount acc)
        {
            Account = acc;
            Init(acc.Database, $"classAvailability.{acc.AccountId}");
        }

        public void Init(EmbeddedData data)
        {
            ObjectDesc field = null;
            foreach (var i in data.ObjectDescs.Where(_ => _.Value.Player || _.Value.Class == "Player"))
            {
                field = i.Value;
                SetValue(field.ObjectType.ToString(), JsonConvert.SerializeObject(new DbClassAvailabilityEntry()
                {
                    Id = field.ObjectId,
                    Restricted = field.ObjectType == 782 ? "unrestricted" : "restricted"
                }));
            }
        }

        public DbClassAvailabilityEntry this[ushort type]
        {
            get
            {
                string v = GetValue<string>(type.ToString());
                if (v != null)
                    return JsonConvert.DeserializeObject<DbClassAvailabilityEntry>(v);
                else
                    return default(DbClassAvailabilityEntry);
            }
            set
            {
                SetValue(type.ToString(), JsonConvert.SerializeObject(value));
            }
        }
    }

    public class DbClassStats : RedisObject
    {
        public DbAccount Account { get; private set; }

        public DbClassStats(DbAccount acc)
        {
            Account = acc;
            Init(acc.Database, "classStats." + acc.AccountId);
        }

        public void Update(DbChar character)
        {
            var field = character.ObjectType.ToString();
            string json = GetValue<string>(field);
            if (json == null)
                SetValue(field, JsonConvert.SerializeObject(new DbClassStatsEntry()
                {
                    BestLevel = character.Level,
                    BestFame = character.Fame
                }));
            else
            {
                var entry = JsonConvert.DeserializeObject<DbClassStatsEntry>(json);
                if (character.Level > entry.BestLevel)
                    entry.BestLevel = character.Level;
                if (character.Fame > entry.BestFame)
                    entry.BestFame = character.Fame;
                SetValue(field, JsonConvert.SerializeObject(entry));
            }
        }

        public DbClassStatsEntry this[ushort type]
        {
            get
            {
                string v = GetValue<string>(type.ToString());
                if (v != null)
                    return JsonConvert.DeserializeObject<DbClassStatsEntry>(v);
                else
                    return default(DbClassStatsEntry);
            }
            set
            {
                SetValue(type.ToString(), JsonConvert.SerializeObject(value));
            }
        }
    }

    public class DbChar : RedisObject
    {
        public DbAccount Account { get; private set; }
        public int CharId { get; private set; }

        internal DbChar(DbAccount acc, int charId)
        {
            Account = acc;
            CharId = charId;
            Init(acc.Database, "char." + acc.AccountId + "." + charId);
        }

        //public LootCache[] LootCaches
        //{
        //    get { return JsonConvert.DeserializeObject<LootCache[]>(GetValue<string>("lootCache")); }
        //    set { SetValue("lootCache", JsonConvert.SerializeObject(value)); }
        //}

        public ushort ObjectType
        {
            get { return GetValue<ushort>("charType", 782); }
            set { SetValue("charType", value); }
        }

        public int Level
        {
            get { return GetValue("level", 1); }
            set { SetValue("level", value); }
        }

        public int Experience
        {
            get { return GetValue("exp", 0); }
            set { SetValue("exp", value); }
        }

        public int Fame
        {
            get { return GetValue("fame", 0); }
            set { SetValue("fame", value); }
        }

        public int[] Items
        {
            get { return GetValue<int[]>("items"); }
            set { SetValue("items", value); }
        }

        public int[] Backpack
        {
            get { return GetValue<int[]>("backpack"); }
            set { SetValue("backpack", value); }
        }

        public int HP
        {
            get { return GetValue("hp", 100); }
            set { SetValue("hp", value); }
        }

        public int MP
        {
            get { return GetValue("mp", 100); }
            set { SetValue("mp", value); }
        }

        public int[] Stats
        {
            get { return GetValue<int[]>("stats"); }
            set { SetValue("stats", value); }
        }

        public int Tex1
        {
            get { return GetValue("tex1", 0); }
            set { SetValue("tex1", value); }
        }

        public int Tex2
        {
            get { return GetValue("tex2", 0); }
            set { SetValue("tex2", value); }
        }

        public int Skin
        {
            get { return GetValue("skin", -1); }
            set { SetValue("skin", value); }
        }

        public int Pet
        {
            get { return GetValue("pet", 0); }
            set { SetValue("pet", value); }
        }

        public byte[] FameStats
        {
            get { return GetValue("fameStats", new byte[] { }); }
            set { SetValue("fameStats", value); }
        }

        public string TaskStats
        {
            get { return GetValue("taskStats", string.Empty); }
            set { SetValue("taskStats", value); }
        }

        public DateTime CreateTime
        {
            get { return GetValue("createTime", DateTime.Now); }
            set { SetValue("createTime", value); }
        }

        public DateTime LastSeen
        {
            get { return GetValue("lastSeen", DateTime.Now); }
            set { SetValue("lastSeen", value); }
        }

        public bool Dead
        {
            get { return GetValue("dead", false); }
            set { SetValue("dead", value); }
        }

        public int HealthPotions
        {
            get { return GetValue("healthPotions", 1); }
            set { SetValue("healthPotions", value); }
        }

        public int MagicPotions
        {
            get { return GetValue("magicPotions", 0); }
            set { SetValue("magicPotions", value); }
        }

        public bool HasBackpack
        {
            get { return GetValue("hasBackpack", false); }
            set { SetValue("hasBackpack", value); }
        }

        public int LootDropTimer
        {
            get { return GetValue("lootDropTimer", 0); }
            set { SetValue("lootDropTimer", value); }
        }

        public int LootTierTimer
        {
            get { return GetValue("lootTierTimer", 0); }
            set { SetValue("lootTierTimer", value); }
        }

        public int XPBoostTimer
        {
            get { return GetValue("xpBoostTimer", 0); }
            set { SetValue("xpBoostTimer", value); }
        }

        public bool XPBoosted
        {
            get { return GetValue("xpBoosted", false); }
            set { SetValue("xpBoosted", value); }
        }
    }

    public class DbDeath : RedisObject
    {
        public DbAccount Account { get; private set; }
        public int CharId { get; private set; }

        public DbDeath(DbAccount acc, int charId)
        {
            Account = acc;
            CharId = charId;
            Init(acc.Database, "death." + acc.AccountId + "." + charId);
        }

        public ushort ObjectType
        {
            get { return GetValue<ushort>("objType"); }
            set { SetValue("objType", value); }
        }

        public int Level
        {
            get { return GetValue<int>("level"); }
            set { SetValue("level", value); }
        }

        public int TotalFame
        {
            get { return GetValue<int>("totalFame"); }
            set { SetValue("totalFame", value); }
        }

        public string Killer
        {
            get { return GetValue<string>("killer"); }
            set { SetValue("killer", value); }
        }

        public bool FirstBorn
        {
            get { return GetValue<bool>("firstBorn"); }
            set { SetValue("firstBorn", value); }
        }

        public DateTime DeathTime
        {
            get { return GetValue<DateTime>("deathTime"); }
            set { SetValue("deathTime", value); }
        }
    }

    public class DbGuild : RedisObject
    {
        public DbAccount AccountId { get; private set; }
        public int Id { get; private set; }

        internal DbGuild(Database db, int id)
        {
            Id = id;
            Init(db, "guild." + id);
        }

        public DbGuild(DbAccount acc)
        {
            Id = Convert.ToInt32(acc.GuildId);
            Init(acc.Database, "guild." + Id);
        }

        public string Name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }

        public int Level
        {
            get { return GetValue<int>("level"); }
            set { SetValue("level", value); }
        }

        public int Fame
        {
            get { return GetValue<int>("fame"); }
            set { SetValue("fame", value); }
        }

        public int TotalFame
        {
            get { return GetValue<int>("totalFame"); }
            set { SetValue("totalFame", value); }
        }

        public int[] Members
        {
            get { return GetValue<int[]>("members"); }
            set { SetValue("members", value); }
        }

        public string Board
        {
            get { return GetValue<string>("board") ?? ""; }
            set { SetValue("board", value); }
        }
    }

    public struct DbNewsEntry
    {
        [JsonIgnore]
        public DateTime Date { get; set; }

        public string Icon { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public class DbNews
    {
        public DbNews(Database db, int count)
        {
            News = db.SortedSets.Range(0, "news", 0, 10, false).Exec()
                .Select(x =>
                {
                    var ret = JsonConvert.DeserializeObject<DbNewsEntry>(
                        Encoding.UTF8.GetString(x.Key));
                    ret.Date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(x.Value);
                    return ret;
                }).ToArray();
        }

        private DbNewsEntry[] News { get; set; }
        public DbNewsEntry[] Entries => News;
    }

    public class DbVault : RedisObject
    {
        public DbAccount Account { get; private set; }

        public DbVault(DbAccount acc)
        {
            Account = acc;
            Init(acc.Database, "vault." + acc.AccountId);
        }

        public int[] this[int index]
        {
            get { return GetValue<int[]>("vault." + index); }
            set { SetValue("vault." + index, value); }
        }
    }

    public struct DbLegendEntry
    {
        public int TotalFame { get; set; }
        public int AccId { get; set; }
        public int ChrId { get; set; }
    }

    public enum DbLegendTimeSpan
    {
        All,
        Month,
        Week
    }

    public class DbLegend
    {
        public DbLegend(Database db, DbLegendTimeSpan timeSpan, int count)
        {
            double begin;
            if (timeSpan == DbLegendTimeSpan.Week)
                begin = DateTime.Now.Subtract(TimeSpan.FromDays(7)).ToUnixTimestamp();
            else if (timeSpan == DbLegendTimeSpan.Month)
                begin = DateTime.Now.AddMonths(-1).ToUnixTimestamp();
            else
                begin = 0;

            Entries = db.SortedSets.Range(0, "legends", begin, double.PositiveInfinity, false, count: count).Exec()
                .Select(x => new DbLegendEntry()
                {
                    TotalFame = BitConverter.ToInt32(x.Key, 0),
                    AccId = BitConverter.ToInt32(x.Key, 4),
                    ChrId = BitConverter.ToInt32(x.Key, 8)
                })
                .OrderByDescending(x => x.TotalFame)
                .ToArray();
        }

        public static void Insert(Database db, DateTime time, DbLegendEntry entry)
        {
            double t = time.ToUnixTimestamp();
            byte[] buff = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(entry.TotalFame), 0, buff, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(entry.AccId), 0, buff, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(entry.ChrId), 0, buff, 8, 4);
            db.SortedSets.Add(0, "legends", buff, t);
        }

        private DbLegendEntry[] Entries { get; set; }
        public DbLegendEntry[] GetEntries => Entries;
    }
}