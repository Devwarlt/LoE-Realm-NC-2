#region

using LoESoft.Core.config;
using log4net;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace LoESoft.Core
{
    public class Database : IDisposable
    {
        public static ILog log = LogManager.GetLogger(nameof(Database));

        private const int _lockTTL = 60;

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IServer _server;

        public IDatabase Conn => _db;
        public ISubscriber Sub { get; }

        public Database()
        {
            var host = Settings.REDIS_DATABASE.HOST;
            var port = Settings.REDIS_DATABASE.PORT;
            var password = Settings.REDIS_DATABASE.PASSWORD;

            var conString = Settings.REDIS_DATABASE.HOST + ":" + Settings.REDIS_DATABASE.PORT + ",syncTimeout=" + Settings.RESTART_DELAY_MINUTES * 60 * 1000 * 2;

            if (password != null && !password.Equals(""))
                conString += ",password=" + password;

            _redis = ConnectionMultiplexer.Connect(conString);
            _server = _redis.GetServer(_redis.GetEndPoints(true)[0]);
            _db = _redis.GetDatabase(1);

            Sub = _redis.GetSubscriber();
        }

        public void Dispose() => _redis.Dispose();

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
            return new DbAccount(_db, "0")
            {
                AccountType = (int)AccountType.REGULAR,
                AccountLifetime = DateTime.MinValue,
                UUID = uuid,
                Name = Names[(uint)uuid.GetHashCode() % Names.Length],
                Admin = false,
                NameChosen = false,
                Verified = false,
                Converted = false,
                GuildId = "-1",
                GuildRank = -1,
                GuildFame = -1,
                VaultCount = 1,
                MaxCharSlot = Settings.STARTUP.MAX_CHAR_SLOTS,
                RegTime = DateTime.UtcNow,
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

            var info = new DbLoginInfo(_db, uuid);

            if (info.IsNull)
                return LoginStatus.AccountNotExists;

            var userPass = Utils.SHA1(password + info.Salt);

            if (Convert.ToBase64String(userPass) != info.HashedPassword)
                return LoginStatus.InvalidCredentials;

            acc = new DbAccount(_db, info.AccountId);

            return LoginStatus.OK;
        }

        public bool AcquireLock(DbAccount acc)
        {
            string lockToken = Guid.NewGuid().ToString();
            string key = "lock." + acc.AccountId;

            var trans = _db.CreateTransaction();

            trans.AddCondition(Condition.KeyNotExists(key));
            trans.StringSetAsync(key, lockToken, TimeSpan.FromSeconds(_lockTTL));

            var committed = trans.Execute();

            acc.LockToken = committed ? lockToken : null;
            return committed;
        }

        public bool RenewLock(DbAccount acc)
        {
            var trans = _db.CreateTransaction();
            var key = $"lock.{acc.AccountId}";

            trans.AddCondition(Condition.StringEqual(key, acc.LockToken));
            trans.KeyExpireAsync(key, TimeSpan.FromSeconds(_lockTTL));

            return trans.Execute();
        }

        public void ReleaseLock(DbAccount acc)
        {
            var trans = _db.CreateTransaction();
            var key = $"lock.{acc.AccountId}";

            trans.AddCondition(Condition.StringEqual(key, acc.LockToken));
            trans.KeyDeleteAsync(key);

            trans.ExecuteAsync(CommandFlags.FireAndForget);
        }

        public IDisposable Lock(DbAccount acc) => new L(this, acc);

        public bool LockOk(IDisposable l) => ((L)l).lockOk;

        private struct L : IDisposable
        {
            private Database db;
            private readonly DbAccount acc;
            internal bool lockOk;

            public L(Database db, DbAccount acc)
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
            var trans = _db.CreateTransaction();
            var lockToken = Guid.NewGuid().ToString();

            trans.AddCondition(Condition.KeyNotExists(key));
            trans.StringSetAsync(key, lockToken, TimeSpan.FromSeconds(_lockTTL));

            return trans.Execute() ? lockToken : null;
        }

        public void ReleaseLock(string key, string token)
        {
            var trans = _db.CreateTransaction();
            trans.AddCondition(Condition.StringEqual(key, token));
            trans.KeyDeleteAsync(key);
            trans.Execute();
        }

        public bool RenameUUID(DbAccount acc, string newUuid, string lockToken)
        {
            var p = _db.HashGet("login", acc.UUID.ToUpperInvariant());
            var trans = _db.CreateTransaction();
            trans.AddCondition(Condition.StringEqual(REG_LOCK, lockToken));
            trans.AddCondition(Condition.HashNotExists("login", newUuid.ToUpperInvariant()));
            trans.HashDeleteAsync("login", acc.UUID.ToUpperInvariant());
            trans.HashSetAsync("login", newUuid.ToUpperInvariant(), p);

            if (!trans.Execute())
                return false;

            acc.UUID = newUuid;
            acc.FlushAsync();
            return true;
        }

        public bool RenameIGN(DbAccount acc, string newName, string lockToken)
        {
            if (Names.Contains(newName, StringComparer.InvariantCultureIgnoreCase))
                return false;

            var trans = _db.CreateTransaction();
            trans.AddCondition(Condition.StringEqual(NAME_LOCK, lockToken));
            trans.HashDeleteAsync("names", acc.Name.ToUpperInvariant());
            trans.HashSetAsync("names", newName.ToUpperInvariant(), acc.AccountId.ToString());

            if (!trans.Execute())
                return false;

            acc.Name = newName;
            acc.NameChosen = true;
            acc.FlushAsync();
            return true;
        }

        private static RandomNumberGenerator gen = RandomNumberGenerator.Create();

        public void ChangePassword(string uuid, string password)
        {
            var login = new DbLoginInfo(_db, uuid);

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

            if (!_db.HashSet("logins", uuid.ToUpperInvariant(), "{}", When.NotExists))
                return RegisterStatus.UsedName;

            int newAccId = (int)_db.StringIncrement("nextAccId");

            acc = new DbAccount(_db, newAccId.ToString())
            {
                AccountType = (int)AccountType.REGULAR,
                AccountLifetime = DateTime.MinValue,
                UUID = uuid,
                Name = Names[(uint)uuid.GetHashCode() % Names.Length],
                Admin = false,
                NameChosen = false,
                Verified = Settings.STARTUP.VERIFIED,
                Converted = false,
                GuildId = "-1",
                GuildRank = 0,
                GuildFame = 0,
                VaultCount = 1,
                MaxCharSlot = Settings.STARTUP.MAX_CHAR_SLOTS,
                RegTime = DateTime.UtcNow,
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
            acc.FlushAsync();

            var login = new DbLoginInfo(_db, uuid);

            var x = new byte[0x10];
            gen.GetNonZeroBytes(x);
            string salt = Convert.ToBase64String(x);
            string hash = Convert.ToBase64String(Utils.SHA1(password + salt));

            login.HashedPassword = hash;
            login.Salt = salt;
            login.AccountId = acc.AccountId;
            login.Flush();

            var stats = new DbClassStats(acc);
            stats.FlushAsync();

            var vault = new DbVault(acc);
            vault[0] = Enumerable.Repeat(-1, 8).ToArray();
            vault.FlushAsync();

            return RegisterStatus.OK;
        }

        public bool HasUUID(string uuid) => _db.HashExists("login", uuid.ToUpperInvariant());

        public DbAccount GetAccountById(string id)
        {
            var ret = new DbAccount(_db, id);

            if (ret.IsNull)
                return null;

            return ret;
        }

        public DbAccount GetAccountByUUID(string uuid)
        {
            var info = new DbLoginInfo(_db, uuid);

            if (info.IsNull)
                return null;

            var ret = new DbAccount(_db, info.AccountId);

            if (ret.IsNull)
                return null;

            return ret;
        }

        public string ResolveId(string name) => (string)_db.HashGet("names", name.ToUpperInvariant()) ?? "0";

        public string ResolveIgn(string accId) => (string)_db.HashGet($"account.{accId}", "name") ?? "Unknown";

        public string ResolveIgn(DbAccount acc) => (string)_db.HashGet($"account.{acc.AccountId}", "name") ?? "Unknown";

        public void AddSkin(DbAccount acc, int skin)
        {
            var skinList = acc.OwnedSkins.ToList();
            skinList.Add(skin);

            acc.OwnedSkins = skinList.ToArray();
            Update(acc);
        }

        public bool CheckMysteryBox(DbAccount acc, int box, int total)
            => acc.PurchasedBoxes.Where(mbox => mbox == box).ToList().Count >= total;

        public bool CheckPackage(DbAccount acc, int package, int total)
            => acc.PurchasedPackages.Where(pack => pack == package).ToList().Count >= total;

        public void AddMysteryBox(DbAccount acc, int box)
        {
            var boxList = acc.PurchasedBoxes.ToList();
            boxList.Add(box);

            acc.PurchasedBoxes = boxList.ToArray();

            _db.HashSet(acc.Key, "PurchasedBoxes", Utils.GetCommaSepString(acc.PurchasedBoxes));

            Update(acc);
        }

        public void AddPackage(DbAccount acc, int package)
        {
            var packageList = acc.PurchasedPackages.ToList();
            packageList.Add(package);

            acc.PurchasedPackages = packageList.ToArray();

            _db.HashSet(acc.Key, "purchasedPackages", Utils.GetCommaSepString(acc.PurchasedPackages));

            Update(acc);
        }

        public void UpdateCredit(DbAccount acc, int amount)
        {
            var trans = _db.CreateTransaction();
            trans.HashIncrementAsync(acc.Key, "credits", amount);
            trans.ExecuteAsync();

            acc.Credits += amount;

            Update(acc);
        }

        public void UpdateFame(DbAccount acc, double amount, bool fromDeath = false)
        {
            var trans = _db.CreateTransaction();
            trans.HashIncrementAsync(acc.Key, "fame", amount);

            if (fromDeath)
                trans.HashIncrementAsync(acc.Key, "totalFame", amount);

            trans.ExecuteAsync();

            acc.Fame += amount;

            if (fromDeath)
                acc.TotalFame += amount;

            Update(acc);
        }

        public void UpdateTokens(DbAccount acc, int amount)
        {
            var trans = _db.CreateTransaction();
            trans.HashIncrementAsync(acc.Key, "fortuneTokens", amount);
            trans.ExecuteAsync();

            acc.FortuneTokens += amount;

            Update(acc);
        }

        public void UpdateEmpiresCoin(DbAccount acc, int amount)
        {
            var trans = _db.CreateTransaction();
            trans.HashIncrementAsync(acc.Key, "empiresCoin", amount);
            trans.ExecuteAsync();

            acc.EmpiresCoin += amount;

            Update(acc);
        }

        public void UpdateAccountLifetime(DbAccount acc, AccountType accType, int amount)
        {
            acc.AccountLifetime = DateTime.UtcNow;
            acc.AccountLifetime = acc.AccountLifetime.AddDays(amount);
            acc.AccountType = (int)accType;
            Update(acc);
        }

        public void Update(DbAccount acc)
        {
            acc.FlushAsync();
            acc.Reload();
        }

        public DbClassStats ReadClassStats(DbAccount acc) => new DbClassStats(acc);

        public DbVault ReadVault(DbAccount acc) => new DbVault(acc);

        public int CreateChest(DbVault vault)
        {
            var id = (int)_db.HashIncrement(vault.Account.Key, "vaultCount");
            var newid = id - 1; //since index of vaults is zero it'll be reduced by 1 then incremented to index again

            vault[newid] = Enumerable.Repeat(-1, 8).ToArray();
            vault.FlushAsync();

            return newid;
        }

        public void AddChest(DbAccount acc)
        {
            acc.VaultCount++;
            Update(acc);
        }

        public void AddChar(DbAccount acc)
        {
            acc.MaxCharSlot++;
            Update(acc);
        }

        public DbChar GetAliveCharacter(DbAccount acc)
        {
            int chara = 1;

            foreach (var i in _db.SetMembers("alive." + acc.AccountId).Reverse())
                chara = BitConverter.ToInt32(i, 0);

            return LoadCharacter(acc, chara);
        }

        public IEnumerable<int> GetAliveCharacters(DbAccount acc)
        {
            foreach (var i in _db.SetMembers("alive." + acc.AccountId))
                yield return BitConverter.ToInt32(i, 0);
        }

        public IEnumerable<int> GetDeadCharacters(DbAccount acc)
        {
            foreach (var i in _db.ListRange("dead." + acc.AccountId, 0, int.MaxValue))
                yield return BitConverter.ToInt32(i, 0);
        }

        public bool IsAlive(DbChar character) => _db.SetContains("alive." + character.Account.AccountId, BitConverter.GetBytes(character.CharId));

        public CreateStatus CreateCharacter(EmbeddedData dat, DbAccount acc, ushort type, int skin, out DbChar character)
        {
            var @class = dat.ObjectTypeToElement[type];

            if (_db.SetLength("alive." + acc.AccountId) >= acc.MaxCharSlot)
            {
                character = null;
                return CreateStatus.ReachCharLimit;
            }

            var newId = (int)_db.HashIncrement(acc.Key, "nextCharId");

            character = new DbChar(acc, newId)
            {
                //LootCaches = new LootCache[] { },
                ObjectType = type,
                Level = 1,
                Experience = 0,
                FakeExperience = 0,
                IsFakeEnabled = true,
                Bless1 = false,
                Bless2 = false,
                Bless3 = false,
                Bless4 = false,
                Bless5 = false,
                EnablePetAttack = true,
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
                CreateTime = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };
            character.FlushAsync();
            _db.SetAdd("alive." + acc.AccountId, BitConverter.GetBytes(newId));
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
            var acc = new DbAccount(_db, accId.ToString());
            if (acc.IsNull)
                return null;
            var ret = new DbChar(acc, charId);
            if (ret.IsNull)
                return null;
            return ret;
        }

        public bool SaveCharacter(DbAccount acc, DbChar character, bool lockAcc)
        {
            var trans = _db.CreateTransaction();

            if (lockAcc)
                trans.AddCondition(Condition.StringEqual($"lock.{acc.AccountId}", acc.LockToken));

            character.FlushAsync(trans);

            var stats = new DbClassStats(acc);
            stats.Update(character);
            stats.FlushAsync(trans);

            return trans.Execute();
        }

        public void DeleteCharacter(DbAccount acc, int charId)
        {
            _db.KeyDeleteAsync("char." + acc.AccountId + "." + charId);

            var buff = BitConverter.GetBytes(charId);

            _db.SetRemoveAsync("alive." + acc.AccountId, buff);
            _db.ListRemoveAsync("dead." + acc.AccountId, buff);
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
                DeathTime = DateTime.UtcNow
            };
            death.FlushAsync();

            var idBuff = BitConverter.GetBytes(character.CharId);
            _db.SetRemoveAsync("alive." + acc.AccountId, idBuff, CommandFlags.FireAndForget);
            _db.ListLeftPushAsync("dead." + acc.AccountId, idBuff, When.Always, CommandFlags.FireAndForget);

            UpdateFame(acc, finalFame, true);

            if (acc.AccountType <= (int)AccountType.VIP)
                DbLegend.Insert(_db, int.Parse(acc.AccountId), character.CharId, finalFame);
        }

        public void VerifyAge(DbAccount acc)
        {
            _db.HashSet(acc.Key, "isAgeVerified", "1");

            Update(acc);
        }

        public void ChangeClassAvailability(DbAccount acc, EmbeddedData data, ushort type)
        {
            int price;
            if (acc.Credits < (price = data.ObjectDescs[type].UnlockCost))
                return;

            _db.HashSet($"classAvailability.{acc.AccountId}", type.ToString(),
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
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder();
            var random = rand ?? new Random();

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
            _db.HashSetAsync(acc.Key, "muted", "1");

            Update(acc);
        }

        public void UnmuteAccount(DbAccount acc)
        {
            _db.HashSetAsync(acc.Key, "muted", "0");

            Update(acc);
        }

        public bool BanAccount(Database db, string accId)
        {
            var acc = new DbAccount(_db, accId) { Banned = true };

            if (acc.IsNull || acc.AccountType >= (int)AccountType.MOD)
                return false;

            acc.FlushAsync();

            Update(acc);

            return true;
        }

        public bool UnBanAccount(Database db, string accId)
        {
            var acc = new DbAccount(_db, accId);

            if (acc.Banned)
            {
                acc.Banned = false;
                acc.FlushAsync();

                return true;
            }

            return false;
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
                List<int> x = new List<int>
                {
                    lockId
                };
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
                List<int> x = new List<int>
                {
                    lockId
                };
                int[] result = x.ToArray();
                acc.Ignored = result;
                Update(acc);
            }
        }

        public GuildCreateStatus CreateGuild(string guildName, out DbGuild guild)
        {
            guild = null;

            if (string.IsNullOrWhiteSpace(guildName))
                return GuildCreateStatus.InvalidName;

            guildName = guildName.Trim();

            var newGuildId = (int)_db.StringIncrement("nextGuildId");

            if (!_db.HashSet("guilds", guildName.ToUpperInvariant(), newGuildId, When.NotExists))
                return GuildCreateStatus.UsedName;

            guild = new DbGuild(_db, newGuildId)
            {
                Name = guildName,
                Level = 0,
                Fame = 0,
                TotalFame = 0
            };

            guild.FlushAsync();

            return GuildCreateStatus.OK;
        }

        public DbGuild GetGuild(string guildId)
        {
            int id = Convert.ToInt32(guildId);

            DbGuild ret = new DbGuild(_db, id);

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
                List<int> gfounder = new List<int>
                {
                    Convert.ToInt32(acc.AccountId)
                };
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

            guild.FlushAsync();

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
                guild.FlushAsync();
            }
            var idBuff = BitConverter.GetBytes(guild.Id);

            if (members.Count <= 0)
            {
                var trans = _db.CreateTransaction();
                trans.HashDeleteAsync("guilds", guild.Name.ToUpperInvariant());
                trans.Execute();
            }

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
            guild.FlushAsync();
            return true;
        }

        public bool ChangeGuildLevel(DbGuild guild, int level)
        {
            if (level != 1 &&
                level != 2 &&
                level != 3)
                return false;

            guild.Level = level;
            guild.FlushAsync();
            return true;
        }

        public int LastLegendsUpdateTime()
        {
            var time = _db.StringGet("legends.updateTime");

            if (time.IsNullOrEmpty)
                return -1;

            return int.Parse(time);
        }

        public DbChar[] GetLegendsBoard(string timeSpan)
        {
            return DbLegend
                .Get(_db, timeSpan)
                .Select(e => LoadCharacter(e.AccId, e.ChrId))
                .Where(e => e != null)
                .ToArray();
        }
    }
}
