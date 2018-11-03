using LoESoft.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace LoESoft.GameServer.realm.entity.player
{
    public partial class Player
    {
        public List<LootCache> ImportLootCaches()
        {
            var lootCaches = new List<LootCache>();

            try
            { lootCaches = JsonConvert.DeserializeObject<List<LootCache>>(File.ReadAllText(Path.Combine(GameServer.LootCachePath, $"lc-char.{AccountId}.{Client.Character.CharId}.json"))); }
            catch { }

            return lootCaches;
        }

        public void ExportLootCaches(List<LootCache> lootCaches)
            => File.WriteAllText(Path.Combine(GameServer.LootCachePath, $"lc-char.{AccountId}.{Client.Character.CharId}.json"), JsonConvert.SerializeObject(lootCaches));
    }
}
