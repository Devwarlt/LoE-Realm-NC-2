#region

using LoESoft.Core.config;
using LoESoft.GameServer.logic;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace LoESoft.GameServer.realm.commands
{
    internal class GuildCommand : Command
    {
        public GuildCommand() : base("g", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.NameChosen)
            {
                player.SendError("Choose a name!");
                return false;
            }

            if (string.IsNullOrEmpty(player.Guild))
            {
                player.SendError("You need to be in a guild to guild chat.");
                return false;
            }

            GameServer.Manager.Chat.Guild(player, string.Join(" ", args));
            return true;
        }
    }

    internal class TutorialCommand : Command
    {
        public TutorialCommand() : base("tutorial", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new RECONNECT
            {
                Host = "",
                Port = Settings.GAMESERVER.PORT,
                GameId = (int)WorldID.TUT_ID,
                Name = "Tutorial",
                Key = Empty<byte>.Array,
            });
            return true;
        }
    }

    internal class DrastaCitadelCommand : Command
    {
        public DrastaCitadelCommand() : base("drasta", (int)AccountType.VIP)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new RECONNECT
            {
                Host = "",
                Port = Settings.GAMESERVER.PORT,
                GameId = (int)WorldID.DRASTA_CITADEL_ID,
                Name = "Drasta Citadel",
                Key = Empty<byte>.Array,
            });
            return true;
        }
    }

    internal class TradeCommand : Command
    {
        public TradeCommand() : base("trade", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                player.SendInfo("Usage: /trade <player name>");
                return false;
            }
            player.RequestTrade(time, new REQUESTTRADE
            {
                Name = args[0]
            });
            return true;
        }
    }

    internal class PauseCommand : Command
    {
        public PauseCommand() : base("pause", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var whitelist = new List<string> { "nexus", "vault" };

            if (!whitelist.Contains(player.Owner.Name.ToLower()))
            {
                player.SendInfo("You cannot pause here.");
                return false;
            }

            if (player.HasConditionEffect(ConditionEffectIndex.Paused))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = 0
                });
                player.SendInfo("Game resumed.");
            }
            else
            {
                var allowedPlaces = new List<int>()
                {
                    (int)WorldID.NEXUS_ID,
                    (int)WorldID.VAULT_ID
                };

                if (!allowedPlaces.Contains(player.Owner.Id))
                {
                    player.SendInfo("You cannot pause here!");
                    return false;
                }

                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = -1
                });
                player.SendInfo("Game paused.");
            }
            return true;
        }
    }

    internal class TeleportCommand : Command
    {
        public TeleportCommand() : base("teleport", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (string.Equals(player.Name.ToLower(), args[0].ToLower()))
                {
                    player.SendInfo("You are already at yourself, and always will be!");
                    return false;
                }

                foreach (var i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.ApplyConditionEffect(ConditionEffectIndex.Invulnerable, 2000);
                        player.Teleport(time, new TELEPORT { ObjectId = i.Value.Id });
                        return true;
                    }
                }
                player.SendInfo(string.Format("Cannot teleport, {0} not found!", args[0].Trim()));
            }
            catch { player.SendHelp("Usage: /teleport <player name>"); }
            return false;
        }
    }

    internal class TellCommand : Command
    {
        public TellCommand() : base("tell", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.NameChosen)
            {
                player.SendError("Choose a name!");
                return false;
            }

            if (args.Length < 2)
            {
                player.SendHelp("Usage: /tell <player name> <text>");
                return false;
            }

            var playername = args[0].Trim();
            var msg = string.Join(" ", args, 1, args.Length - 1);

            if (string.Equals(player.Name.ToLower(), playername.ToLower()))
            {
                player.SendInfo("Quit telling yourself!");
                return false;
            }

            if (string.Join(" ", args, 0, 1).ToLower() == "npc")
            {
                var npcName = $"NPC {Utils.FirstCharToUpper(string.Join(" ", args, 1, 1).ToLower())}";

                if (NPCs.Database.ContainsKey($"NPC {Utils.FirstCharToUpper(string.Join(" ", args, 1, 1).ToLower())}"))
                {
                    var npcMsg = args.Length > 2 ? string.Join(" ", args.Skip(2)) : null;
                    var npc = NPCs.Database.ContainsKey(npcName) ? NPCs.Database[npcName] : null;

                    if (npcMsg == null)
                    {
                        player.SendInfo($"Send a valid message to NPC {Utils.FirstCharToUpper(string.Join(" ", args, 1, 1).ToLower())}.");
                        return false;
                    }
                    else
                    {
                        if (npc == null)
                        {
                            player.SendInfo($"Oh! {npcName} found in database but not declared yet, try again later.");
                            return false;
                        }
                        else
                        {
                            if (!npc.ReturnPlayersCache().Contains(player.Name))
                            {
                                player.SendInfo($"You need to initialize conversation with {npcName}.");
                                return false;
                            }
                            else
                            {
                                npc.Commands(player, npcMsg.ToLower()); // handle all commands, redirecting to properly NPC instance
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    player.SendInfo($"There is no {npcName} found in our database.");
                    return false;
                }
            }

            try
            {
                foreach (var client in GameServer.Manager.GetManager.Clients.Values)
                {
                    if (client.Account.NameChosen && client.Account.Name.EqualsIgnoreCase(playername))
                    {
                        player.Client.SendMessage(new TEXT()
                        {
                            ObjectId = player.Id,
                            BubbleTime = 10,
                            Stars = player.Stars,
                            Name = player.Name,
                            Admin = 0,
                            Recipient = client.Account.Name,
                            Text = msg,
                            CleanText = "",
                            TextColor = 0x123456,
                            NameColor = 0x123456
                        });

                        client.SendMessage(new TEXT()
                        {
                            ObjectId = client.Player.Owner.Id,
                            BubbleTime = 10,
                            Stars = player.Stars,
                            Name = player.Name,
                            Admin = 0,
                            Recipient = client.Account.Name,
                            Text = msg,
                            CleanText = "",
                            TextColor = 0x123456,
                            NameColor = 0x123456
                        });
                        return true;
                    }
                }
            }
            catch { }

            player.SendInfo($"{playername} not found.");
            return false;
        }
    }

    internal class GlobalChat : Command
    {
        public GlobalChat() : base("gchat", (int)AccountType.VIP)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /gchat <saytext>");
                return false;
            }

            var saytext = string.Join(" ", args);

            if (player.Stars >= 14 || player.AccountType != (int)AccountType.REGULAR)
                foreach (var client in GameServer.Manager.GetManager.Clients.Values)
                    client.SendMessage(new TEXT()
                    {
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Text = " " + saytext,
                        NameColor = 0xFFFFFF,
                        TextColor = 0xFFFFFF
                    });
            else
            {
                player.SendHelp("You need at least 14 stars to unlock the global chat feature, try again later.");
                return false;
            }

            return true;
        }
    }

    internal class RealmCommand : Command
    {
        public RealmCommand()
            : base("realm", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is IRealm)
                if ((player.Owner as GameWorld).IsRealmClosed)
                {
                    player.SendInfo("Too late! You cannot join realm when its closing.");
                    return false;
                }

            var client = player.Client;

            if (player.AccountType == (int)AccountType.REGULAR)
            {
                if (client.RealmRegularEntryMS == -1)
                    client.RealmRegularEntryMS = GameServer.Manager.Logic.GameTime.TotalElapsedMs;
                else
                {
                    if (GameServer.Manager.Logic.GameTime.TotalElapsedMs - client.RealmRegularEntryMS > 30000)
                        client.CanRealm = true;
                    else
                        client.CanRealm = false;
                }
            }
            else
            {
                if (client.RealmVIPEntryMS == -1)
                    client.RealmVIPEntryMS = GameServer.Manager.Logic.GameTime.TotalElapsedMs;
                else
                {
                    if (GameServer.Manager.Logic.GameTime.TotalElapsedMs - client.RealmVIPEntryMS > 10000)
                        client.CanRealm = true;
                    else
                        client.CanRealm = false;
                }
            }

            if (!client.CanRealm)
            {
                var isfree = player.AccountType == (int)AccountType.REGULAR;
                var elapsed = (isfree ? 30 : 10) - (GameServer.Manager.Logic.GameTime.TotalElapsedMs
                    - (isfree ? client.RealmRegularEntryMS : client.RealmVIPEntryMS)) / 1000;

                player.SendInfo($"{elapsed} second{(elapsed > 1 ? "s" : "")} remains to use '/realm' command.");
                return false;
            }

            var world = GameServer.Manager.Monitor.GetRandomRealm();

            if (world.IsFull)
            {
                player.SendError("{\"key\":\"server.dungeon_full\"}");
                return false;
            }

            if (player.Stars >= 14 || player.AccountType != (int)AccountType.REGULAR)
                player.Client.Reconnect(new RECONNECT()
                {
                    Host = "",
                    Port = Settings.GAMESERVER.PORT,
                    GameId = world.Id,
                    Name = world.Name,
                    Key = world.PortalKey,
                });
            else
            {
                player.SendHelp("You need at least 14 stars to unlock the realm instant access feature, try again later.");
                return false;
            }

            return true;
        }
    }

    internal class VaultCommand : Command
    {
        public VaultCommand() : base("vault", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault)
            {
                player.SendInfo("You are already at vault.");
                return false;
            }

            if (player.Stars >= 14 || player.AccountType != (int)AccountType.REGULAR)
                player.Client.Reconnect(new RECONNECT()
                {
                    Host = "",
                    Port = Settings.GAMESERVER.PORT,
                    GameId = GameServer.Manager.PlayerVault(player.Client).Id,
                    Name = GameServer.Manager.PlayerVault(player.Client).Name,
                    Key = GameServer.Manager.PlayerVault(player.Client).PortalKey
                });
            else
            {
                player.SendHelp("You need at least 14 stars to unlock the vault instant access feature, try again later.");
                return false;
            }

            return true;
        }
    }

    internal class GlandCommand : Command
    {
        public GlandCommand() : base("glands", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!(player.Owner is IRealm))
            {
                player.SendInfo("You can only use this command at realm.");
                return false;
            }

            var client = player.Client;

            if (player.AccountType == (int)AccountType.REGULAR)
            {
                if (client.GlandsRegularEntryMS == -1)
                    client.GlandsRegularEntryMS = GameServer.Manager.Logic.GameTime.TotalElapsedMs;
                else
                {
                    if (GameServer.Manager.Logic.GameTime.TotalElapsedMs - client.GlandsRegularEntryMS > 30000)
                        client.CanGlands = true;
                    else
                        client.CanGlands = false;
                }
            }
            else
            {
                if (client.GlandsVIPEntryMS == -1)
                    client.GlandsVIPEntryMS = GameServer.Manager.Logic.GameTime.TotalElapsedMs;
                else
                {
                    if (GameServer.Manager.Logic.GameTime.TotalElapsedMs - client.GlandsVIPEntryMS > 10000)
                        client.CanGlands = true;
                    else
                        client.CanGlands = false;
                }
            }

            if (!client.CanGlands)
            {
                var isfree = player.AccountType == (int)AccountType.REGULAR;
                var elapsed = (isfree ? 30 : 10) - (GameServer.Manager.Logic.GameTime.TotalElapsedMs
                    - (isfree ? client.GlandsRegularEntryMS : client.GlandsVIPEntryMS)) / 1000;

                player.SendInfo($"{elapsed} second{(elapsed > 1 ? "s" : "")} remains to use '/glands' command.");
                return false;
            }

            if (player.Stars >= 14 || player.AccountType != (int)AccountType.REGULAR)
            {
                player.Move(1478.5f, 1086.5f);
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
            }
            else
            {
                player.SendHelp("You need at least 14 stars to unlock the god lands instant access feature, try again later.");
                return false;
            }

            return true;
        }
    }

    internal class OnlineCommand : Command
    {
        public OnlineCommand() : base("online", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var sb = new StringBuilder("Online at this moment: ");
            var playersonline = 0;

            foreach (var name in GameServer.Manager.GetManager.GetPlayersName())
            {
                sb.Append(name);
                sb.Append(", ");
                playersonline++;
            }

            if (player.AccountType >= (int)AccountType.MOD)
            {
                var fixedString = sb.ToString().TrimEnd(',', ' ');

                player.SendInfo(fixedString + ".");
            }

            player.SendInfo($"There {(playersonline > 1 ? "are" : "is")} {playersonline} player{(playersonline > 1 ? "s" : "")} online.");

            return true;
        }
    }

    internal class ListCommands : Command
    {
        public ListCommands() : base("commands", (int)AccountType.REGULAR)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var cmds = new Dictionary<string, Command>();
            var t = typeof(Command);

            foreach (var i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    var instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }

            var sb = new StringBuilder("");
            var copy = cmds.Values.ToArray();

            for (var i = 0; i < copy.Length; i++)
            {
                if (player.AccountType >= copy[i].PermissionLevel)
                    sb.Append((i != 0 ? ", " : "") + copy[i].CommandName);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }

    internal class PetAttackCommand : Command
    {
        public PetAttackCommand() : base("petattack", (int)AccountType.VIP)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length != 1)
            {
                player.SendHelp("Usage: /petattack <on/off>");
                return false;
            }

            var onoff = (args[0]).ToLower();

            if (onoff != "on" && onoff != "off")
            {
                player.SendInfo("Use labels 'on' or 'off'.");
                return false;
            }

            player.SendInfo($"Pet attack is {onoff}.");
            player.EnablePetAttack = onoff == "on";
            player.SaveToCharacter();
            player.UpdateCount++;

            return true;
        }
    }
}