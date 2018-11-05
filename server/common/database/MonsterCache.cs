namespace LoESoft.Core.database
{
    public class MonsterCache
    {
        public string ObjectId { get; set; }

        public int TaskCount { get; set; } = -1;

        public int TaskLimit { get; set; } = -1;

        public int Total { get; set; } = 0;
    }

    public class MonsterData
    {
        public string ObjectId { get; set; }
        public int Total { get; set; }
    }

    public class RewardData
    {
        public string ObjectId { get; set; }
        public int Total { get; set; }
    }
}