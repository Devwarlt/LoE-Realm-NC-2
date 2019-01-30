using System.Collections.Generic;

namespace LoESoft.Core.config
{
    public partial class Settings
    {
        public static class NETWORKING
        {
            public static readonly byte[] INCOMING_CIPHER = ProcessToken("14FCA055AB3BDAFAB31174283CC1D478");
            public static readonly byte[] OUTGOING_CIPHER = ProcessToken("3E1C0DBEA4BECA433D0925498F1F4170");
            public static readonly string APPENGINE_URL = "https://loesoft-games.github.io"; //"http://appengine.loesoft.org";
            public static readonly int CPU_HANDLER = 4096;
            public static readonly int MAX_CONNECTIONS = 100;
            public static readonly bool DISABLE_NAGLES_ALGORITHM = SERVER_MODE != ServerMode.Local;

            public static class RESTART
            {
                public static readonly bool ENABLE_RESTART = ENABLE_RESTART_SYSTEM;
                public static readonly int RESTART_DELAY_MINUTES = Settings.RESTART_DELAY_MINUTES;
                public static readonly int RESTART_APPENGINE_DELAY_MINUTES = Settings.RESTART_APPENGINE_DELAY_MINUTES;
            }

            public static class INTERNAL
            {
                public static readonly List<string> PRODUCTION_DDNS = new List<string>
                {
                    "loe-nc.servegame.com"
                };
            }
        }
    }
}