#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm.commands
{
    internal class DonatorAnnounce : Command
    {
        public DonatorAnnounce() : base("dannounce", (int)AccountType.VIP_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /dannounce <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);

            foreach (ClientData cData in GameServer.Manager.ClientManager.Values)
            {
                cData.Client.SendMessage(new TEXT
                {
                    BubbleTime = 10,
                    Stars = player.Stars,
                    Name = player.Name,
                    Text = " " + saytext,
                    NameColor = 0xFFFFFF,
                    TextColor = 0xFFFFFF
                });
            }
            return true;
        }
    }

    internal class RealmCommand : Command
    {
        public RealmCommand()
            : base("realm", (int)AccountType.VIP_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            World world = player.Client.Manager.Monitor.GetRandomRealm();

            player.Client.Reconnect(new RECONNECT
            {
                Host = "",
                Port = Settings.GAMESERVER.PORT,
                GameId = world.Id,
                Name = world.Name,
                Key = world.PortalKey,
            });
            return true;
        }
    }

    internal class AutoTradeCommand : Command
    {
        public AutoTradeCommand() : base("autotrade", (int)AccountType.DEM_ACCOUNT)
        { }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Player plr = GameServer.Manager.FindPlayer(args[0]);
            if (plr != null && plr.Owner == player.Owner)
            {
                player.RequestTrade(time, new networking.incoming.REQUESTTRADE { Name = plr.Name });
                plr.RequestTrade(time, new networking.incoming.REQUESTTRADE { Name = player.Name });
                return true;
            }
            return true;
        }
    }

    internal class VaultCommand : Command
    {
        public VaultCommand() : base("vault", (int)AccountType.VIP_ACCOUNT)
        { }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault)
            {
                player.SendHelp("You are already in Vault...");
            }
            player.Client.Reconnect(new RECONNECT()
            {
                Host = "",
                Port = Settings.GAMESERVER.PORT,
                GameId = GameServer.Manager.PlayerVault(player.Client).Id,
                Name = GameServer.Manager.PlayerVault(player.Client).Name,
                Key = GameServer.Manager.PlayerVault(player.Client).PortalKey
            });
            return true;
        }
    }

    internal class VisitCommand : Command
    {
        public VisitCommand() : base("visit", (int)AccountType.GM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length < 1)
            {
                player.SendHelp("Usage: /visit <player>");
                return false;
            }

            foreach (ClientData i in GameServer.Manager.ClientManager.Values)
            {
                Player target = i.Client.Player;

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

    internal class GlandCommand : Command
    {
        public GlandCommand() : base("glands", (int)AccountType.FREE_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!(player.Owner is IRealm))
            {
                player.SendInfo("You can only use this command at realm.");
                return false;
            }

            player.Move(1000f, 1000f);
            player.Owner.BroadcastMessage(new GOTO
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.X,
                    Y = player.Y
                }
            }, null);
            player.UpdateCount++;

            return true;
        }
    }

    internal class Summon : Command
    {
        public Summon()
            : base("summon", (int)AccountType.CM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault)
            {
                player.SendInfo($"You cant summon player {args[0]} in your vault.");
                return false;
            }

            foreach (ClientData i in GameServer.Manager.ClientManager.Values)
            {
                Player target = i.Client.Player;

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

                    i.Client.SendMessage(msg);

                    return true;
                }
            }

            player.SendError($"An error occurred: player '{args[0]}' couldn't be found.");
            return false;
        }
    }

    internal class PosCmd : Command
    {
        public PosCmd() : base("p", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo("X: " + (int)player.X + " - Y: " + (int)player.Y);
            return true;
        }
    }

    internal class AddRealmCommand : Command
    {
        public AddRealmCommand() : base("addrealm", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            GameServer.Manager.AddWorld(GameWorld.AutoName(1, true));
            return true;
        }
    }

    internal class SpawnCommand : Command
    {
        public SpawnCommand() : base("spawn", (int)AccountType.DEM_ACCOUNT)
        {
        }

        private List<string> Whitelist = new List<string>()
        {
            "1"
        };

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!Whitelist.Contains(player.AccountId))
            {
                player.SendInfo($"Unknown command: /spawn.");
                return false;
            }

            if (args.Length > 0 && int.TryParse(args[0], out int num)) //multi
            {
                var name = string.Join(" ", args.Skip(1).ToArray());
                var icdatas = new Dictionary<string, ushort>(GameServer.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);

                if (!icdatas.TryGetValue(name, out ushort objType) ||
                    !GameServer.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Unknown entity!");
                    return false;
                }

                var c = int.Parse(args[0]);

                for (var i = 0; i < num; i++)
                {
                    Entity entity = Entity.Resolve(objType);
                    entity.Move(player.X, player.Y);
                    player.Owner.EnterWorld(entity);
                }

                player.SendInfo("Success!");
            }
            else
            {
                var name = string.Join(" ", args);
                var icdatas = new Dictionary<string, ushort>(GameServer.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);

                if (!icdatas.TryGetValue(name, out ushort objType) ||
                    !GameServer.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                    return false;
                }

                var entity = Entity.Resolve(objType);
                entity.Move(player.X, player.Y);
                player.Owner.EnterWorld(entity);
            }

            return true;
        }
    }

    internal class AddEffCommand : Command
    {
        public AddEffCommand() : base("addeff", (int)AccountType.DEM_ACCOUNT)
        {
        }

        private List<string> Whitelist = new List<string>()
        {
            "1"
        };

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!Whitelist.Contains(player.AccountId))
            {
                player.SendInfo($"Unknown command: /addeff.");
                return false;
            }

            if (args.Length == 0)
            {
                player.SendHelp("Usage: /addeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = -1
                });
                {
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class RemoveEffCommand : Command
    {
        public RemoveEffCommand() : base("remeff", (int)AccountType.DEM_ACCOUNT)
        {
        }

        private List<string> Whitelist = new List<string>()
        {
            "1"
        };

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!Whitelist.Contains(player.AccountId))
            {
                player.SendInfo($"Unknown command: /remeff.");
                return false;
            }

            if (args.Length == 0)
            {
                player.SendHelp("Usage: /remeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = 0
                });
                player.SendInfo("Success!");
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class GiveCommand : Command
    {
        public GiveCommand() : base("give", (int)AccountType.DEM_ACCOUNT)
        {
        }

        private List<string> Blacklist = new List<string>()
        {
            "admin sword", "admin wand", "admin staff", "admin dagger", "admin bow", "admin katana", "crown",
            "public arena key"
        };

        private List<string> Whitelist = new List<string>()
        {
            "1"
        };

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!Whitelist.Contains(player.AccountId))
            {
                player.SendInfo($"Unknown command: /give.");
                return false;
            }

            if (args.Length == 0)
            {
                player.SendHelp("Usage: /give <item name>");
                return false;
            }

            var name = string.Join(" ", args.ToArray()).Trim();

            if (Blacklist.Contains(name.ToLower()) && player.AccountType != (int)AccountType.DEM_ACCOUNT)
            {
                player.SendHelp($"You cannot give '{name}', access denied.");
                return false;
            }

            try
            {
                var icdatas = new Dictionary<string, ushort>(GameServer.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);

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

    internal class TpCommand : Command
    {
        public TpCommand() : base("tp", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Usage: /tp <X coordinate> <Y coordinate>");
            }
            else
            {
                int x, y;
                try
                {
                    x = int.Parse(args[0]);
                    y = int.Parse(args[1]);
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return false;
                }
                player.Move(x + 0.5f, y + 0.5f);
                player.UpdateCount++;
                player.Owner.BroadcastMessage(new GOTO
                {
                    ObjectId = player.Id,
                    Position = new Position
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
            return true;
        }
    }

    internal class KillAll : Command
    {
        public KillAll() : base("killAll", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var iterations = 0;
            var lastKilled = -1;
            var killed = 0;

            var mobName = args.Aggregate((s, a) => string.Concat(s, " ", a));
            while (killed != lastKilled)
            {
                lastKilled = killed;
                foreach (var i in player.Owner.Enemies.Values
                    .Where(e => e.ObjectDesc.ObjectId != null && e.ObjectDesc.ObjectId.ContainsIgnoreCase(mobName) && !e.IsPet && e.ObjectDesc.Enemy))
                {
                    i.CheckDeath = true;
                    killed++;
                }
                if (++iterations >= 5)
                    break;
            }

            player.SendInfo($"{killed / 5} enemy killed!");
            return true;
        }
    }

    internal class Kick : Command
    {
        public Kick() : base("kick", (int)AccountType.CM_ACCOUNT)
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
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
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
        public Max() : base("max", (int)AccountType.GM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                var target = args[0];

                if (!string.IsNullOrEmpty(target) && player.AccountType >= (int)AccountType.CM_ACCOUNT)
                {
                    if (target == "all" && player.AccountType == (int)AccountType.DEM_ACCOUNT)
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

    internal class OryxSay : Command
    {
        public OryxSay() : base("osay", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /oryxsay <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);
            player.SendEnemy("Oryx the Mad God", saytext);
            return true;
        }
    }

    internal class OnlineCommand : Command
    {
        public OnlineCommand() : base("online", (int)AccountType.GM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("Online at this moment: ");

            foreach (KeyValuePair<int, World> w in GameServer.Manager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                {
                    Player[] copy = world.Players.Values.ToArray();
                    if (copy.Length != 0)
                    {
                        for (int i = 0; i < copy.Length; i++)
                        {
                            sb.Append(copy[i].Name);
                            sb.Append(", ");
                        }
                    }
                }
            }
            string fixedString = sb.ToString().TrimEnd(',', ' '); //clean up trailing ", "s

            player.SendInfo(fixedString + ".");
            return true;
        }
    }

    internal class Announcement : Command
    {
        public Announcement() : base("announce", (int)AccountType.CM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);

            foreach (ClientData cData in GameServer.Manager.ClientManager.Values)
            {
                cData.Client.SendMessage(new TEXT
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@ANNOUNCEMENT",
                    Text = " " + saytext,
                    NameColor = 0x123456,
                    TextColor = 0x123456
                });
            }
            return true;
        }
    }

    internal class KillPlayerCommand : Command
    {
        public KillPlayerCommand() : base("kill", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var found = false;

            foreach (ClientData cData in GameServer.Manager.ClientManager.Values)
                if (cData.Client.Account.Name.EqualsIgnoreCase(args[0]))
                {
                    cData.Client.Player.HP = 0;
                    cData.Client.Player.Death(player.Name);

                    player.SendInfo($"Player {cData.Client.Account.Name} has been killed!");

                    found = true;
                    break;
                }

            if (!found)
            {
                player.SendInfo(string.Format("Player '{0}' could not be found!", args));
                return false;
            }
            else
                return true;
        }
    }

    internal class RestartCommand : Command
    {
        public RestartCommand() : base("restart", (int)AccountType.CM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (KeyValuePair<int, World> w in GameServer.Manager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                    world.BroadcastMessage(new TEXT
                    {
                        Name = "@ANNOUNCEMENT",
                        Stars = -1,
                        BubbleTime = 0,
                        Text = $"Server restarting soon by {(player.AccountType == (int)AccountType.CM_ACCOUNT ? "CM" : "D&M")} {player.Name}. Please be ready to disconnect.",
                        NameColor = 0x123456,
                        TextColor = 0x123456
                    }, null);
            }

            Thread.Sleep(4 * 1000);

            GameServer.SafeRestart();

            return true;
        }
    }

    internal class TqCommand : Command
    {
        public TqCommand() : base("tq", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Quest == null)
            {
                player.SendInfo("There is no quest to teleport!");
                return false;
            }
            player.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            player.UpdateCount++;
            player.Owner.BroadcastMessage(new GOTO
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.Quest.X,
                    Y = player.Quest.Y
                }
            }, null);
            player.SendInfo("Success!");
            return true;
        }
    }

    internal class LevelCommand : Command
    {
        public LevelCommand() : base("level", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Usage: /level <ammount>");
                    return false;
                }

                if (args.Length == 1)
                {
                    player.Client.Character.Level = (int.Parse(args[0]) >= 1 && int.Parse(args[0]) <= 20) ? int.Parse(args[0]) : player.Client.Character.Level;
                    player.Client.Player.Level = (int.Parse(args[0]) >= 1 && int.Parse(args[0]) <= 20) ? int.Parse(args[0]) : player.Client.Player.Level;
                    player.SendInfo(string.Format("Success! Level changed from level {0} to level {1}!",
                        player.Client.Player.Level, (int.Parse(args[0]) >= 1 && int.Parse(args[0]) <= 20) ? int.Parse(args[0]) : player.Client.Player.Level));
                    player.UpdateCount++;
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class SetpieceCommand : Command
    {
        public SetpieceCommand() : base("setpiece", (int)AccountType.DEM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                MapSetPiece piece = (MapSetPiece)Activator.CreateInstance(System.Type.GetType(
                    "LoESoft.GameServer.realm.mapsetpiece." + args[0], true, true));
                piece.RenderSetPiece(player.Owner, new IntPoint((int)player.X + 1, (int)player.Y + 1));
                return true;
            }
            catch
            {
                player.SendError("Invalid SetPiece.");
                return false;
            }
        }
    }

    internal class ListCommands : Command
    {
        public ListCommands() : base("commands", (int)AccountType.GM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var cmds = new Dictionary<string, Command>();
            var t = typeof(Command);

            foreach (var i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }

            var sb = new StringBuilder("");
            var copy = cmds.Values.ToArray();

            for (var i = 0; i < copy.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");

                sb.Append(copy[i].CommandName);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }

    internal class Mute : Command
    {
        public Mute() : base("mute", (int)AccountType.GM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Client.Manager.Database.MuteAccount(i.Value.Client.Account);
                        player.SendInfo("Player Muted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot mute!");
                return false;
            }
            return true;
        }
    }

    internal class Unmute : Command
    {
        public Unmute() : base("unmute", (int)AccountType.GM_ACCOUNT)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = false;
                        i.Value.Client.Manager.Database.UnmuteAccount(i.Value.Client.Account);
                        player.SendInfo("Player Unmuted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot unmute!");
                return false;
            }
            return true;
        }
    }

    internal class BanCommand : Command
    {
        public BanCommand() : base("ban", (int)AccountType.GM_ACCOUNT)
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

                player.Client.Manager.Database.BanAccount(int.Parse(args[0]));
                player.SendInfo("Player has been banned!");
                return true;
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
        public UnBanCommand() : base("unban", (int)AccountType.GM_ACCOUNT)
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

                player.Client.Manager.Database.UnBanAccount(int.Parse(args[0]));
                player.SendInfo("Player has been unbanned!");
                return true;
            }
            catch
            {
                player.SendError("Cannot unban!");
                return false;
            }
        }
    }
}