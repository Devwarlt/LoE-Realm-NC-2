#region

using LoESoft.GameServer.realm.entity.player;
using log4net;
using System;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.commands
{
    internal static class Outgoing
    {
        internal static string command { get; set; }
    }

    public abstract class Command
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Command));

        public Command(string name, int permLevel = 0)
        {
            CommandName = name;
            PermissionLevel = permLevel;
        }

        public string CommandName { get; private set; }
        public int PermissionLevel { get; private set; }

        protected abstract bool Process(Player player, RealmTime time, string[] args);

        private static int GetPermissionLevel(Player player) => player.Client.Account.AccountType;

        public bool HasPermission(Player player)
        {
            if (GetPermissionLevel(player) < PermissionLevel)
                return false;
            return true;
        }

        public bool Execute(Player player, RealmTime time, string args)
        {
            if (!HasPermission(player))
            {
                player.SendInfo($"Unknown command: {Outgoing.command}.");
                return false;
            }

            try
            {
                string[] a = args.Split(' ');
                return Process(player, time, a);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error when executing the command.\nError: {0}", ex);
                return false;
            }
        }
    }

    public class CommandManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommandManager));

        private readonly Dictionary<string, Command> cmds;

        public CommandManager()
        {
            cmds = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

            var t = typeof(Command);

            foreach (var i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    var instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }
        }

        public IDictionary<string, Command> Commands
        {
            get { return cmds; }
        }

        public bool Execute(Player player, RealmTime time, string text)
        {
            int index = text.IndexOf(' ');
            string cmd = text.Substring(1, index == -1 ? text.Length - 1 : index - 1);
            string args = index == -1 ? "" : text.Substring(index + 1);

            if (!cmds.TryGetValue(cmd, out Command command))
            {
                player.SendInfo($"Unknown command: {text}");
                return false;
            }
            Outgoing.command = text;
            log.InfoFormat("[Command] <{0}> {1}", player.Name, text);
            return command.Execute(player, time, args);
        }
    }
}