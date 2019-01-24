using LoESoft.Core.config;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LoESoft.GameServer.realm.commands
{
    public class TestingCommands : Command
    {
        public TestingCommands() : base("test", (int)AccountType.DEVELOPER)
        {
        }

        private readonly bool AllowTestingCommands = true;

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Production)
            {
                player.SendInfo("You cannot use this feature along Production mode.");
                return false;
            }

            if (!AllowTestingCommands)
            {
                player.SendInfo("Testing commands disabled.");
                return false;
            }

            var cmd = string.Join(" ", args, 1, args.Length - 1);

            switch (args[0].Trim())
            {
                case "chatdata":
                    {
                        // returns only your chat data
                        if (cmd == "my")
                            player.SendInfo($"[ChatData] [{ChatManager.ChatDataCache[player.Name].Item1}] <{player.Name}> {ChatManager.ChatDataCache[player.Name].Item2}");

                        if (cmd == "all")
                            foreach (KeyValuePair<string, Tuple<DateTime, string>> messageInfos in ChatManager.ChatDataCache)
                                player.SendInfo($"[ChatData] [{ChatManager.ChatDataCache[messageInfos.Key].Item1}] <{messageInfos.Key}> {ChatManager.ChatDataCache[messageInfos.Key].Item2}");
                    }
                    break;

                case "projectiles":
                    {
                        if (cmd == "ids")
                            foreach (var i in player.Owner.Projectiles.Keys)
                                player.SendInfo($"[Projectiles] [Player ID: {i.ProjectileOwner.Id} / Projectile ID: {i.ProjectileId}]");

                        if (cmd == "all")
                            foreach (var i in player.Owner.Projectiles.Keys)
                                player.SendInfo($"[Projectiles] [Player ID: {i.ProjectileOwner.Id} / Projectile ID: {i.ProjectileId} / Damage: {i.Damage}]");
                    }
                    break;

                case "id":
                    {
                        if (cmd == "mine")
                            player.SendInfo($"Your ID is: {player.Id}");

                        if (cmd == "pet" && player.Pet != null)
                            player.SendInfo($"Your Pet ID is: {player.Pet.Id}");
                        else
                            player.SendInfo($"You don't have any pet yet.");
                    }
                    break;

                case "exp":
                    {
                        if (int.TryParse(cmd, out int result))
                        {
                            player.Experience += result;
                            player.FakeExperience += result;

                            do
                            {
                                Thread.Sleep(1000);

                                if (player == null)
                                    break;
                            }
                            while (player.CheckLevelUp(false));
                        }
                        else
                            player.SendInfo("Use numeric values.");
                        break;
                    }

                default:
                    player.SendHelp("Available testing commands: 'chatdata' (my / all), 'projectiles' (ids / all), 'id' (mine / pet) and 'exp'.");
                    break;
            }
            return true;
        }
    }
}