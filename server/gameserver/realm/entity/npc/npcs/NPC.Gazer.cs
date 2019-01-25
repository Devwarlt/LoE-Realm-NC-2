#region

using LoESoft.Core.config;
using LoESoft.Core.database;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Linq;
using static LoESoft.GameServer.realm.entity.player.Player;

#endregion

namespace LoESoft.GameServer.realm.entity.npc.npcs
{
    public class Gazer : NPC
    {
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize)
        {
            for (int i = 0; i < locations.Count; i += nSize)
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
        }

        public override void Welcome(Player player) => Callback(player, $"Hello {player.Name}! I'm Gazer, how can I help you?");

        public override void Leave(Player player, bool polite) => Callback(player, polite ? _NPCLeaveMessage.Replace("{PLAYER}", player.Name) : "How rude!");

        public override void Commands(Player player, string command)
        {
            string callback = null;
            string task = null;
            var isTask = false;

            if (command.Length >= 6)
            {
                var taskStart = command;
                var taskOriginalNames = GameTask.Tasks.Keys.ToList();
                var taskToLowerNames = taskOriginalNames.Select(names => names.ToLower()).ToList();

                if (taskStart.Contains("start "))
                    taskStart = taskStart.Substring(6);

                task = taskToLowerNames.Contains(taskStart) ? taskOriginalNames.FirstOrDefault(name => name.ToLower() == taskStart) : null;
                isTask = task != null;
            }

            if (!isTask)
                switch (command)
                {
                    #region "Uptime"
                    case "uptime":
                        {
                            TimeSpan uptime = DateTime.Now - GameServer.Uptime;
                            double thisUptime = uptime.TotalMinutes;
                            if (thisUptime <= 1)
                                callback = "Server started recently.";
                            else if (thisUptime > 1 && thisUptime <= 59)
                                callback = string.Format("Uptime: {0}{1}{2}{3}.",
                                    $"{uptime.Minutes:n0}",
                                    (uptime.Minutes >= 1 && uptime.Minutes < 2) ? " minute" : " minutes",
                                    uptime.Seconds < 1 ? "" : $" and {uptime.Seconds:n0}",
                                    uptime.Seconds < 1 ? "" : (uptime.Seconds >= 1 && uptime.Seconds < 2) ? " second" : " seconds");
                            else
                                callback = string.Format("Uptime: {0}{1}{2}{3}{4}{5}.",
                                    $"{uptime.Hours:n0}",
                                    (uptime.Hours >= 1 && uptime.Hours < 2) ? " hour" : " hours",
                                    uptime.Minutes < 1 ? "" : $", {uptime.Minutes:n0}",
                                    uptime.Minutes < 1 ? "" : (uptime.Minutes >= 1 && uptime.Minutes < 2) ? " minute" : " minutes",
                                    uptime.Seconds < 1 ? "" : $" and {uptime.Seconds:n0}",
                                    uptime.Seconds < 1 ? "" : (uptime.Seconds >= 1 && uptime.Seconds < 2) ? " second" : " seconds");
                        }
                        break;
                    #endregion
                    #region "Online"
                    case "online":
                        {
                            int serverMaxUsage = Settings.NETWORKING.MAX_CONNECTIONS;
                            int serverCurrentUsage = GameServer.Manager.GetManager.Clients.Count;
                            int worldCurrentUsage = player.Owner.Players.Keys.Count;
                            callback = $"Server: {serverCurrentUsage}/{serverMaxUsage} player{(serverCurrentUsage > 1 ? "s" : "")} | {player.Owner.Name}: {worldCurrentUsage} player{(worldCurrentUsage > 1 ? "s" : "")}.";
                        }
                        break;
                    #endregion
                    #region "Tasks"
                    case "task":
                    case "tasks":
                        callback = "Yeah! There are several tasks to do in-game and you can also receive wonderful rewards! Do you want more 'tasks info'?";
                        break;

                    case "task info":
                    case "tasks info":
                        callback = "The tasks are based in star requirement and some could be repeated. Tell me when you are 'ready' to begin any of them.";
                        break;

                    case "ready":
                        var gameTasks = SplitList(GameTask.Tasks.Keys.ToList(), 5).ToList();
                        var gameTaskNames = new List<string>();

                        foreach (var i in gameTasks)
                            gameTaskNames.Add(Utils.ToCommaSepString(i.ToArray()));

                        player.SendHelp("Tasks:");

                        foreach (var j in gameTaskNames)
                            player.SendHelp($"- {j}");

                        callback = "So far I have the following tasks, check your chat box and say the name of task to me to start. Remember, you can only handle one task per time until completed.";
                        break;

                    case "task status":
                        if (player.ActualTask == null)
                            callback = "You are in none task at this moment.";
                        else
                        {
                            var getTaskMonsters = player.MonsterCaches.Where(monster => monster.TaskLimit != -1);
                            var taskData = new List<string>();

                            foreach (var i in GameTask.Tasks[player.ActualTask].MonsterDatas)
                            {
                                var mob = getTaskMonsters.FirstOrDefault(monster => monster.ObjectId == i.ObjectId);

                                if (mob == null)
                                    taskData.Add($"- [0/{i.Total}] Defeat {i.Total} x {i.ObjectId}.");
                                else
                                    taskData.Add($"- [{mob.TaskCount}/{i.Total}] Defeat {i.Total} x {mob.ObjectId}{(mob.TaskCount >= i.Total ? " (OK!)" : "")}.");
                            }

                            player.SendHelp($"[Task] {player.ActualTask}:");

                            foreach (var j in taskData)
                                player.SendHelp(j);

                            callback = $"You are doing the '{player.ActualTask}' task and your status are displaying at chat box. You can ask for 'task reward' when you finish this task.";
                        }
                        break;

                    case "task reward":
                        if (player.ActualTask == null)
                            callback = "There is no reward for people who didn't finish any task with me. Come back here later...";
                        else
                        {
                            var currentTask = GameTask.Tasks[player.ActualTask];
                            var getTaskMonsters = player.MonsterCaches.Where(monster => monster.TaskLimit != -1);
                            var success = 0;

                            foreach (var i in currentTask.MonsterDatas)
                            {
                                var mob = getTaskMonsters.FirstOrDefault(monster => monster.ObjectId == i.ObjectId);

                                if (mob == null)
                                    continue;

                                if (mob.TaskCount >= i.Total)
                                    success++;
                            }

                            if (success == currentTask.MonsterDatas.Count)
                            {
                                foreach (var i in currentTask.MonsterDatas)
                                {
                                    var mob = getTaskMonsters.FirstOrDefault(monster => monster.ObjectId == i.ObjectId);

                                    if (mob == null)
                                        continue;

                                    mob.TaskCount = mob.TaskLimit = -1;
                                }

                                var gifts = player.Client.Account.Gifts.ToList();

                                for (var i = 0; i < currentTask.RewardDatas.Count; i++)
                                    for (var j = 0; j < currentTask.RewardDatas[i].Total; j++)
                                        gifts.Add(GameServer.Manager.GameData.IdToObjectType[currentTask.RewardDatas[i].ObjectId]);

                                callback = $"Congratulations {player.Name}! You have finished the task '{player.ActualTask}'.";

                                currentTask.Bonus?.Invoke(player);
                                currentTask.GetAchievement(player.ActualTask, player);

                                player.Client.Account.Gifts = gifts.ToArray();
                                player.Client.Account.FlushAsync();
                                player.Client.Account.Reload();
                                player.ActualTask = null;
                                player.MonsterCaches.Clear();
                                player.SaveToCharacter();
                            }
                            else
                                callback = "You didn't finish your task properly, ask me for 'task status' for more details.";
                        }
                        break;
                    #endregion
                    #region "Help"
                    case "help":
                        callback = "You can ask me about 'uptime', 'online', 'tasks', 'check bless', 'bless' and 'fame' for more details.";
                        break;
                    #endregion
                    #region "Access Dream Island"
                    case "dream island":
                        callback = "Do you want to access Dream Island? If you want then say 'access dream island' to proceed.";
                        break;

                    case "access dream island":
                        RemovePlayer(player);
                        Callback(player, command, false); // player only (self)
                        Leave(player, true);

                        player.Client.Reconnect(new networking.outgoing.RECONNECT()
                        {
                            Host = "",
                            Port = Settings.GAMESERVER.PORT,
                            GameId = (int)WorldID.DREAM_ISLAND,
                            Name = "Dream Island",
                            Key = Empty<byte>.Array,
                        });
                        return;
                    #endregion
                    #region "Event: max"
                    case "max":
                        var eventmax = new DateTime(2019, 1, 18, 12, 59, 59);

                        if (DateTime.UtcNow > eventmax)
                            callback = "The event already over, try again later.";
                        else
                        {
                            player.Stats[0] = player.ObjectDesc.MaxHitPoints;
                            player.Stats[1] = player.ObjectDesc.MaxMagicPoints;
                            player.Stats[2] = player.ObjectDesc.MaxAttack;
                            player.Stats[3] = player.ObjectDesc.MaxDefense;
                            player.Stats[4] = player.ObjectDesc.MaxSpeed;
                            player.Stats[5] = player.ObjectDesc.MaxHpRegen;
                            player.Stats[6] = player.ObjectDesc.MaxMpRegen;
                            player.Stats[7] = player.ObjectDesc.MaxDexterity;
                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You have been maxed!";
                        }
                        break;
                    #endregion
                    #region "Event: vip"
                    case "vip":
                        var eventvip = new DateTime(2019, 1, 18, 12, 59, 59);

                        if (DateTime.UtcNow > eventvip)
                            callback = "The event already over, try again later.";
                        else
                        {
                            if (player.AccountType != (int)AccountType.REGULAR)
                                callback = "You already have VIP perks.";
                            else
                            {
                                var _outgoing = new List<Message>();
                                var _world = GameServer.Manager.GetWorld(player.Owner.Id);
                                var acc = player.Client.Account;
                                var days = 7;

                                var _notification = new NOTIFICATION
                                {
                                    Color = new ARGB(0xFFFFFF),
                                    ObjectId = player.Id,
                                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Success!\"}}"
                                };

                                _outgoing.Add(_notification);

                                var _showeffect = new SHOWEFFECT
                                {
                                    Color = new ARGB(0xffddff00),
                                    EffectType = EffectType.Nova,
                                    PosA = new Position { X = 2 }
                                };

                                _outgoing.Add(_showeffect);

                                player.Owner.BroadcastMessage(_outgoing, null);

                                acc.AccountLifetime = DateTime.UtcNow;
                                acc.AccountLifetime = acc.AccountLifetime.AddDays(days);
                                acc.AccountType = (int)AccountType.VIP;
                                acc.FlushAsync();
                                acc.Reload();

                                player.UpdateCount++;

                                player.SendInfo("Reconnecting...");

                                var _reconnect = new RECONNECT
                                {
                                    GameId = (int)WorldID.NEXUS_ID, // change to Drasta Citadel in future versions!
                                    Host = string.Empty,
                                    Key = Empty<byte>.Array,
                                    Name = "Nexus",
                                    Port = Settings.GAMESERVER.PORT
                                };

                                _world.Timers.Add(new WorldTimer(2000, (w, t) => player.Client.Reconnect(_reconnect)));

                                callback = $"Success! You received {days} day{(days > 1 ? "s" : "")} as account lifetime to your VIP account type along event!";
                            }
                        }
                        break;
                    #endregion
                    #region "Blessings"
                    case "check bless":
                    case "check blessings":
                        callback = $"You have {player.CountBlessings()} of 5 blessings activated.";
                        break;

                    case "bless":
                    case "blessings":
                        player.SendHelp("[Blessings]:");
                        player.SendHelp("- Medusa's Bless.");
                        player.SendHelp("- Ghost's Bless.");
                        player.SendHelp("- Slime's Bless.");
                        player.SendHelp("- Beholder's Bless.");
                        player.SendHelp("- Ent's Bless.");

                        callback = "Oh you know about blessings! I can sell 5 different blessings that can protect your against evil forces. Check your chat log.";
                        break;

                    case "medusa's bless":
                    case "ghost's bless":
                    case "slime's bless":
                    case "beholder's bless":
                    case "ent's bless":
                        callback = $"That blessing is costing for your level {player.GetBlessingPrice()} Fame. Say \"buy {command}\" to confirm purchase.";
                        break;

                    case "buy medusa's bless":
                        if (player.Bless1)
                            callback = "You already have this bless.";
                        else if (player.Client.Account.Fame >= player.GetBlessingPrice())
                        {
                            player.Bless1 = true;
                            player.CurrentFame -= player.GetBlessingPrice();

                            GameServer.Manager.Database.UpdateFame(player.Client.Account, player.GetBlessingPrice());

                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You received the blessings of Medusa God!";
                        }
                        else
                            callback = "You do not have enought Fame to purchase this blessing.";
                        break;

                    case "buy ghost's bless":
                        if (player.Bless2)
                            callback = "You already have this bless.";
                        else if (player.Client.Account.Fame >= player.GetBlessingPrice())
                        {
                            player.Bless2 = true;
                            player.CurrentFame -= player.GetBlessingPrice();

                            GameServer.Manager.Database.UpdateFame(player.Client.Account, player.GetBlessingPrice());

                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You received the blessings of Ghost God!";
                        }
                        else
                            callback = "You do not have enought Fame to purchase this blessing.";
                        break;

                    case "buy slime's bless":
                        if (player.Bless3)
                            callback = "You already have this bless.";
                        else if (player.Client.Account.Fame >= player.GetBlessingPrice())
                        {
                            player.Bless3 = true;
                            player.CurrentFame -= player.GetBlessingPrice();

                            GameServer.Manager.Database.UpdateFame(player.Client.Account, player.GetBlessingPrice());

                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You received the blessings of Slime God!";
                        }
                        else
                            callback = "You do not have enought Fame to purchase this blessing.";
                        break;

                    case "buy beholder's bless":
                        if (player.Bless4)
                            callback = "You already have this bless.";
                        else if (player.Client.Account.Fame >= player.GetBlessingPrice())
                        {
                            player.Bless4 = true;
                            player.CurrentFame -= player.GetBlessingPrice();

                            GameServer.Manager.Database.UpdateFame(player.Client.Account, player.GetBlessingPrice());

                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You received the blessings of Beholder God!";
                        }
                        else
                            callback = "You do not have enought Fame to purchase this blessing.";
                        break;

                    case "buy ent's bless":
                        if (player.Bless5)
                            callback = "You already have this bless.";
                        else if (player.Client.Account.Fame >= player.GetBlessingPrice())
                        {
                            player.Bless5 = true;
                            player.CurrentFame -= player.GetBlessingPrice();

                            GameServer.Manager.Database.UpdateFame(player.Client.Account, player.GetBlessingPrice());

                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You received the blessings of Ent God!";
                        }
                        else
                            callback = "You do not have enought Fame to purchase this blessing.";
                        break;
                    #endregion
                    #region "Wipe Fame"
                    case "fame":
                        callback = "I can transfer your fame base to your account if you want, just say 'wipe fame'.";
                        break;

                    case "wipe fame":
                        if (player.Fame == 0 || player.FakeExperience == 0)
                            callback = "You cannot use this feature yet, try again later.";
                        else if (player.Fame < 400)
                            callback = "You can only transfer fame to your account when you get 400 fame base.";
                        else
                        {
                            player.FakeExperience = 0;
                            player.CalculateFame(false);
                            player.CurrentFame += player.Fame;

                            GameServer.Manager.Database.UpdateFame(player.Client.Account, player.Fame);

                            player.SaveToCharacter();
                            player.UpdateCount++;

                            callback = "You wipe your fame base of your character and transfered to your account!";
                        }
                        break;
                    #endregion
                    case "hi":
                    case "hello":
                    case "hey":
                    case "good morning":
                    case "good afternoon":
                    case "good evening":
                        Callback(player, command, false); // player only (self)
                        NoRepeat(player);
                        return;

                    case "bye":
                    case "good bye":
                    case "good night":
                        RemovePlayer(player);
                        Callback(player, command, false); // player only (self)
                        Leave(player, true);
                        return;

                    default:
                        callback = "Sorry, I don't understand. Say 'help' for more details.";
                        break;
                }
            else
            {
                if (!command.ToLower().Contains("start ")) // task details
                {
                    var starrequirement = GameTask.Tasks[task].StarsRequirement;

                    if (player.Stars >= starrequirement)
                        callback = $"You seems prepared to initialize this task, just say 'start {task}' to begin!";
                    else
                        callback = $"You are too young at this moment to do '{task}' task. Come back here later when you get {starrequirement} star{(starrequirement > 1 ? "s" : "")}...";
                }
                else // task start
                {
                    var getTask = GameTask.Tasks[task];

                    if (player.ActualTask == null)
                    {
                        if (player.Achievements.Contains(task) && !getTask.MultipleTimes)
                            callback = $"You cannot do '{task}' task again.";
                        else
                        {
                            if (player.Stars >= getTask.StarsRequirement)
                            {
                                var monsterCaches = new List<MonsterCache>();

                                foreach (var i in getTask.MonsterDatas)
                                    monsterCaches.Add(new MonsterCache()
                                    {
                                        ObjectId = i.ObjectId,
                                        TaskCount = 0,
                                        TaskLimit = i.Total,
                                        Total = 0
                                    });

                                foreach (var j in monsterCaches)
                                {
                                    var mob = player.MonsterCaches.FirstOrDefault(monster => monster.ObjectId == j.ObjectId);

                                    if (mob == null)
                                        player.MonsterCaches.Add(j);
                                    else
                                    {
                                        mob.TaskCount = j.TaskCount;
                                        mob.TaskLimit = j.TaskLimit;
                                    }
                                }

                                player.ActualTask = task;
                                player.Task = getTask;
                                player.UpdateCount++;
                                player.SaveToCharacter();

                                callback = $"You have successfully initialized '{task}' task and all I want to say is good luck! Come back here and ask me for 'task reward' when you finish your task!";
                            }
                            else
                                callback = $"You are too young at this moment to do '{task}' task. Come back here later...";
                        }
                    }
                    else
                    {
                        if (player.ActualTask != null)
                            callback = $"You are already in '{player.ActualTask}' task and cannot begin another one until you finish yours. Also, you can ask me for 'task status' to show your progress.";
                        else
                            callback = $"I could give you permission to start this '{task}' task, but I'm gonna?";
                    }
                }
            }
            Callback(player, command, false); // player only (self)
            Callback(player, callback); // to NPC
            ChatManager.ChatDataCache.Remove(player.Name); // Removing player from chat data cache.
        }
    }
}