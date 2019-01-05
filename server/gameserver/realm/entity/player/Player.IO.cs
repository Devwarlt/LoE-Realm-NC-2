using LoESoft.Core.database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace LoESoft.GameServer.realm.entity.player
{
    public partial class Player
    {
        public List<string> ImportAchivementCache()
        {
            var achievementCache = new List<string>();

            try
            {
                var path = Path.Combine(GameServer.AchievementCachePath, $"ac-char.{AccountId}.{Client.Character.CharId}.json");

                if (File.Exists(path))
                {
                    var content = File.ReadAllText(path);
                    achievementCache = JsonConvert.DeserializeObject<List<string>>(content);
                }
            }
            catch { }

            return achievementCache;
        }

        public void ExportAchievementCache(List<string> achievementCache)
        {
            if (achievementCache.Count != 0)
                try { File.WriteAllText(Path.Combine(GameServer.AchievementCachePath, $"ac-char.{AccountId}.{Client.Character.CharId}.json"), JsonConvert.SerializeObject(achievementCache)); }
                catch { }
        }

        public string ImportTaskCache()
        {
            string taskCache = null;

            try
            { taskCache = JsonConvert.DeserializeObject<string>(File.ReadAllText(Path.Combine(GameServer.TaskCachePath, $"tc-char.{AccountId}.{Client.Character.CharId}.json"))); }
            catch { }

            return taskCache;
        }

        public void ExportTaskCache(string taskCache)
        {
            if (taskCache != null)
                try
                { File.WriteAllText(Path.Combine(GameServer.TaskCachePath, $"tc-char.{AccountId}.{Client.Character.CharId}.json"), JsonConvert.SerializeObject(taskCache)); }
                catch { }
        }

        public List<MonsterCache> ImportMonsterCaches()
        {
            var monsterCaches = new List<MonsterCache>();

            try
            { monsterCaches = JsonConvert.DeserializeObject<List<MonsterCache>>(File.ReadAllText(Path.Combine(GameServer.MonsterCachePath, $"mc-char.{AccountId}.{Client.Character.CharId}.json"))); }
            catch { }

            return monsterCaches;
        }

        public void ExportMonsterCaches(List<MonsterCache> lootCaches)
        {
            if (lootCaches.Count != 0)
                try { File.WriteAllText(Path.Combine(GameServer.MonsterCachePath, $"mc-char.{AccountId}.{Client.Character.CharId}.json"), JsonConvert.SerializeObject(lootCaches)); }
                catch { }
        }
    }
}