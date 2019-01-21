#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm.commands
{
    internal class ArenaCommand : Command
    {
        public ArenaCommand() : base("arena", (int)AccountType.DEVELOPER)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Production)
            {
                player.SendInfo("You cannot use this feature along Production mode.");
                return false;
            }

            Entity prtal = Entity.Resolve("Undead Lair Portal");
            prtal.Move(player.X, player.Y);
            player.Owner.EnterWorld(prtal);
            World w = GameServer.Manager.GetWorld(player.Owner.Id);
            w.Timers.Add(new WorldTimer(30 * 1000, (world, t) =>
            {
                try
                {
                    w.LeaveWorld(prtal);
                }
                catch
                {
                    Console.Out.WriteLine("F.");
                }
            }));
            foreach (var client in GameServer.Manager.GetManager.Clients.Values)
                client?.SendMessage(new TEXT
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = "Arena Opened"
                });
            foreach (var client in GameServer.Manager.GetManager.Clients.Values)
                client?.SendMessage(new NOTIFICATION
                {
                    Color = new ARGB(0xff00ff00),
                    ObjectId = player.Id,
                    Text = "Arena Opened"
                });
            return true;
        }
    }

    internal class VisitCommand : Command
    {
        public VisitCommand() : base("visit", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length < 1)
            {
                player.SendHelp("Usage: /visit <player>");
                return false;
            }

            foreach (var i in GameServer.Manager.GetManager.Clients.Values)
            {
                var target = i.Player;

                if (target.Owner is Vault)
                {
                    player.SendInfo($"Player {target.Name} is at Vault.");
                    return false;
                }

                if (target.Name.EqualsIgnoreCase(args[0]))
                {
                    if (player.Owner == target.Owner)
                    {
                        player.SendInfo($"You are already in the same world as this player {target.Name}.");
                        return false;
                    }

                    player.Client.Reconnect(new RECONNECT
                    {
                        GameId = target.Owner.Id,
                        Host = "",
                        IsFromArena = false,
                        Key = target.Owner.PortalKey,
                        KeyTime = -1,
                        Name = target.Owner.Name,
                        Port = -1
                    });
                    return true;
                }
            }

            player.SendError($"An error occurred: player {args[0]} couldn't be found.");
            return false;
        }
    }

    internal class Summon : Command
    {
        public Summon()
            : base("summon", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault)
            {
                player.SendInfo($"You cant summon player {args[0]} in your vault.");
                return false;
            }

            foreach (var i in GameServer.Manager.GetManager.Clients.Values)
            {
                var target = i.Player;

                if (target.Name.EqualsIgnoreCase(args[0]))
                {
                    Message msg;

                    if (target.Owner == player.Owner)
                    {
                        target.Move(player.X, player.Y);

                        msg = new GOTO
                        {
                            ObjectId = target.Id,
                            Position = new Position(player.X, player.Y)
                        };

                        target.UpdateCount++;

                        player.SendInfo($"Player {target.Name} was moved to near you.");
                    }
                    else
                    {
                        msg = new RECONNECT
                        {
                            GameId = player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = player.Owner.PortalKey,
                            KeyTime = -1,
                            Name = player.Owner.Name,
                            Port = -1
                        };

                        player.SendInfo($"Player {target.Name} is connecting to {player.Owner.Name}.");
                    }

                    i.SendMessage(msg);

                    return true;
                }
            }

            player.SendError($"An error occurred: player '{args[0]}' couldn't be found.");
            return false;
        }
    }

    internal class PosCmd : Command
    {
        public PosCmd() : base("pos", (int)AccountType.DEVELOPER)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo("X: " + player.X + " - Y: " + player.Y);
            return true;
        }
    }

    internal class SpawnCommand : Command
    {
        public SpawnCommand() : base("spawn", (int)AccountType.DEVELOPER)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Production)
            {
                player.SendInfo("You cannot use this feature along Production mode.");
                return false;
            }

            if (args.Length > 0 && int.TryParse(args[0], out int num)) //multi
            {
                string name = string.Join(" ", args.Skip(1).ToArray());
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
                    GameServer.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out ushort objType) ||
                    !GameServer.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Unknown entity!");
                    return false;
                }
                int c = int.Parse(args[0]);
                for (int i = 0; i < num; i++)
                {
                    Entity entity = Entity.Resolve(objType);
                    entity.Move(player.X, player.Y);
                    player.Owner.EnterWorld(entity);
                }
                player.SendInfo("Success!");
            }
            else
            {
                string name = string.Join(" ", args);
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
                    GameServer.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out ushort objType) ||
                    !GameServer.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                    return false;
                }
                Entity entity = Entity.Resolve(objType);
                entity.Move(player.X, player.Y);
                player.Owner.EnterWorld(entity);
            }
            return true;
        }
    }

    internal class GiveCommand : Command
    {
        public GiveCommand() : base("give", (int)AccountType.MOD)
        {
        }

        private List<string> Blacklist = new List<string>
        {
            "admin sword", "admin wand", "admin staff", "admin dagger", "admin bow", "admin katana", "crown",
            "public arena key"
        };

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Production)
            {
                player.SendInfo("You cannot use this feature along Production mode.");
                return false;
            }

            if (args.Length == 0)
            {
                player.SendHelp("Usage: /give <item name>");
                return false;
            }

            string name = string.Join(" ", args.ToArray()).Trim();

            if (Blacklist.Contains(name.ToLower()) && player.AccountType <= (int)AccountType.DEVELOPER)
            {
                player.SendHelp($"You cannot give '{name}', access denied.");
                return false;
            }

            try
            {
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(GameServer.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);

                if (!icdatas.TryGetValue(name, out ushort objType))
                {
                    player.SendError("Unknown type!");
                    return false;
                }

                if (!GameServer.Manager.GameData.Items[objType].Secret || player.Client.Account.Admin)
                {
                    for (int i = 4; i < player.Inventory.Length; i++)
                        if (player.Inventory[i] == null)
                        {
                            player.Inventory[i] = GameServer.Manager.GameData.Items[objType];
                            player.UpdateCount++;
                            player.SaveToCharacter();
                            player.SendInfo("Success!");
                            break;
                        }
                }
                else
                {
                    player.SendError("An error occurred: inventory out of space, item cannot be given.");
                    return false;
                }
            }
            catch (KeyNotFoundException)
            {
                player.SendError($"An error occurred: item '{name}' doesn't exist in game assets.");
                return false;
            }

            return true;
        }
    }

    internal class Kick : Command
    {
        public Kick() : base("kick", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kick <playername>");
                return false;
            }
            try
            {
                foreach (var i in player.Owner.Players)
                {
                    if (i.Value.AccountType >= player.AccountType)
                    {
                        player.SendInfo("You cannot kick someone with same account type or greater than yours!");
                        break;
                    }

                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.SendInfo($"Player {i.Value.Name} has been disconnected!");
                        GameServer.Manager.TryDisconnect(i.Value.Client, DisconnectReason.PLAYER_KICK);
                        break;
                    }
                }
            }
            catch
            {
                player.SendError("Cannot kick!");
                return false;
            }
            return true;
        }
    }

    internal class Max : Command
    {
        public Max() : base("max", (int)AccountType.DEVELOPER)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Production)
            {
                player.SendInfo("You cannot use this feature along Production mode.");
                return false;
            }

            try
            {
                var target = args[0];

                if (!string.IsNullOrEmpty(target) && player.AccountType >= (int)AccountType.DEVELOPER)
                {
                    if (target == "all" && player.AccountType == (int)AccountType.DEVELOPER)
                    {
                        var count = 0;

                        player.Owner.Players.Values.ToList().Where(players => players != null).Where(players =>
                            !(players.Stats[0] == players.ObjectDesc.MaxHitPoints && players.Stats[1] == players.ObjectDesc.MaxMagicPoints &&
                            players.Stats[2] == players.ObjectDesc.MaxAttack && players.Stats[3] == players.ObjectDesc.MaxDefense &&
                            players.Stats[4] == players.ObjectDesc.MaxSpeed && players.Stats[5] == players.ObjectDesc.MaxHpRegen &&
                            players.Stats[6] == players.ObjectDesc.MaxMpRegen && players.Stats[7] == players.ObjectDesc.MaxDexterity)).Select(players =>
                        {
                            players.Stats[0] = players.ObjectDesc.MaxHitPoints;
                            players.Stats[1] = players.ObjectDesc.MaxMagicPoints;
                            players.Stats[2] = players.ObjectDesc.MaxAttack;
                            players.Stats[3] = players.ObjectDesc.MaxDefense;
                            players.Stats[4] = players.ObjectDesc.MaxSpeed;
                            players.Stats[5] = players.ObjectDesc.MaxHpRegen;
                            players.Stats[6] = players.ObjectDesc.MaxMpRegen;
                            players.Stats[7] = players.ObjectDesc.MaxDexterity;
                            players.SaveToCharacter();
                            players.UpdateCount++;

                            if (player.Name != players.Name)
                                players.SendInfo($"You were maxed to 8/8 by {player.Name}.");
                            else
                                players.SendInfo("You maxed yourself!");

                            count++;

                            return players;
                        }).ToList();

                        player.SendInfo($"You maxed {count} player{(count > 1 ? "s" : "")} from world '{player.Owner.Name}'.");
                    }
                    else
                    {
                        var otherPlayer = player.Owner.Players.Values.FirstOrDefault(tplayer => tplayer.Name.ToLower() == target.ToLower());

                        if (otherPlayer == null)
                            player.SendInfo("Player not found.");
                        else
                        {
                            otherPlayer.Stats[0] = otherPlayer.ObjectDesc.MaxHitPoints;
                            otherPlayer.Stats[1] = otherPlayer.ObjectDesc.MaxMagicPoints;
                            otherPlayer.Stats[2] = otherPlayer.ObjectDesc.MaxAttack;
                            otherPlayer.Stats[3] = otherPlayer.ObjectDesc.MaxDefense;
                            otherPlayer.Stats[4] = otherPlayer.ObjectDesc.MaxSpeed;
                            otherPlayer.Stats[5] = otherPlayer.ObjectDesc.MaxHpRegen;
                            otherPlayer.Stats[6] = otherPlayer.ObjectDesc.MaxMpRegen;
                            otherPlayer.Stats[7] = otherPlayer.ObjectDesc.MaxDexterity;
                            otherPlayer.SaveToCharacter();
                            otherPlayer.UpdateCount++;

                            player.SendInfo($"You maxed the player {otherPlayer.Name}!");
                        }
                    }

                    return true;
                }

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
                player.SendInfo("You maxed yourself!");
            }
            catch { player.SendError("An error occurred while maxing stats..."); }

            return true;
        }
    }

    internal class Announcement : Command
    {
        public Announcement() : base("announce", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <saytext>");
                return false;
            }

            var saytext = string.Join(" ", args);
            var rank = "";

            switch ((AccountType)player.AccountType)
            {
                case AccountType.MOD: rank = "Mod"; break;
                case AccountType.DEVELOPER: rank = "Dev"; break;
                case AccountType.ADMIN: rank = "Admin"; break;
                default: break;
            }

            foreach (var client in GameServer.Manager.GetManager.Clients.Values)
                client.SendMessage(new TEXT
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@ANNOUNCEMENT",
                    Text = $" {rank} {player.Name} says: " + saytext,
                    NameColor = 0x123456,
                    TextColor = 0x123456
                });

            return true;
        }
    }

    internal class GetAccountIdCommand : Command
    {
        public GetAccountIdCommand() : base("getid", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                player.SendHelp("Usage: /getid <player>");
                return false;
            }

            var id = GameServer.Manager.Database.ResolveId(args[0]);
            var acc = GameServer.Manager.Database.GetAccountById(id);

            if (id == null)
            {
                player.SendInfo($"Player '{args[0]}' not found!");
                return false;
            }

            player.SendInfo($"Account ID of player '{acc.Name ?? args[0]}' is: {id}.");
            return true;
        }
    }

    internal class SendCurrencyCommand : Command
    {
        public SendCurrencyCommand() : base("send", (int)AccountType.ADMIN)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.AccountId != "1" || player.Name != "Devwarlt")
            {
                player.SendHelp("Only Devwarlt can use this feature.");
                return false;
            }

            if (args.Length != 3)
            {
                player.SendHelp("Usage: /send <currency> <amount> <accountId>");
                return false;
            }

            var currency = args[0];
            var amount = int.Parse(args[1]);
            var accid = args[2];
            var acc = GameServer.Manager.Database.GetAccountById(accid);

            if (acc == null)
            {
                player.SendInfo($"Account ID '{accid}' was not found!");
                return false;
            }

            player.SendInfo($"Success! You deposited '{amount}' {(currency == "coin" ? "empires coins" : currency)} to '{acc.Name}' account!");

            switch (currency)
            {
                case "fame": GameServer.Manager.Database.UpdateFame(acc, amount); break;
                case "gold": GameServer.Manager.Database.UpdateCredit(acc, amount); break;
                case "fortune": GameServer.Manager.Database.UpdateTokens(acc, amount); break;
                case "coin": GameServer.Manager.Database.UpdateEmpiresCoin(acc, amount); break;
                default: player.SendHelp($"Currency '{currency}' doesn't exist!"); return false;
            }

            return true;
        }
    }

    internal class RestartCommand : Command
    {
        public RestartCommand() : base("restart", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (var w in GameServer.Manager.Worlds)
            {
                var world = w.Value;
                var rank = string.Empty;

                switch ((AccountType)player.AccountType)
                {
                    case AccountType.MOD: rank = "Mod"; break;
                    case AccountType.DEVELOPER: rank = "Developer"; break;
                    case AccountType.ADMIN: rank = "Admin"; break;
                    default: return false;
                }

                if (w.Key != 0)
                    world.BroadcastMessage(new TEXT
                    {
                        Name = "@ANNOUNCEMENT",
                        Stars = -1,
                        BubbleTime = 0,
                        Text = $"Server restarting soon by {rank} {player.Name}. Please be ready to disconnect.",
                        NameColor = 0x123456,
                        TextColor = 0x123456
                    }, null);
            }

            Thread.Sleep(4 * 1000);

            GameServer.SafeRestart();

            return true;
        }
    }

    internal class BanCommand : Command
    {
        public BanCommand() : base("ban", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Usage: /ban <id>");
                    return false;
                }

                if (GameServer.Manager.Database.BanAccount(GameServer.Manager.Database, args[0]))
                {
                    player.SendInfo("Player has been banned!");
                    return true;
                }

                player.SendInfo("Cannon find account!");
                return false;
            }
            catch
            {
                player.SendError("Cannot ban!");
                return false;
            }
        }
    }

    internal class UnBanCommand : Command
    {
        public UnBanCommand() : base("unban", (int)AccountType.MOD)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Usage: /unban <id>");
                    return false;
                }

                if (GameServer.Manager.Database.UnBanAccount(GameServer.Manager.Database, args[0]))
                {
                    player.SendInfo("Player has been unbanned!");
                    return true;
                }

                player.SendInfo("Cannon find account!");
                return false;
            }
            catch
            {
                player.SendError("Cannot unban!");
                return false;
            }
        }
    }
}