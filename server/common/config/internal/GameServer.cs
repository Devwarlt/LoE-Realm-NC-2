namespace LoESoft.Core.config
{
    public partial class Settings
    {
        public static class GAMESERVER
        {
            public static string TITLE = "[LoESoft] (New Chicago) LoE Realm - GameServer";
            public static string FILE = ProcessFile("gameserver");
            public static int GAME_PORT = 5556;
            public static int TICKETS_PER_SECOND = 5;
            public static int MAX_IN_REALM = NETWORKING.MAX_CONNECTIONS / 3;
            public static int TTL = 5;
        }
    }
}
