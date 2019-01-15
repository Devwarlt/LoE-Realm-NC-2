#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using log4net.Config;
using System;
using System.IO;

#endregion

namespace LoESoft.AppEngine
{
    internal class AppEngine
    {
        public static bool Restart
        { get { return Settings.NETWORKING.RESTART.ENABLE_RESTART; } }

        public static string Message
        { get; private set; }

        public delegate bool WebSocketDelegate();

        internal static Database Database
        { get; set; }

        internal static EmbeddedData GameData
        { get; set; }

        //internal static ItemDataManager ItemDataManager
        //{ get; set; }

        internal static ISManager Manager
        { get; set; }

        internal static string InstanceId
        { get; set; }

        internal static AppEngineManager AppEngineManager
        { get; set; }

        private static void Main(string[] args)
        {
            Message = null;

            Message = "Loading...";

            Console.Title = Message;

            XmlConfigurator.ConfigureAndWatch(new FileInfo("_appengine.config"));

            Database = new Database();
            GameData = new EmbeddedData();

            InstanceId = Guid.NewGuid().ToString();

            Manager = new ISManager();
            Manager.Run();

            Log.Info("Initializing AppEngine...");

            AppEngineManager = new AppEngineManager(Restart);
            AppEngineManager.Start();

            Console.Title = Settings.APPENGINE.TITLE;

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                ;

            Database.Dispose();

            AppEngineManager._shutdown = true;

            Log.Warn("Terminating AppEngine, disposing all instances.");

            var webSocketIAsyncResult = new WebSocketDelegate(() => true).BeginInvoke(new AsyncCallback(AppEngineManager.SafeDispose), null);
            webSocketIAsyncResult.AsyncWaitHandle.WaitOne(5000, true);
        }
    }
}