#region

using LoESoft.Core.config;
using LoESoft.Core.database;
using LoESoft.GameServer.logic;
using LoESoft.GameServer.logic.skills.Pets;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    internal interface IPlayer
    {
        void Damage(int dmg, Entity chr, bool NoDef, bool manaDrain = false);

        bool IsVisibleToEnemy();
    }

    public static class ComparableExtension
    {
        public static bool InRange<T>(this T value, T from, T to) where T : IComparable<T> => value.CompareTo(from) >= 1 && value.CompareTo(to) <= -1;
    }

    public partial class Player : Character, IContainer, IPlayer
    {
        public Player(Client client) : base(client.Character.ObjectType, client.Random)
        {
            try
            {
				
                if (client.Account.Admin == true)
                    Admin = 1;
                Achievements = new List<string>();
                ActualTask = null;
                MonsterCaches = new List<MonsterCache>();
                Task = null;
                AccountType = client.Account.AccountType;
                AccountPerks = new AccountTypePerks(AccountType);
                AccountLifetime = client.Account.AccountLifetime;
                IsVip = AccountLifetime != DateTime.MinValue;
                Client = client;
                StatsManager = new StatsManager(this, client.Random.CurrentSeed);
                Name = client.Account.Name;
                AccountId = client.Account.AccountId;
                FameCounter = new FameCounter(this);
                Tokens = client.Account.FortuneTokens;
                HpPotionPrice = 5;
                MpPotionPrice = 5;
                Level = client.Character.Level == 0 ? 1 : client.Character.Level;
                Experience = client.Character.Experience;
                ExperienceGoal = GetExpGoal(Level);
                Stars = AccountType >= (int)Core.config.AccountType.DEVELOPER ? 70 : GetStars();
                Texture1 = client.Character.Tex1;
                Texture2 = client.Character.Tex2;
                Credits = client.Account.Credits;
                NameChosen = client.Account.NameChosen;
                CurrentFame = client.Account.Fame;
                Fame = client.Character.Fame;
                PetHealing = null;
                PetAttack = null;
                if (client.Character.Pet != 0)
                {
                    PetHealing = new List<List<int>>();
                    PetAttack = new List<int>();
                    PetID = client.Character.Pet;
                    var HPData = PetHPHealing.MinMaxBonus(Resolve((ushort)PetID).ObjectDesc.HPTier, Stars);
                    var MPData = PetMPHealing.MinMaxBonus(Resolve((ushort)PetID).ObjectDesc.MPTier, Stars);
                    PetHealing.Add(new List<int> { HPData.Item1, HPData.Item2, (int)((HPData.Item3 - 1) * 100) });
                    PetHealing.Add(new List<int> { MPData.Item1, MPData.Item2, (int)((MPData.Item3 - 1) * 100) });
                    PetAttack.Add(7750 - Stars * 100);
                    PetAttack.Add(30 + Stars);
                    PetAttack.Add(Resolve((ushort)PetID).ObjectDesc.Projectiles[0].MinDamage);
                    PetAttack.Add(Resolve((ushort)PetID).ObjectDesc.Projectiles[0].MaxDamage);
                }
                LootDropBoostTimeLeft = client.Character.LootDropTimer;
                lootDropBoostFreeTimer = LootDropBoost;
                LootTierBoostTimeLeft = client.Character.LootTierTimer;
                lootTierBoostFreeTimer = LootTierBoost;
                FameGoal = (AccountType >= (int)Core.config.AccountType.MOD) ? 0 : GetFameGoal(FameCounter.ClassStats[ObjectType].BestFame);
                Glowing = AccountType == (int)Core.config.AccountType.VIP;
                var guild = GameServer.Manager.Database.GetGuild(client.Account.GuildId);
                if (guild != null)
                {
                    Guild = GameServer.Manager.Database.GetGuild(client.Account.GuildId).Name;
                    GuildRank = client.Account.GuildRank;
                }
                else
                {
                    Guild = "";
                    GuildRank = -1;
                }
                HP = client.Character.HP <= 0 ? (int)ObjectDesc.MaxHP : client.Character.HP;
                MP = client.Character.MP;
                ConditionEffects = 0;
                OxygenBar = 100;
                HasBackpack = client.Character.HasBackpack == true;
                PlayerSkin = Client.Account.OwnedSkins.Contains(Client.Character.Skin) ? Client.Character.Skin : 0;
                HealthPotions = client.Character.HealthPotions < 0 ? 0 : client.Character.HealthPotions;
                MagicPotions = client.Character.MagicPotions < 0 ? 0 : client.Character.MagicPotions;

                try
                {
                    Locked = client.Account.Database.GetLockeds(client.Account);
                    Ignored = client.Account.Database.GetIgnoreds(client.Account);
                }
                catch (Exception) { }

                if (HasBackpack)
                {
                    Item[] inv =
                        client.Character.Items.Select(
                            _ =>
                                _ == -1
                                    ? null
                                    : (GameServer.Manager.GameData.Items.ContainsKey((ushort)_) ? GameServer.Manager.GameData.Items[(ushort)_] : null))
                            .ToArray();
                    Item[] backpack =
                        client.Character.Backpack.Select(
                            _ =>
                                _ == -1
                                    ? null
                                    : (GameServer.Manager.GameData.Items.ContainsKey((ushort)_) ? GameServer.Manager.GameData.Items[(ushort)_] : null))
                            .ToArray();

                    Inventory = inv.Concat(backpack).ToArray();
                    XElement xElement = GameServer.Manager.GameData.ObjectTypeToElement[ObjectType].Element("SlotTypes");
                    if (xElement != null)
                    {
                        int[] slotTypes =
                            Utils.FromCommaSepString32(
                                xElement.Value);
                        Array.Resize(ref slotTypes, 20);
                        SlotTypes = slotTypes;
                    }
                }
                else
                {
                    Inventory =
                            client.Character.Items.Select(
                                _ =>
                                    _ == -1
                                        ? null
                                        : (GameServer.Manager.GameData.Items.ContainsKey((ushort)_) ? GameServer.Manager.GameData.Items[(ushort)_] : null))
                                .ToArray();
                    XElement xElement = GameServer.Manager.GameData.ObjectTypeToElement[ObjectType].Element("SlotTypes");
                    if (xElement != null)
                        SlotTypes =
                            Utils.FromCommaSepString32(
                                xElement.Value);
                }
                Stats = (int[])client.Character.Stats.Clone();

                for (var i = 0; i < SlotTypes.Length; i++)
                    if (SlotTypes[i] == 0)
                        SlotTypes[i] = 10;

                if (Client.Account.AccountType >= (int)Core.config.AccountType.DEVELOPER)
                    return;

                for (var i = 0; i < 4; i++)
                    if (Inventory[i]?.SlotType != SlotTypes[i])
                        Inventory[i] = null;
            }
            catch (Exception) { }
        }

        public override void Move(float x, float y)
        {
            if (Pet != null)
            {
                if (Dist(this, Pet) > 20f)
                {
                    Pet.Move(X, Y);
                    UpdateCount++;
                }
            }

            base.Move(x, y);
        }

        private void AnnounceDeath(string killer)
        {
            var playerDesc = GameServer.Manager.GameData.ObjectDescs[ObjectType];
            var rip = Name + " died to " + killer + "( " + playerDesc.ObjectId + ", " + Fame + " Fame )";
            if (Fame >= 2000 && !Client.Account.Admin)
            {
                foreach (var w in GameServer.Manager.Worlds.Values)
                    foreach (var p in w.Players.Values)
                        p.SendDeathAnnounce(rip);
                return;
            }
            else
            {
                foreach (var i in Owner.Players.Values)
                {
                    i.SendDeathAnnounce(rip);
                }
            }
        }

        public void Death(string killer, ObjectDesc desc = null, Entity entity = null)
        {
            if (dying)
                return;
            dying = true;
            switch (Owner.Name)
            {
                case "Arena":
                    {
                        Client.SendMessage(new ARENA_DEATH
                        {
                            RestartPrice = 100
                        });
                        HP = (int)ObjectDesc.MaxHP;
                        ApplyConditionEffect(new ConditionEffect
                        {
                            Effect = ConditionEffectIndex.Paused,
                            DurationMS = -1
                        });
                        return;
                    }
            }
            if (Client.State == ProtocolState.Disconnected || resurrecting)
                return;
            if (CheckResurrection())
                return;

            if (Client.Character.Dead)
            {
                GameServer.Manager.TryDisconnect(Client, DisconnectReason.CHARACTER_IS_DEAD);
                return;
            }
            GenerateGravestone();
            if ((entity is Player))
            {
                AnnounceDeath((entity as Player).Name + " the " + entity.ObjectDesc.ObjectId);
            }
            else
            {
                AnnounceDeath(killer);
            }
            if (desc != null)
                if (desc.DisplayId != null)
                    killer = desc.DisplayId;
                else
                    killer = desc.ObjectId;
            switch (killer)
            {
                case "":
                case "Unknown":
                    break;

                default:
                    Owner.BroadcastMessage(new TEXT
                    {
                        BubbleTime = 0,
                        Stars = -1,
                        Name = "",
                        Text = "{\"key\":\"server.death\",\"tokens\":{\"player\":\"" + Name + "\",\"level\":\"" + Level + "\",\"enemy\":\"" + killer + "\"}}",
                        NameColor = 0x123456,
                        TextColor = 0x123456
                    }, null);
                    break;
            }

            try
            {
                Client.Character.Dead = true;

                SaveToCharacter();

                GameServer.Manager.Database.SaveCharacter(Client.Account, Client.Character, true);
                GameServer.Manager.Database.Death(GameServer.Manager.GameData, Client.Account, Client.Character, FameCounter.Stats, killer);

                if (Owner.Id != -6)
                {
                    DEATH _death = new DEATH
                    {
                        AccountId = AccountId,
                        CharId = Client.Character.CharId,
                        Killer = killer,
                        zombieId = -1,
                        zombieType = -1
                    };

                    Client.SendMessage(_death);

                    Owner.Timers.Add(new WorldTimer(1000, (w, t) => GameServer.Manager.TryDisconnect(Client, DisconnectReason.CHARACTER_IS_DEAD)));

                    Owner.LeaveWorld(this);
                }
                else
                    GameServer.Manager.TryDisconnect(Client, DisconnectReason.CHARACTER_IS_DEAD_ERROR);
            }
            catch (Exception) { }
        }

        public override void Init(World owner)
        {
            MaxHackEntries = 0;
            visibleTiles = new Dictionary<IntPoint, bool>();
            WorldInstance = owner;

            var rand = new Random();

            int x, y;

            do
            {
                x = rand.Next(0, owner.Map.Width);
                y = rand.Next(0, owner.Map.Height);
            } while (owner.Map[x, y].Region != TileRegion.Spawn);

            var newposition = owner.RemovePositionFromReconnect(AccountId);

            if (newposition != null)
                Move((int)newposition.Item1, (int)newposition.Item2);
            else
                Move(x + 0.5f, y + 0.5f);

            tiles = new byte[owner.Map.Width, owner.Map.Height];

            SetNewbiePeriod();

            base.Init(owner);

            var gifts = Client.Account.Gifts.ToList();

            if (owner.Id == (int)WorldID.NEXUS_ID || owner.Name == "Vault")
            {
                Client.SendMessage(new GLOBAL_NOTIFICATION
                {
                    Type = 0,
                    Text = gifts.Count > 0 ? "giftChestOccupied" : "giftChestEmpty"
                });
            }

            if (Client.Character.Pet != 0)
            {
                HatchlingPet = false;
                HatchlingNotification = false;
                Pet = Resolve((ushort)PetID);
                Pet.Move(x, y);
                Pet.SetPlayerOwner(this);
                Owner.EnterWorld(Pet);
                Pet.IsPet = true;
            }
			//var player = Resolve("Filisha");
			SendAccountList(Locked, ACCOUNTLIST.LOCKED_LIST_ID);
            SendAccountList(Ignored, ACCOUNTLIST.IGNORED_LIST_ID);

            CheckSetTypeSkin();

            if (Settings.SERVER_MODE == Settings.ServerMode.Local)
                if ((AccountType)AccountType == Core.config.AccountType.DEVELOPER)
                {
                    var invincible = new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Invincible,
                        DurationMS = -1
                    };

                    ApplyConditionEffect(invincible);

                    var invulnerable = new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Invulnerable,
                        DurationMS = -1
                    };

                    ApplyConditionEffect(invulnerable);
                }

            ApplyConditionEffect(AccountPerks.SetAccountTypeIcon());

            Achievements = ImportAchivementCache();
            ActualTask = ImportTaskCache();
            MonsterCaches = ImportMonsterCaches();

            if (ActualTask != null)
                Task = GameTask.Tasks[ActualTask];
        }

        public void Teleport(RealmTime time, TELEPORT packet)
        {
            var obj = Client.Player.Owner.GetEntity(packet.ObjectId);
            try
            {
                if (obj == null)
                    return;
                if (!TPCooledDown())
                {
                    SendError("Player.teleportCoolDown");
                    return;
                }
                if (obj.HasConditionEffect(ConditionEffectIndex.Invisible))
                {
                    SendError("server.no_teleport_to_invisible");
                    return;
                }
                if (obj.HasConditionEffect(ConditionEffectIndex.Paused))
                {
                    SendError("server.no_teleport_to_paused");
                    return;
                }
                if (obj is Player player && !player.NameChosen)
                {
                    SendError("server.teleport_needs_name");
                    return;
                }
                if (obj.Id == Id)
                {
                    SendError("server.teleport_to_self");
                    return;
                }
                if (!Owner.AllowTeleport)
                {
                    SendError(GetLanguageString("server.no_teleport_in_realm", new KeyValuePair<string, object>("realm", Owner.Name)));
                    return;
                }

                SetTPDisabledPeriod();
                Move(obj.X, obj.Y);
                FameCounter.Teleport();
                SetNewbiePeriod();
                UpdateCount++;
            }
            catch (Exception)
            {
                SendError("player.cannotTeleportTo");
                return;
            }

            Owner.BroadcastMessage(new GOTO
            {
                ObjectId = Id,
                Position = new Position
                {
                    X = X,
                    Y = Y
                }
            }, null);
            Owner.BroadcastMessage(new SHOWEFFECT
            {
                EffectType = EffectType.Teleport,
                TargetId = Id,
                PosA = new Position
                {
                    X = X,
                    Y = Y
                },
                Color = new ARGB(0xFFFFFFFF)
            }, null);

            foreach (var plr in Owner.Players.Values)
                plr.AwaitGotoAck(time.TotalElapsedMs);
        }

        private int QuestPriority(ObjectDesc enemy)
        {
            int score = 0;

            if (enemy.Oryx)
                score += 100000;

            if (enemy.Cube)
                score += 2500;

            if (enemy.God)
                score += 500;

            if (enemy.Hero)
                score += 1250;

            if (enemy.Encounter)
                score += 5000;

            if (enemy.Quest)
                score += 250;

            if (enemy.ObjectId.ToLower().Contains("maurth"))
                score += 100000;

            if (enemy.ObjectId.ToLower().Contains("undertaker"))
                score += 500000;

            score += enemy.MaxHitPoints;
            score += enemy.Defense * enemy.Level;

            return score;
        }

        private void HandleQuest(RealmTime time)
        {
            if (time.TickCount % 5 != 0)
                return;

            int newQuestId = -1;
            int questId = Quest == null ? -1 : Quest.Id;

            HashSet<Enemy> candidates = new HashSet<Enemy>();

            foreach (Enemy i in Owner.Quests.Values
                .OrderBy(j => MathsUtils.DistSqr(j.X, j.Y, X, Y))
                .Where(k => k.ObjectDesc != null && k.ObjectDesc.Quest))
            {
                if (!RealmManager.QuestPortraits.TryGetValue(i.ObjectDesc.ObjectId, out int questLevel))
                    continue;

                if (Level < questLevel)
                    continue;

                if (!RealmManager.QuestPortraits.ContainsKey(i.ObjectDesc.ObjectId))
                    continue;

                candidates.Add(i);
            }

            if (candidates.Count != 0)
            {
                Enemy newQuest = candidates.OrderByDescending(i => QuestPriority(i.ObjectDesc)).Take(3).ToList()[0];

                newQuestId = newQuest.Id;
                Quest = newQuest;
            }

            if (newQuestId == questId)
                return;

            Client.SendMessage(new QUESTOBJID
            {
                ObjectId = newQuestId
            });
        }

        private void CalculateFame()
        {
            double newFame = 0.0;

            newFame += Math.Max(0, Math.Min(20000, Experience)) * 0.001;
            newFame += Math.Max(0, Math.Min(45200, Experience) - 20000) * 0.002;
            newFame += Math.Max(0, Math.Min(80000, Experience) - 45200) * 0.003;
            newFame += Math.Max(0, Math.Min(101200, Experience) - 80000) * 0.002;
            newFame += Math.Max(0, Experience - 101200) * 0.0005;
            newFame += Math.Min(Math.Floor((double)FameCounter.Stats.MinutesActive / 6), 30);

            newFame = Math.Floor(newFame);

            if (newFame == Fame)
                return;

            Fame = (int)newFame;
            int newGoal;
            var stats = FameCounter.ClassStats[ObjectType];
            if (stats.BestFame > Fame)
                newGoal = GetFameGoal(stats.BestFame);
            else
                newGoal = GetFameGoal(Fame);
            if (newGoal > FameGoal && AccountType < (int)Core.config.AccountType.MOD)
            {
                Owner.BroadcastMessage(new NOTIFICATION
                {
                    ObjectId = Id,
                    Color = new ARGB(0xFF00FF00),
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Class Quest Complete!\"}}",
                }, null);
                Stars = GetStars();
            }
            FameGoal = (AccountType >= (int)Core.config.AccountType.MOD) ? 0 : newGoal;
            UpdateCount++;
        }

        private bool CheckLevelUp()
        {
            if (Experience - GetLevelExp(Level) >= ExperienceGoal && Level < 20)
            {
                Level++;
                ExperienceGoal = GetExpGoal(Level);
                foreach (var i in GameServer.Manager.GameData.ObjectTypeToElement[ObjectType].Elements("LevelIncrease"))
                {
                    var rand = new Random();
                    var min = int.Parse(i.Attribute("min").Value);
                    var max = int.Parse(i.Attribute("max").Value) + 1;
                    var xElement = GameServer.Manager.GameData.ObjectTypeToElement[ObjectType].Element(i.Value);
                    if (xElement == null)
                        continue;
                    var limit =
                        int.Parse(
                            xElement.Attribute("max").Value);
                    var idx = StatsManager.StatsNameToIndex(i.Value);
                    Stats[idx] += rand.Next(min, max);
                    if (Stats[idx] > limit)
                        Stats[idx] = limit;
                }
                HP = Stats[0] + Boost[0];
                MP = Stats[1] + Boost[1];

                UpdateCount++;

                var playerDesc = GameServer.Manager.GameData.ObjectDescs[ObjectType];
                if (Level == 20)
                {
                    foreach (var i in Owner.Players.Values)
                        i.SendInfo(Name + " achieved level 20" + " as a " + playerDesc.ObjectId);
                    XpBoosted = false;
                    XpBoostTimeLeft = 0;
                }
				if (Stars == 2)
				{
					SendInfo("You can now use the /glands Feature!");
				}
				if (Stars == 10)
				{
						SendInfo("You can now use the Hotkey Feature! Go into Options>Misc and set your keybinds!");
				}
					Quest = null;
                return true;
            }
            CalculateFame();
            return false;
        }

        public bool EnemyKilled(Enemy enemy, int exp, bool killer)
        {
            if (enemy == Quest)
                Owner.BroadcastMessage(new NOTIFICATION
                {
                    ObjectId = Id,
                    Color = new ARGB(0xFF00FF00),
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Quest Complete!\"}}",
                }, null);
            if (exp > 0 && Experience != int.MaxValue)
            {
                double newexp = Experience;

                if (XpBoosted)
                    newexp += (int)(exp * 2 * Settings.EVENT_RATE);
                else
                    newexp += (int)(exp * Settings.EVENT_RATE);

                if (newexp >= int.MaxValue)
                {
                    Experience = int.MaxValue;
                    SendInfo("You achieved the maximum experience!");
                }
                else
                    Experience = (int)newexp;

                UpdateCount++;

                foreach (var i in Owner.PlayersCollision.HitTest(X, Y, 16).Where(i => i != this).OfType<Player>())
                {
                    try
                    {
                        double tempexp = i.Experience;
                        tempexp += (int)((i.XpBoosted ? exp * 2 : exp) * Settings.EVENT_RATE);

                        if (tempexp >= int.MaxValue)
                        {
                            i.Experience = int.MaxValue;
                            i.SendInfo("You achieved the maximum experience!");
                        }
                        else
                            i.Experience = (int)tempexp;

                        i.UpdateCount++;
                        i.CheckLevelUp();
                    }
                    catch (Exception) { }
                }
            }
            FameCounter.Killed(enemy, killer);
            return CheckLevelUp();
        }

        internal Projectile PlayerShootProjectile(
            byte id,
            ProjectileDesc desc,
            ushort objType,
            long time,
            Position position,
            float angle
            )
        {
            ProjectileId = id;
            return CreateProjectile(desc, objType, (int)StatsManager.GetAttackDamage(desc.MinDamage, desc.MaxDamage), time, position, angle);
        }

        public override void Tick(RealmTime time)
        {
            if (Client == null)
                return;

            if (!KeepAlive(time) || Client.State == ProtocolState.Disconnected)
            {
                if (Owner != null)
                    Owner.LeaveWorld(this);
                else
                    WorldInstance.LeaveWorld(this);

                return;
            }

            if (Stats != null && Boost != null)
            {
                MaxHp = Stats[0] + Boost[0];
                MaxMp = Stats[1] + Boost[1];
            }

            if (Boost == null)
                CalculateBoost();

            if (!HasConditionEffect(ConditionEffects.Paused))
            {
                HandleRegen(time);

                HandleGround(time);

                FameCounter.Tick(time);
            }

            HandleTrade?.Tick(time);

            try
            {
                HandleQuest(time);
            }
            catch (NullReferenceException) { }

            HandleEffects(time);

            HandleBoosts();

            if (MP < 0)
                MP = 0;

            if (Owner != null)
            {
                HandleNewTick(time);

                HandleUpdate(time);
            }

            if (HP < 0 && !dying)
            {
                Death("Unknown");
                return;
            }

            base.Tick(time);
        }
    }
}