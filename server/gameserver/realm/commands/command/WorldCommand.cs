#region

using LoESoft.Core.config;
using LoESoft.GameServer.logic;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.commands
{
    internal class GuildCommand : Command
    {
        public GuildCommand() : base("g", (int)AccountType.FREE_ACCOUNT)
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
        public TutorialCommand() : base("tutorial", (int)AccountType.FREE_ACCOUNT)
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
        public DrastaCitadelCommand() : base("drasta", (int)AccountType.VIP_ACCOUNT)
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
        public TradeCommand() : base("trade", (int)AccountType.FREE_ACCOUNT)
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
        public PauseCommand() : base("pause", (int)AccountType.FREE_ACCOUNT)
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
        public TeleportCommand() : base("teleport", (int)AccountType.FREE_ACCOUNT)
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
        public TellCommand() : base("tell", (int)AccountType.FREE_ACCOUNT)
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

            foreach (ClientData cData in GameServer.Manager.ClientManager.Values)
            {
                if (cData.Client.Account.NameChosen && cData.Client.Account.Name.EqualsIgnoreCase(playername))
                {
                    player.Client.SendMessage(new TEXT()
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Admin = 0,
                        Recipient = cData.Client.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = "",
                        TextColor = 0x123456,
                        NameColor = 0x123456
                    });

                    cData.Client.SendMessage(new TEXT()
                    {
                        ObjectId = cData.Client.Player.Owner.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Admin = 0,
                        Recipient = cData.Client.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = "",
                        TextColor = 0x123456,
                        NameColor = 0x123456
                    });
                    return true;
                }
            }
            player.SendInfo($"{playername} not found.");
            return false;
        }
    }
}