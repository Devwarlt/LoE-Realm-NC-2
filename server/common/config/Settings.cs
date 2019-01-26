using System;
using System.Collections.Generic;

namespace LoESoft.Core.config
{
    public partial class Settings
    {
        public enum ServerMode
        {
            Local,
            Production
        }

        public static readonly double EVENT_RATE = 3;
        public static readonly DateTime EVENT_OVER = new DateTime(2019, 1, 27, 23, 59, 59);

        public static readonly string EVENT_MESSAGE = $"The server is hosting an event with " +
            $"+{(GetEventRate() - (GetEventRate() != 1 ? 1 : 0)) * 100}% EXP, stats EXP and loot drop rate. Enjoy it until " +
            $"{EVENT_OVER.ToString("MM/dd/yyyy hh:mm tt")} UTC!";

        public static readonly ServerMode SERVER_MODE = ServerMode.Local;
        public static readonly bool ENABLE_RESTART_SYSTEM = SERVER_MODE == ServerMode.Production;
        public static readonly int RESTART_DELAY_MINUTES = 90;
        public static readonly int RESTART_APPENGINE_DELAY_MINUTES = 30;
        public static readonly DateTimeKind DateTimeKind = DateTimeKind.Utc;

        public static double GetEventRate() => DateTime.UtcNow > EVENT_OVER ? 1 : EVENT_RATE;

        public static readonly List<string> ALLOWED_LOCAL_DNS = new List<string>
        {
            "::1", "localhost", "127.0.0.1", "loe-nc.servegame.com"
        };

        public static class STARTUP
        {
            public static readonly int GOLD = 40;
            public static readonly int FAME = 0;
            public static readonly int TOTAL_FAME = 0;
            public static readonly int TOKENS = 0;
            public static readonly int EMPIRES_COIN = 0;
            public static readonly int MAX_CHAR_SLOTS = 2;
            public static readonly int IS_AGE_VERIFIED = 1;
            public static readonly bool VERIFIED = true;
        }

        public static readonly List<GameVersion> GAME_VERSIONS = new List<GameVersion>
        {
            new GameVersion(Version: "2.0", Allowed: false),
            new GameVersion(Version: "2.1", Allowed: false),
            new GameVersion(Version: "2.2", Allowed: false),
            new GameVersion(Version: "2.3", Allowed: false),
            new GameVersion(Version: "2.4", Allowed: false),
            new GameVersion(Version: "2.5", Allowed: false),
            new GameVersion(Version: "2.5.1", Allowed: false),
            new GameVersion(Version: "2.5.2", Allowed: false),
            new GameVersion(Version: "3.0", Allowed: false),
            new GameVersion(Version: "3.0.1", Allowed: false),
            new GameVersion(Version: "3.1", Allowed: false),
            new GameVersion(Version: "3.2", Allowed: false),
            new GameVersion(Version: "3.2.1", Allowed: false),
            new GameVersion(Version: "3.2.2", Allowed: false),
            new GameVersion(Version: "3.2.3", Allowed: false),
            new GameVersion(Version: "3.2.4", Allowed: false),
            new GameVersion(Version: "3.2.5", Allowed: false),
            new GameVersion(Version: "3.2.6", Allowed: false),
            new GameVersion(Version: "3.2.7", Allowed: false),
            new GameVersion(Version: "3.2.8", Allowed: false),
            new GameVersion(Version: "3.2.8.1", Allowed: true)
        };
    }
}