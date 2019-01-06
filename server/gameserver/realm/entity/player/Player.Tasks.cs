using LoESoft.Core.database;
using LoESoft.GameServer.networking.outgoing;
using System;
using System.Collections.Generic;

namespace LoESoft.GameServer.realm.entity.player
{
    public partial class Player
    {
        public string ActualTask { get; set; }
        public GameTask Task { get; set; }
        public List<string> Achievements { get; set; }

        public class GameTask
        {
            public static Dictionary<string, GameTask> Tasks = new Dictionary<string, GameTask>()
            {
                {
                    "In Service of Gazer 1",
                    new GameTask()
                    {
                        StarsRequirement = 14,
                        MultipleTimes = true,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Muzzlereaper", Total = 50 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "Succubus Horn x 1", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 25);

                            player.Experience += 1000;
                            player.CurrentFame = player.Client.Account.Fame += 25;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "1,000 EXP", "25 Fame", "(Vault) 1 x Succubus Horn"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "In Service of Gazer 2",
                    new GameTask()
                    {
                        StarsRequirement = 24,
                        MultipleTimes = true,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Muzzlereaper", Total = 100 },
                            new MonsterData() { ObjectId = "Guzzlereaper", Total = 25 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "Succubus Horn x 1", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 100);
                            GameServer.Manager.Database.UpdateCredit(player.Client.Account, 5);

                            player.Experience += 10000;
                            player.CurrentFame = player.Client.Account.Fame += 100;
                            player.Credits = player.Client.Account.Credits += 5;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "10,000 EXP", "100 Fame", "5 Realm Gold", "(Vault) 1 x Succubus Horn"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "In Service of Gazer 3",
                    new GameTask()
                    {
                        StarsRequirement = 32,
                        MultipleTimes = true,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Silencer", Total = 50 },
                            new MonsterData() { ObjectId = "Nightmare", Total = 50 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "Succubus Horn x 2", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 225);
                            GameServer.Manager.Database.UpdateCredit(player.Client.Account, 10);

                            player.Experience += 25000;
                            player.CurrentFame = player.Client.Account.Fame += 225;
                            player.Credits = player.Client.Account.Credits += 10;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "25,000 EXP", "225 Fame", "10 Realm Gold", "(Vault) 2 x Succubus Horn"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "In Service of Gazer 4",
                    new GameTask()
                    {
                        StarsRequirement = 45,
                        MultipleTimes = true,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Lost Prisoner Soul", Total = 35 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "Succubus Horn x 2", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 350);
                            GameServer.Manager.Database.UpdateCredit(player.Client.Account, 15);

                            player.Experience += 37500;
                            player.CurrentFame = player.Client.Account.Fame += 350;
                            player.Credits = player.Client.Account.Credits += 15;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "37,500 EXP", "350 Fame", "15 Realm Gold", "(Vault) 2 x Succubus Horn"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "In Service of Gazer 5",
                    new GameTask()
                    {
                        StarsRequirement = 56,
                        MultipleTimes = true,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Eyeguard of Surrender", Total = 25 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "Succubus Horn x 2", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 500);
                            GameServer.Manager.Database.UpdateCredit(player.Client.Account, 20);

                            player.Experience += 50000;
                            player.CurrentFame = player.Client.Account.Fame += 500;
                            player.Credits = player.Client.Account.Credits += 20;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "50,000 EXP", "500 Fame", "20 Realm Gold", "(Vault) 2 x Succubus Horn"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "The Dream Warden",
                    new GameTask()
                    {
                        StarsRequirement = 28,
                        MultipleTimes = false,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Muzzlereaper", Total = 1500 },
                            new MonsterData() { ObjectId = "Guzzlereaper", Total = 750 },
                            new MonsterData() { ObjectId = "Silencer", Total = 500 },
                            new MonsterData() { ObjectId = "Nightmare", Total = 500 },
                            new MonsterData() { ObjectId = "Lost Prisoner Soul", Total = 275 },
                            new MonsterData() { ObjectId = "Eyeguard of Surrender", Total = 100 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "Succubus Horn x 20", Total = 1 },
                            new RewardData() { ObjectId = "Warden Two-handed Sword", Total = 1 },
                            new RewardData() { ObjectId = "Warden Helmet", Total = 1 },
                            new RewardData() { ObjectId = "The Sentinel Warden", Total = 1 },
                            new RewardData() { ObjectId = "Bracer of the Guardian", Total = 1 },
                            new RewardData() { ObjectId = "lcp egg dream t0-100 sb", Total = 8 },
                            new RewardData() { ObjectId = "Blue Star Ensign", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 10000);

                            player.Experience += 1000000;
                            player.CurrentFame = player.Client.Account.Fame += 10000;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "1,000,000 EXP", "10,000 Fame", "(Vault) 20 x Succubus Horn", "(Vault) 1 x Forgotten Sentinel Set",
                                "(Vault) 8 x Muzzlereaper Minion Egg (SB)", "(Vault) 1 x Ensign of Fortune (Blue)"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "Ghost Buster",
                    new GameTask()
                    {
                        StarsRequirement = 0,
                        MultipleTimes = false,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Ghost God", Total = 25 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "lcp egg t0-100", Total = 1 },
                            new RewardData() { ObjectId = "lcp egg t1-75", Total = 1 },
                            new RewardData() { ObjectId = "lcp egg t2-50", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 100);

                            player.Experience += 1000;
                            player.CurrentFame = player.Client.Account.Fame += 100;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "1,000 EXP", "100 Fame", "(Vault) 1 x Little Ghost Egg (100%)", "(Vault) 1 x Chilling Fire Egg (75%)",
                                "(Vault) 1 x Mini Tree Egg (50%)"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "Medusa Buster",
                    new GameTask()
                    {
                        StarsRequirement = 0,
                        MultipleTimes = false,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Medusa", Total = 50 }
                        },
                        RewardDatas = new List<RewardData>()
                        {
                            new RewardData() { ObjectId = "lcp egg t1-100", Total = 1 },
                            new RewardData() { ObjectId = "lcp egg t2-75", Total = 1 },
                            new RewardData() { ObjectId = "lcp egg t3-50", Total = 1 }
                        },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 300);

                            player.Experience += 5000;
                            player.CurrentFame = player.Client.Account.Fame += 300;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "5,000 EXP", "300 Fame", "(Vault) 1 x Chilling Fire Egg (100%)", "(Vault) 1 x Mini Tree Egg (75%)",
                                "(Vault) 1 x Mini Icycle Egg (50%)"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                },
                {
                    "Red Demon Hunter",
                    new GameTask()
                    {
                        StarsRequirement = 0,
                        MultipleTimes = false,
                        MonsterDatas = new List<MonsterData>()
                        {
                            new MonsterData() { ObjectId = "Red Demon", Total = 5 }
                        },
                        RewardDatas = new List<RewardData>() { },
                        Bonus = (player) =>
                        {
                            GameServer.Manager.Database.UpdateFame(player.Client.Account, 500);

                            player.Experience += 10000;
                            player.CurrentFame = player.Client.Account.Fame += 500;
                            player.UpdateCount++;

                            var notifications = new List<string>()
                            {
                                "10,000 EXP", "500 Fame"
                            };

                            player.SendInfo("You received as bonus reward:");

                            for (int i = 0; i < notifications.Count; i++)
                                player.SendInfo($"- {notifications[i]}");
                        }
                    }
                }
            };

            public int StarsRequirement { get; set; }
            public bool MultipleTimes { get; set; }
            public List<MonsterData> MonsterDatas { get; set; }
            public List<RewardData> RewardDatas { get; set; }
            public Action<Player> Bonus { get; set; }

            public void GetAchievement(string achievement, Player player)
            {
                if (!player.Achievements.Contains(achievement))
                {
                    player.Achievements.Add(achievement);
                    player.SendHelp($"[{player.Achievements.Count} of {Tasks.Keys.Count} achievement{(Tasks.Keys.Count > 1 ? "s" : "")}] You unlocked a new achievement: {achievement}!");
                }

                player.Owner.BroadcastMessage(new NOTIFICATION
                {
                    ObjectId = player.Id,
                    Color = new ARGB(0xFF450000),
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + achievement + " Complete!\"}}",
                }, null);
            }
        }
    }
}