using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoESoft.GameServer.realm
{
    internal partial class Realm
    {
        public static double GetNormal(Random rand) =>
            Math.Sqrt(-2.0 * Math.Log(GetUniform(rand))) * Math.Sin(2.0 * Math.PI * GetUniform(rand));

        public static double GetNormal(Random rand, double mean, double standardDeviation) =>
            mean + standardDeviation * GetNormal(rand);

        public static double GetUniform(Random rand) =>
            ((uint)(rand.NextDouble() * uint.MaxValue) + 1.0) * 2.328306435454494e-10;

        public int CountEnemies(params string[] enemies)
        {
            var enemyList = new List<ushort>();

            foreach (var i in enemies)
            {
                try
                {
                    ushort id = 0;

                    if (GameServer.Manager.GameData.IdToObjectType.ContainsKey(i))
                        id = GameServer.Manager.GameData.IdToObjectType[i];

                    if (id != 0)
                        enemyList.Add(id);
                }
                catch { }
            }
            return world.Enemies.Count(i => enemyList.Contains(i.Value.ObjectType));
        }

        private ushort GetRandomObjType(WmapTerrain wmapTerrain)
        {
            foreach (var i in RealmSpawnCache)
            {
                if (i.WmapTerrain == wmapTerrain) // assuming only one
                {
                    GetRandomObjType(i.Entities);
                    break;
                }
            }
            return 0;
        }

        private ushort GetRandomObjType(List<KeyValuePair<string, double>> dat)
        {
            double p = rand.NextDouble();
            double n = 0;
            ushort objType = 0;

            foreach (KeyValuePair<string, double> k in dat)
            {
                n += k.Value;
                if (n > p)
                {
                    objType = GameServer.Manager.GameData.IdToObjectType[k.Key];
                    break;
                }
            }
            return objType;
        }

        private void RecalculateEnemyCount()
        {
            for (var i = 0; i < enemyCounts.Length; i++)
                enemyCounts[i] = 0;
            foreach (var i in world.Enemies)
            {
                if (i.Value.Terrain == WmapTerrain.None)
                    continue;
                enemyCounts[(int)i.Value.Terrain - 1]++;
            }
        }

        public void Tick(RealmTime time)
        {
            if (!disposed)
            {
                if (HandleHeroes())
                {
                    if (!ClosingStarted)
                        InitCloseRealm();
                }
                else
                {
                    if (time.TotalElapsedMs - prevTick > 25000)
                    {
                        if (x % 2 == 0)
                            HandleAnnouncements();
                        if (x % 6 == 0)
                            EnsurePopulation();
                        x++;
                        prevTick = time.TotalElapsedMs;
                    }
                }
            }
        }

        public void BroadcastMsg(string message) =>
            GameServer.Manager.Chat.Oryx(world, message);

        private void SendMsg(Player player, string message, string src = "") =>
            player.Client.SendMessage(new TEXT
            {
                Name = src,
                ObjectId = -1,
                Stars = -1,
                BubbleTime = 0,
                Recipient = "",
                Text = message,
                CleanText = "",
                NameColor = 0x123456,
                TextColor = 0x123456
            });

        private void HandleAnnouncements()
        {
            Tuple<string, TauntData> taunt = criticalEnemies[rand.Next(0, criticalEnemies.Length)];
            int count = 0;

            foreach (var i in world.Enemies)
            {
                ObjectDesc desc = i.Value.ObjectDesc;

                if (desc == null || (desc.DisplayId ?? desc.ObjectId) != taunt.Item1)
                    continue;

                count++;
            }

            if (count == 0)
                return;

            if (count == 1 && taunt.Item2.final != null)
            {
                string[] arr = taunt.Item2.final;
                string msg = arr[rand.Next(0, arr.Length)];

                BroadcastMsg(msg);
            }
            else
            {
                string[] arr = taunt.Item2.numberOfEnemies;
                string msg = arr[rand.Next(0, arr.Length)];

                BroadcastMsg(msg.Replace("{COUNT}", count.ToString()));
            }
        }
    }
}