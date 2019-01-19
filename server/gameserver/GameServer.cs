#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.realm;
using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer
{
    internal static class Empty<T>
    {
        public static T[] Array = new T[0];
    }

    internal static class GameServer
    {
        public static DateTime Uptime { get; private set; }

        public static readonly ILog log = LogManager.GetLogger("Server");

        private static readonly ManualResetEvent Shutdown = new ManualResetEvent(false);

        public static int GameUsage { get; private set; }

        public static bool AutoRestart { get; private set; }

        public static ChatManager Chat { get; set; }

        public static RealmManager Manager;

        public static DateTime WhiteListTurnOff { get; private set; }

        public static string LootCachePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "/loe-nc-2/loots/");
        public static string MonsterCachePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "/loe-nc-2/monsters/");
        public static string TaskCachePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "/loe-nc-2/tasks/");
        public static string AchievementCachePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "/loe-nc-2/achievements/");

        private static void Main(string[] args)
        {
            Console.Title = "Loading...";

            XmlConfigurator.ConfigureAndWatch(new FileInfo("_gameserver.config"));

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            if (!Directory.Exists(LootCachePath)) Directory.CreateDirectory(LootCachePath);
            if (!Directory.Exists(MonsterCachePath)) Directory.CreateDirectory(MonsterCachePath);
            if (!Directory.Exists(TaskCachePath)) Directory.CreateDirectory(TaskCachePath);
            if (!Directory.Exists(AchievementCachePath)) Directory.CreateDirectory(AchievementCachePath);

            try
            {
                var db = new Database();
                GameUsage = -1;

                Manager = new RealmManager(db);

                AutoRestart = Settings.NETWORKING.RESTART.ENABLE_RESTART;

                Manager.Initialize();
                Manager.Run();

                Log._("Message", Message.Messages.Count);

                var server = new Server();
                var policy = new PolicyServer();

                Console.CancelKeyPress += (sender, e) => e.Cancel = true;

                Settings.DISPLAY_SUPPORTED_VERSIONS();

                Log.Info("Initializing GameServer...");

                policy.Start();
                server.Start();

                if (AutoRestart)
                {
                    Chat = Manager.Chat;
                    Uptime = DateTime.Now;
                    Restart();
                }

                Console.Title = Settings.GAMESERVER.TITLE;

                Log.Info("Initializing GameServer... OK!");

                Console.CancelKeyPress += delegate
                {
                    Shutdown?.Set();
                };

                while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                    ;

                Log.Info("Terminating...");

                server?.Stop();
                policy?.Stop();
                Manager?.Stop();
                Manager?.Database.Dispose();
                Shutdown?.Dispose();

                Log.Warn("Terminated GameServer.");

                Thread.Sleep(1000);

                Environment.Exit(0);
            }
            catch (Exception e) { ForceShutdown(e); }
        }

        private static int ToMiliseconds(int minutes) => minutes * 60 * 1000;

        public static void Restart(Task task = null) => ForceShutdown();

        public async static void ForceShutdown(Exception ex = null)
        {
            if (ex != null)
            {
                log.Error(ex);

                await Task.Delay(2 * 1000);
            }

            Process.Start(Settings.GAMESERVER.FILE);

            Environment.Exit(0);
        }

        public static void SafeRestart()
        {
            var message = "Server is now offline.";

            Log.Warn(message);

            try
            {
                Manager.ClientManager.Values.Where(j => j.Client != null).Select(k =>
                {
                    k.Client.Player?.SendInfo(message);
                    return k;
                }).ToList();
            }
            catch (Exception ex) { ForceShutdown(ex); }

            Thread.Sleep(5 * 1000);

            try
            {
                AccessDenied = true;

                Manager.ClientManager.Values.Where(j => j.Client != null).Select(k =>
                {
                    Manager.TryDisconnect(k.Client, DisconnectReason.RESTART);
                    return k;
                }).ToList();
            }
            catch (Exception ex) { ForceShutdown(ex); }

            Thread.Sleep(1 * 1000);

            Process.Start(Settings.GAMESERVER.FILE);

            Environment.Exit(0);
        }

        public static void Restart()
        {
            var timer = new System.Timers.Timer(ToMiliseconds((Settings.NETWORKING.RESTART.RESTART_DELAY_MINUTES <= 5 ? 6 : Settings.NETWORKING.RESTART.RESTART_DELAY_MINUTES) - 5))
            { AutoReset = false };
            timer.Elapsed += delegate
            {
                string message = null;
                int i = 5;

                do
                {
                    message = $"Server will be restarted in {i} minute{(i <= 1 ? "" : "s")}.";

                    Log.Info(message);

                    try
                    {
                        foreach (var cData in Manager.ClientManager.Values)
                            cData.Client.Player?.SendInfo(message);
                    }
                    catch (Exception ex) { ForceShutdown(ex); }

                    Thread.Sleep(ToMiliseconds(1));

                    i--;
                } while (i != 0);

                message = "Server is now offline.";

                Log.Warn(message);

                try
                {
                    Manager.ClientManager.Values.Where(j => j.Client != null).Select(k =>
                    {
                        k.Client.Player?.SendInfo(message);
                        return k;
                    }).ToList();
                }
                catch (Exception ex) { ForceShutdown(ex); }

                Thread.Sleep(5 * 1000);

                try
                {
                    AccessDenied = true;

                    Manager.ClientManager.Values.Where(j => j.Client != null).Select(k =>
                    {
                        Manager.TryDisconnect(k.Client, DisconnectReason.RESTART);
                        return k;
                    }).ToList();
                }
                catch (Exception ex) { ForceShutdown(ex); }

                Thread.Sleep(1 * 1000);

                Process.Start(Settings.GAMESERVER.FILE);

                Environment.Exit(0);
            };
            timer.Start();
        }

        public static void Stop(Task task = null)
        {
            if (task != null)
                log.Error(task.Exception.InnerException);

            Shutdown.Set();
        }
    }
}