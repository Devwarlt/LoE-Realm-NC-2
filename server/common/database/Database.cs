#region

using BookSleeve;
using LoESoft.Core.config;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace LoESoft.Core
{
    public class Database : RedisConnection
    {
        public static ILog log = LogManager.GetLogger(nameof(Database));

        public Database()
            : base(
                  host: Settings.REDIS_DATABASE.HOST,
                  port: Settings.REDIS_DATABASE.PORT,
                  ioTimeout: Settings.REDIS_DATABASE.IO_TIMEOUT,
                  password: Settings.REDIS_DATABASE.PASSWORD == "" ? null : Settings.REDIS_DATABASE.PASSWORD,
                  maxUnsent: Settings.REDIS_DATABASE.MAX_UNSENT,
                  allowAdmin: Settings.REDIS_DATABASE.ALLOW_ADMIN,
                  syncTimeout: Settings.REDIS_DATABASE.SYNC_TIMEOUT
                  )
        {
            SetKeepAlive(Settings.RESTART_DELAY_MINUTES * 60 * 2);
            Open().Wait();
        }

        public static readonly List<string> BlackListedNames = new List<string>
        {
            "NPC"
        };

        public static readonly string[] Names =
        {
            "Darq", "Deyst", "Drac", "Drol",
            "Eango", "Eashy", "Eati", "Eendi", "Ehoni",
            "Gharr", "Iatho", "Iawa", "Idrae", "Iri", "Issz", "Itani",
            "Laen", "Lauk", "Lorz",
            "Oalei", "Odaru", "Oeti", "Orothi", "Oshyu",
            "Queq", "Radph", "Rayr", "Ril", "Rilr", "Risrr",
            "Saylt", "Scheev", "Sek", "Serl", "Seus",
            "Tal", "Tiar",
            "Uoro", "Urake", "Utanu",
            "Vorck", "Vorv",
            "Yangu", "Yimi",
            "Zhiar"
        };

        public DbAccount CreateGuestAccount(string uuid)
        {
            return new DbAccount(this, "0")
            {
                AccountType = (int) AccountType.FREE_ACCOUNT,
                AccountLifetime = DateTime.MinValue,
                UUID = uuid,
                Name = Names[(uint) uuid.GetHashCode() % Names.Length],
                Admin = false,
                NameChosen = false,
                Verified = false,
                Converted = false,
                GuildId = "-1",
                GuildRank = -1,
                GuildFame = -1,
                VaultCount = 1,
                MaxCharSlot = Settings.STARTUP.MAX_CHAR_SLOTS,
                RegTime = DateTime.Now,
                Guest = true,
                Fame = Settings.STARTUP.FAME,
                TotalFame = Settings.STARTUP.TOTAL_FAME,
                Credits = Settings.STARTUP.GOLD,
                EmpiresCoin = Settings.STARTUP.EMPIRES_COIN,
                FortuneTokens = Settings.STARTUP.TOKENS,
                Gifts = new int[] { },
                PetYardType = 1,
                IsAgeVerified = 0,
                OwnedSkins = new int[] { },
                PurchasedPackages = new int[] { },
                PurchasedBoxes = new int[] { }
            };
        }

        public LoginStatus Verify(string uuid, string password, out DbAccount acc)
        {
            acc = null;
            var info = new DbLoginInfo(this, uuid);
            if (info.IsNull)
                return LoginStatus.AccountNotExists;

            var userPass = Utils.SHA1(password + info.Salt);
            if (Convert.ToBase64String(userPass) != info.HashedPassword)
                return LoginStatus.InvalidCredentials;

            acc = new DbAccount(this, info.AccountId);

            return LoginStatus.OK;
        }

        public bool AcquireLock(DbAccount acc)
        {
            string lockToken = Guid.NewGuid().ToString();
            string key = "lock." + acc.AccountId;
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyNotExists(1, key));

                trans.Strings.Set(1, key, lockToken);
                trans.Keys.Expire(1, key, 60);

                bool ok = trans.Execute().Exec();
                acc.LockToken = ok ? lockToken : null;
                return ok;
            }
        }

        public int GetLockTime(DbAccount acc) => (int) Keys.TimeToLive(1, $"lock.{acc.AccountId}").Exec();

        public int GetLockTime(string accId) => (int) Keys.TimeToLive(1, $"lock.{accId}").Exec();

        public bool RenewLock(DbAccount acc, int sec = 60)
        {
            string key = $"lock.{acc.AccountId}";
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyEquals(1, key, acc.LockToken));
                Keys.Expire(1, key, sec);
                return trans.Execute().Exec();
            }
        }

        public void ReleaseLock(DbAccount acc)
        {
            string key = $"lock.{acc.AccountId}";
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyEquals(1, key, acc.LockToken));
                trans.Keys.Remove(1, key);
                trans.Execute().Exec();
            }
        }

        public IDisposable Lock(DbAccount acc) => new l(this, acc);

        public bool LockOk(IDisposable l) => ((l) l).lockOk;

        private struct l : IDisposable
        {
            private Database db;
            private DbAccount acc;
            internal bool lockOk;

            public l(Database db, DbAccount acc)
            {
                this.db = db;
                this.acc = acc;
                lockOk = db.AcquireLock(acc);
            }

            public void Dispose()
            {
                if (lockOk)
                    db.ReleaseLock(acc);
            }
        }

        public const string REG_LOCK = "regLock";
        public const string NAME_LOCK = "nameLock";

        public string AcquireLock(string key)
        {
            string lockToken = Guid.NewGuid().ToString();
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyNotExists(1, key));

                trans.Strings.Set(1, key, lockToken);
                trans.Keys.Expire(1, key, 60);

                return trans.Execute().Exec() ? lockToken : null;
            }
        }

        public void ReleaseLock(string key, string token)
        {
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyEquals(1, key, token));
                trans.Keys.Remove(1, key);
                trans.Execute();
            }
        }

        public bool RenameUUID(DbAccount acc, string newUuid, string lockToken)
        {
            string p = Hashes.GetString(0, "login", acc.UUID.ToUpperInvariant()).Exec();
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyEquals(1, REG_LOCK, lockToken));
                trans.Hashes.Remove(0, "login", acc.UUID.ToUpperInvariant());
                trans.Hashes.Set(0, "login", newUuid.ToUpperInvariant(), p);
                if (!trans.Execute().Exec())
                    return false;
            }
            acc.UUID = newUuid;
            acc.Flush();
            return true;
        }

        public bool RenameIGN(DbAccount acc, string newName, string lockToken)
        {
            if (Names.Contains(newName, StringComparer.InvariantCultureIgnoreCase))
                return false;
            using (var trans = CreateTransaction())
            {
                trans.AddCondition(Condition.KeyEquals(1, NAME_LOCK, lockToken));
                Hashes.Remove(0, "names", acc.Name.ToUpperInvariant());
                Hashes.Set(0, "names", newName.ToUpperInvariant(), acc.AccountId.ToString());
                if (!trans.Execute().Exec())
                    return false;
            }
            acc.Name = newName;
            acc.NameChosen = true;
            acc.Flush();
            return true;
        }

        private static RandomNumberGenerator gen = RandomNumberGenerator.Create();

        public void ChangePassword(string uuid, string password)
        {
            var login = new DbLoginInfo(this, uuid);

            var x = new byte[0x10];
            gen.GetNonZeroBytes(x);
            string salt = Convert.ToBase64String(x);
            string hash = Convert.ToBase64String(Utils.SHA1(password + salt));

            login.HashedPassword = hash;
            login.Salt = salt;
            login.Flush();
        }

        public RegisterStatus Register(string uuid, string password, bool isGuest, out DbAccount acc)
        {
            acc = null;
            if (!Hashes.SetIfNotExists(0, "logins", uuid.ToUpperInvariant(), "{}").Exec())
                return RegisterStatus.UsedName;

            int newAccId = (int) Strings.Increment(0, "nextAccId").Exec();

            acc = new DbAccount(this, newAccId.ToString())
            {
                AccountType = (int) AccountType.FREE_ACCOUNT,
                AccountLifetime = DateTime.MinValue,
                UUID = uuid,
                Name = Names[(uint) uuid.GetHashCode() % Names.Length],
                Admin = false,
                NameChosen = false,
                Verified = Settings.STARTUP.VERIFIED,
                Converted = false,
                GuildId = "-1",
                GuildRank = 0,
                GuildFame = 0,
                VaultCount = 1,
                MaxCharSlot = Settings.STARTUP.MAX_CHAR_SLOTS,
                RegTime = DateTime.Now,
                Guest = isGuest,
                Fame = Settings.STARTUP.FAME,
                TotalFame = Settings.STARTUP.TOTAL_FAME,
                Credits = Settings.STARTUP.GOLD,
                EmpiresCoin = Settings.STARTUP.EMPIRES_COIN,
                FortuneTokens = Settings.STARTUP.TOKENS,
                Gifts = new int[] { },
                PetYardType = 1,
                IsAgeVerified = Settings.STARTUP.IS_AGE_VERIFIED,
                OwnedSkins = new int[] { },
                PurchasedPackages = new int[] { },
                PurchasedBoxes = new int[] { },
                AuthToken = GenerateRandomString(128),
                Muted = false,
                Banned = false,
                Locked = new int[] { 0 },
                Ignored = new int[] { 0 }
            };
            acc.Flush();

            var login = new DbLoginInfo(this, uuid);

            var x = new byte[0x10];
            gen.GetNonZeroBytes(x);
            string salt = Convert.ToBase64String(x);
            string hash = Convert.ToBase64String(Utils.SHA1(password + salt));

            login.HashedPassword = hash;
            login.Salt = salt;
            login.AccountId = acc.AccountId;
            login.Flush();

            var stats = new DbClassStats(acc);
            stats.Flush();

            var vault = new DbVault(acc);
            vault[0] = Enumerable.Repeat(-1, 8).ToArray();
            vault.Flush();

            return RegisterStatus.OK;
        }

        public bool HasUUID(string uuid) => Hashes.Exists(0, "login", uuid.ToUpperInvariant()).Exec();

        public DbAccount GetAccountById(string id)
        {
            var ret = new DbAccount(this, id);
            if (ret.IsNull)
                return null;
            return ret;
        }

        public DbAccount GetAccountByUUID(string uuid)
        {
            var info = new DbLoginInfo(this, uuid);
            if (info.IsNull)
                return null;
            var ret = new DbAccount(this, info.AccountId);
            if (ret.IsNull)
                return null;
            return ret;
        }

        public string ResolveId(string name) => Hashes.GetString(0, "names", name.ToUpperInvariant()).Exec() == null ? "0" : Hashes.GetString(0, "names", name.ToUpperInvariant()).Exec();

        public string ResolveIgn(string accId) => Hashes.GetString(0, $"account.{accId}", "name").Exec();

        public string ResolveIgn(DbAccount acc) => Hashes.GetString(0, $"account.{acc.AccountId}", "name").Exec();

        public void AddSkin(DbAccount acc, int skin)
        {
            List<int> skinList = acc.OwnedSkins.ToList();
            skinList.Add(skin);
            int[] result = skinList.ToArray();
            acc.OwnedSkins = result;
            Update(acc);
        }

        public bool CheckMysteryBox(DbAccount acc, int box, int total)
            => acc.PurchasedBoxes.Where(mbox => mbox == box).ToList().Count >= total;

        public bool CheckPackage(DbAccount acc, int package, int total)
            => acc.PurchasedPackages.Where(pack => pack == package).ToList().Count >= total;

        public void AddMysteryBox(DbAccount acc, int box)
        {
            List<int> boxList = acc.PurchasedBoxes.ToList();
            boxList.Add(box);
            int[] result = boxList.ToArray();
            acc.PurchasedBoxes = result;
            Hashes.Set(0, acc.Key, "PurchasedBoxes", Utils.GetCommaSepString(acc.PurchasedBoxes));
            Update(acc);
        }

        public void AddPackage(DbAccount acc, int package)
        {
            List<int> packageList = acc.PurchasedPackages.ToList();
            packageList.Add(package);
            int[] result = packageList.ToArray();
            acc.PurchasedPackages = result;
            Hashes.Set(0, acc.Key, "purchasedPackages", Utils.GetCommaSepString(acc.PurchasedPackages));
            Update(acc);
        }

        public void UpdateCredit(DbAccount acc, int amount)
        {
            if (amount > 0)
                WaitAll(Hashes.Increment(0, acc.Key, "credits", amount));
            else
                Hashes.Increment(0, acc.Key, "credits", amount).Wait();
            Update(acc);
        }

        public void UpdateFame(DbAccount acc, int amount)
        {
            if (amount > 0)
                WaitAll(
                    Hashes.Increment(0, acc.Key, "totalFame", amount),
                    Hashes.Increment(0, acc.Key, "fame", amount));
            else
                Hashes.Increment(0, acc.Key, "fame", amount).Wait();
            Update(acc);
        }

        public void UpdateTokens(DbAccount acc, int amount)
        {
            if (amount > 0)
                WaitAll(Hashes.Increment(0, acc.Key, "fortuneTokens", amount));
            else
                Hashes.Increment(0, acc.Key, "fortuneTokens", amount).Wait();
            Update(acc);
        }

        public void UpdateAccountLifetime(DbAccount acc, AccountType accType, int amount)
        {
            acc.AccountLifetime = DateTime.Now;
            acc.AccountLifetime = acc.AccountLifetime.AddDays(amount);
            acc.AccountType = (int) accType;
            Update(acc);
        }

        public void Update(DbAccount acc)
        {
            acc.Flush();
            acc.Reload();
        }

        public DbClassStats ReadClassStats(DbAccount acc) => new DbClassStats(acc);

        public DbVault ReadVault(DbAccount acc) => new DbVault(acc);

        public int CreateChest(DbVault vault)
        {
            int id = (int) Hashes.Increment(0, vault.Account.Key, "vaultCount").Exec();
            int newid = id - 1; //since index of vaults is zero it'll be reduced by 1 then incremented to index again
            vault[newid] = Enumerable.Repeat(-1, 8).ToArray();
            vault.Flush();
            return newid;
        }

        public DbChar GetAliveCharacter(DbAccount acc)
        {
            int chara = 1;
            foreach (var i in Sets.GetAll(0, "alive." + acc.AccountId).Exec().Reverse())
                chara = BitConverter.ToInt32(i, 0);
            return LoadCharacter(acc, chara);
        }

        public IEnumerable<int> GetAliveCharacters(DbAccount acc)
        {
            foreach (var i in Sets.GetAll(0, "alive." + acc.AccountId).Exec())
                yield return BitConverter.ToInt32(i, 0);
        }

        public IEnumerable<int> GetDeadCharacters(DbAccount acc)
        {
            foreach (var i in Lists.Range(0, "dead." + acc.AccountId, 0, int.MaxValue).Exec())
                yield return BitConverter.ToInt32(i, 0);
        }

        public bool IsAlive(DbChar character) => Sets.Contains(0, $"alive.{character.Account.AccountId}", BitConverter.GetBytes(character.CharId)).Exec();

        public CreateStatus CreateCharacter(EmbeddedData dat, DbAccount acc, ushort type, int skin, out DbChar character)
        {
            var @class = dat.ObjectTypeToElement[type];

            if (Sets.GetLength(0, "alive." + acc.AccountId).Exec() >= acc.MaxCharSlot)
            {
                character = null;
                return CreateStatus.ReachCharLimit;
            }

            int newId = (int) Hashes.Increment(0, acc.Key, "nextCharId").Exec();
            character = new DbChar(acc, newId)
            {
                //LootCaches = new LootCache[] { },
                ObjectType = type,
                Level = 1,
                Experience = 0,
                Fame = 0,
                HasBackpack = false,
                Items = @class.Element("Equipment").Value.Replace("0xa22", "-1").CommaToArray<int>(),
                Stats = new int[]{
                    int.Parse(@class.Element("MaxHitPoints").Value),
                    int.Parse(@class.Element("MaxMagicPoints").Value),
                    int.Parse(@class.Element("Attack").Value),
                    int.Parse(@class.Element("Defense").Value),
                    int.Parse(@class.Element("Speed").Value),
                    int.Parse(@class.Element("Dexterity").Value),
                    int.Parse(@class.Element("HpRegen").Value),
                    int.Parse(@class.Element("MpRegen").Value),
                },
                HP = int.Parse(@class.Element("MaxHitPoints").Value),
                MP = int.Parse(@class.Element("MaxMagicPoints").Value),
                Tex1 = 0,
                Tex2 = 0,
                Skin = skin,
                Pet = 0,
                FameStats = new byte[0],
                TaskStats = string.Empty,
                CreateTime = DateTime.Now,
                LastSeen = DateTime.Now
            };
            character.Flush();
            Sets.Add(0, "alive." + acc.AccountId, BitConverter.GetBytes(newId));
            return CreateStatus.OK;
        }

        public DbChar LoadCharacter(DbAccount acc, int charId)
        {
            var ret = new DbChar(acc, charId);

            if (ret.IsNull)
                return null;

            return ret;
        }

        public DbChar LoadCharacter(int accId, int charId)
        {
            var acc = new DbAccount(this, accId.ToString());
            if (acc.IsNull)
                return null;
            var ret = new DbChar(acc, charId);
            if (ret.IsNull)
                return null;
            return ret;
        }

        public bool SaveCharacter(DbAccount acc, DbChar character, bool lockAcc)
        {
            using (var trans = CreateTransaction())
            {
                if (lockAcc)
                    trans.AddCondition(Condition.KeyEquals(1,
                        $"lock.{acc.AccountId}", acc.LockToken));
                character.Flush(trans);
                var stats = new DbClassStats(acc);
                stats.Update(character);
                stats.Flush(trans);
                return trans.Execute().Exec();
            }
        }

        public void DeleteCharacter(DbAccount acc, int charId)
        {
            Keys.Remove(0, $"char.{acc.AccountId}.{charId}");
            var buff = BitConverter.GetBytes(charId);
            Sets.Remove(0, $"alive.{acc.AccountId}", buff);
            Lists.Remove(0, $"dead.{acc.AccountId}", buff);
        }

        public void Death(EmbeddedData dat, DbAccount acc, DbChar character, FameStats stats, string killer)
        {
            character.Dead = true;
            SaveCharacter(acc, character, acc.LockToken != null);
            var finalFame = stats.CalculateTotalFame(dat, new DbClassStats(acc), character, character.Fame, out bool firstBorn);
            var death = new DbDeath(acc, character.CharId)
            {
                ObjectType = character.ObjectType,
                Level = character.Level,
                TotalFame = finalFame,
                Killer = killer,
                FirstBorn = firstBorn,
                DeathTime = DateTime.Now
            };
            death.Flush();

            var idBuff = BitConverter.GetBytes(character.CharId);
            Sets.Remove(0, $"alive.{acc.AccountId}", idBuff);
            Lists.AddFirst(0, $"dead.{acc.AccountId}", idBuff);

            UpdateFame(acc, finalFame);

            var entry = new DbLegendEntry()
            {
                AccId = int.Parse(acc.AccountId),
                ChrId = character.CharId,
                TotalFame = finalFame
            };
            DbLegend.Insert(this, death.DeathTime, entry);
        }

        public void VerifyAge(DbAccount acc)
        {
            Hashes.Set(0, acc.Key, "isAgeVerified", "1");
            Update(acc);
        }

        public void ChangeClassAvailability(DbAccount acc, EmbeddedData data, ushort type)
        {
            int price;
            if (acc.Credits < (price = data.ObjectDescs[type].UnlockCost))
                return;

            Hashes.Set(0, $"classAvailability.{acc.AccountId}", type.ToString(),
                JsonConvert.SerializeObject(new DbClassAvailabilityEntry()
                {
                    Id = data.ObjectTypeToId[type],
                    Restricted = "unrestricted"
                }));
            UpdateCredit(acc, -price);
            Update(acc);
        }

        public static string GenerateRandomString(int size, Random rand = null)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = rand ?? new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = chars[random.Next(0, chars.Length - 1)];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public void MuteAccount(DbAccount acc)
        {
            Hashes.Set(0, acc.Key, "muted", "1");
            Update(acc);
        }

        public void UnmuteAccount(DbAccount acc)
        {
            Hashes.Set(0, acc.Key, "muted", "0");
            Update(acc);
        }

        public void BanAccount(DbAccount acc)
        {
            Hashes.Set(0, acc.Key, "banned", "1");
            Update(acc);
        }

        public List<string> GetLockeds(DbAccount acc)
        {
            try
            {
                List<int> x = acc.Locked.ToList();
                string x2 = string.Join(",", x);
                List<string> x3 = x2.Split(',').ToList();
                return x3;
            }
            catch
            {
                return new List<string>();
            }
        }

        public List<string> GetIgnoreds(DbAccount acc)
        {
            try
            {
                List<int> x = acc.Ignored.ToList();
                string x2 = string.Join(",", x);
                List<string> x3 = x2.Split(',').ToList();
                return x3;
            }
            catch
            {
                return new List<string>();
            }
        }

        public void LockAccount(DbAccount acc, int lockId)
        {
            try
            {
                List<int> x = acc.Locked.ToList();
                if (!x.Contains(lockId))
                {
                    x.Add(lockId);
                    int[] result = x.ToArray();
                    acc.Locked = result;
                    Update(acc);
                }
                else
                {
                    x.Remove(lockId);
                    int[] result = x.ToArray();
                    acc.Locked = result;
                    Update(acc);
                }
            }
            catch
            {
                List<int> x = new List<int>();
                x.Add(lockId);
                int[] result = x.ToArray();
                acc.Locked = result;
                Update(acc);
            }
        }

        public void IgnoreAccount(DbAccount acc, int lockId)
        {
            try
            {
                List<int> x = acc.Ignored.ToList();
                if (!x.Contains(lockId))
                {
                    x.Add(lockId);
                    int[] result = x.ToArray();
                    acc.Ignored = result;
                    Update(acc);
                }
                else
                {
                    x.Remove(lockId);
                    int[] result = x.ToArray();
                    acc.Ignored = result;
                    Update(acc);
                }
            }
            catch
            {
                List<int> x = new List<int>();
                x.Add(lockId);
                int[] result = x.ToArray();
                acc.Ignored = result;
                Update(acc);
            }
        }

        public GuildCreateStatus CreateGuild(string guildName, out DbGuild guild)
        {
            guild = null;

            if (String.IsNullOrWhiteSpace(guildName))
                return GuildCreateStatus.InvalidName;

            guildName = guildName.Trim();

            int newGuildId = (int) Strings.Increment(0, "newGuildId").Exec();
            if (!Hashes.SetIfNotExists(0, "guilds", guildName.ToUpperInvariant(), Convert.ToString(newGuildId)).Exec())
                return GuildCreateStatus.UsedName;

            guild = new DbGuild(this, newGuildId)
            {
                Name = guildName,
                Level = 0,
                Fame = 0,
                TotalFame = 0
            };

            guild.Flush();

            return GuildCreateStatus.OK;
        }

        public DbGuild GetGuild(string guildId)
        {
            int id = Convert.ToInt32(guildId);

            DbGuild ret = new DbGuild(this, id);

            if (ret.IsNull)
                return null;

            return ret;
        }

        public AddGuildMemberStatus AddGuildMember(DbGuild guild, DbAccount acc, bool founder = false)
        {
            if (acc == null)
                return AddGuildMemberStatus.Error;

            if (acc.NameChosen == false)
                return AddGuildMemberStatus.NameNotChosen;

            if (Convert.ToInt32(acc.GuildId) == guild.Id)
                return AddGuildMemberStatus.AlreadyInGuild;

            if (Convert.ToInt32(acc.GuildId) > 0)
                return AddGuildMemberStatus.InAnotherGuild;

            int guildSize = 100;

            if (guild.Members?.Length >= guildSize)
                return AddGuildMemberStatus.GuildFull;

            if (guild.Members == null)
            {
                List<int> gfounder = new List<int>();

                gfounder.Add(Convert.ToInt32(acc.AccountId));
                guild.Members = gfounder.ToArray();
            }
            else
            {
                List<int> members = guild.Members.ToList();

                if (members.Contains(Convert.ToInt32(acc.AccountId)))
                    return AddGuildMemberStatus.IsAMember;

                members.Add(Convert.ToInt32(acc.AccountId));
                guild.Members = members.ToArray();
            }

            guild.Flush();

            acc.GuildId = Convert.ToString(guild.Id);
            acc.GuildRank = (founder) ? 40 : 0;
            Update(acc);

            return AddGuildMemberStatus.OK;
        }

        public bool RemoveFromGuild(DbAccount acc)
        {
            var guild = GetGuild(acc.GuildId);

            if (guild == null)
                return false;

            List<int> members;
            members = guild.Members.ToList();
            if (members.Contains(Convert.ToInt32(acc.AccountId)))
            {
                members.Remove(Convert.ToInt32(acc.AccountId));
                guild.Members = members.ToArray();
                guild.Flush();
            }
            var idBuff = BitConverter.GetBytes(guild.Id);

            if (members.Count <= 0)
                using (var t = CreateTransaction())
                    t.Hashes.Remove(0, "guilds", guild.Name.ToUpperInvariant());

            acc.GuildId = "-1";
            acc.GuildRank = -1;
            acc.GuildFame = -1;
            Update(acc);
            return true;
        }

        public bool ChangeGuildRank(DbAccount acc, int rank)
        {
            if (Convert.ToInt32(acc.GuildId) <= 0 || !(new Int16[] { 0, 10, 20, 30, 40 }).Any(r => r == rank))
                return false;

            acc.GuildRank = rank;
            Update(acc);
            return true;
        }

        public bool SetGuildBoard(DbGuild guild, string text)
        {
            if (guild.IsNull)
                return false;

            guild.Board = text;
            guild.Flush();
            return true;
        }

        public bool ChangeGuildLevel(DbGuild guild, int level)
        {
            if (level != 1 &&
                level != 2 &&
                level != 3)
                return false;

            guild.Level = level;
            guild.Flush();
            return true;
        }

        public void UpdateAccountLifetime(DbAccount account, int accountType, int amount)
        {
            throw new NotImplementedException();
        }
    }
}