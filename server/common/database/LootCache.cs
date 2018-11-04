using System.Collections.Generic;
using System.Linq;

namespace LoESoft.Core
{
    public class LootCache
    {
        public string ObjectId { get; set; }
        public int Total { get; set; }
        public int Attempts { get; set; }
        public int MaxAttempts { get; set; }
        public int AttemptsBase { get; set; }

        public class Utils
        {
            public static bool ContainsIn(List<LootCache> caches, string objectId)
                => caches.Where(cache => cache.ObjectId == objectId).ToList().Count != 0;

            public static void UpdateTotal(List<LootCache> caches, string objectId)
                => caches.Where(cache => cache.ObjectId == objectId).Select(cache =>
                {
                    cache.Total++;
                    cache.Attempts = 0;
                    cache.MaxAttempts = cache.AttemptsBase;

                    return cache;
                }).ToList();

            public static void UpdateAttempts(List<LootCache> caches, string objectId)
                => caches.Where(cache => cache.ObjectId == objectId).Select(cache => { cache.Attempts++; return cache; }).ToList();

            public static void UpdateMaxAttempts(List<LootCache> caches, string objectId, int maxAttempts)
                => caches.Where(cache => cache.ObjectId == objectId)
                    .Where(cache => cache.MaxAttempts > maxAttempts).Select(cache => { cache.MaxAttempts = maxAttempts; return cache; }).ToList();
        }
    }
}