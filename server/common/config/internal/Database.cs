namespace LoESoft.Core.config
{
    public partial class Settings
    {
        public static class REDIS_DATABASE
        {
            public static readonly string HOST = "localhost";
            public static readonly int PORT = 6379;
            public static readonly int IO_TIMEOUT = 5000;
            public static readonly string PASSWORD = "";
            public static readonly int MAX_UNSENT = int.MaxValue;
            public static readonly bool ALLOW_ADMIN = false;
            public static readonly int SYNC_TIMEOUT = 5000;
        }
    }
}