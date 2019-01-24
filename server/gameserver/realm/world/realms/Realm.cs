#region

using LoESoft.Core.models;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using LoESoft.GameServer.realm.terrain;
using LoESoft.GameServer.realm.world;
using System;

#endregion

namespace LoESoft.GameServer.realm
{
    //The mad god who look after the realm
    internal partial class Realm : IDisposable
    {
        private readonly int[] enemyCounts = new int[12];
        private readonly int[] enemyMaxCounts = new int[12];

        public readonly Random rand = new Random();
        private GameWorld world;
        public bool ClosingStarted = false;
        public bool RealmClosed = false;
        private bool disposed = false;
        private long prevTick;

        private int x;

        public Realm(GameWorld world)
        {
            this.world = world;
            UpdateHeroes();
            Init();
        }

        public void Init()
        {
            var w = world.Map.Width;
            var h = world.Map.Height;
            var stats = new int[12];
            for (var y = 0; y < h; y++)
                for (var x = 0; x < w; x++)
                {
                    var tile = world.Map[x, y];
                    if (tile.Terrain != WmapTerrain.None)
                        stats[(int)tile.Terrain - 1]++;
                }
            foreach (var i in RealmSpawnCache)
            {
                var terrain = i.WmapTerrain;
                var idx = (int)terrain - 1;
                var enemyCount = stats[idx] / i.Density;

                enemyMaxCounts[idx] = enemyCount;
                enemyCounts[idx] = 0;

                for (var j = 0; j < enemyCount; j++)
                {
                    var objType = GetRandomObjType(i.Entities);

                    if (objType == 0)
                        continue;

                    enemyCounts[idx] += HandleSpawn(GameServer.Manager.GameData.ObjectDescs[objType], terrain, w, h);

                    if (enemyCounts[idx] >= enemyCount)
                        break;
                }
            }
        }

        public void OnPlayerEntered(Player player)
        {
            player.SendInfo("Welcome to Realm of the Mad God");
            player.SendEnemy("Oryx the Mad God", "You are food for my minions!");
            player.SendInfo("Use [WASDQE] to move; click to shoot!");
            player.SendInfo("Type \"/help\" for more help");
        }

        private void EnsurePopulation()
        {
            RecalculateEnemyCount();

            int[] state = new int[12];
            int[] diff = new int[12];
            int c = 0;

            for (var i = 0; i < state.Length; i++)
            {
                if (enemyCounts[i] > enemyMaxCounts[i] * 1.5)  //Kill some
                {
                    state[i] = 1;
                    diff[i] = enemyCounts[i] - enemyMaxCounts[i];
                    c++;
                }
                else if (enemyCounts[i] < enemyMaxCounts[i] * 0.75) //Add some
                {
                    state[i] = 2;
                    diff[i] = enemyMaxCounts[i] - enemyCounts[i];
                }
                else
                    state[i] = 0;
            }

            foreach (var i in world.Enemies)    //Kill
            {
                int idx = (int)i.Value.Terrain - 1;

                if (idx == -1
                    || state[idx] == 0
                    || i.Value.GetNearestEntity(10, true) != null
                    || diff[idx] == 0)
                    continue;

                if (state[idx] == 1)
                {
                    world.LeaveWorld(i.Value);
                    diff[idx]--;
                    if (diff[idx] == 0)
                        c--;
                }

                if (c == 0)
                    break;
            }

            int w = world.Map.Width, h = world.Map.Height;

            for (int i = 0; i < state.Length; i++)  //Add
            {
                if (state[i] != 2)
                    continue;

                int x = diff[i];
                WmapTerrain t = (WmapTerrain)(i + 1);

                for (int j = 0; j < x;)
                {
                    ushort objType = GetRandomObjType(t);

                    if (objType == 0)
                        continue;

                    j += HandleSpawn(GameServer.Manager.GameData.ObjectDescs[objType], t, w, h);
                }
            }

            RecalculateEnemyCount();

            GC.Collect();
        }

        private int HandleSpawn(ObjectDesc desc, WmapTerrain terrain, int w, int h)
        {
            Entity entity;
            var ret = 0;
            var pt = new IntPoint();
            if (desc.Spawn != null)
            {
                var num = (int)GetNormal(rand, desc.Spawn.Mean, desc.Spawn.StdDev);
                if (num > desc.Spawn.Max)
                    num = desc.Spawn.Max;
                else if (num < desc.Spawn.Min)
                    num = desc.Spawn.Min;

                do
                {
                    pt.X = rand.Next(0, w);
                    pt.Y = rand.Next(0, h);
                } while (world.Map[pt.X, pt.Y].Terrain != terrain ||
                         !world.IsPassable(pt.X, pt.Y) ||
                         world.AnyPlayerNearby(pt.X, pt.Y));

                for (var k = 0; k < num; k++)
                {
                    entity = Entity.Resolve(desc.ObjectType);
                    entity.Move(
                        pt.X + (float)(rand.NextDouble() * 2 - 1) * 5,
                        pt.Y + (float)(rand.NextDouble() * 2 - 1) * 5);
                    (entity as Enemy).Terrain = terrain;
                    if (entity.GetNearestEntity(10, true) == null)
                        world.EnterWorld(entity);
                    ret++;
                }
            }
            else
            {
                do
                {
                    pt.X = rand.Next(0, w);
                    pt.Y = rand.Next(0, h);
                } while (world.Map[pt.X, pt.Y].Terrain != terrain ||
                         !world.IsPassable(pt.X, pt.Y) ||
                         world.AnyPlayerNearby(pt.X, pt.Y));

                entity = Entity.Resolve(desc.ObjectType);
                entity.Move(pt.X, pt.Y);
                (entity as Enemy).Terrain = terrain;
                world.EnterWorld(entity);
                ret++;
            }
            return ret;
        }

        public void SpawnEvent(string name, MapSetPiece setpiece)
        {
            var pt = new IntPoint();
            do
            {
                pt.X = rand.Next(0, world.Map.Width);
                pt.Y = rand.Next(0, world.Map.Height);
            } while ((world.Map[pt.X, pt.Y].Terrain < WmapTerrain.Mountains ||
                      world.Map[pt.X, pt.Y].Terrain > WmapTerrain.MidForest) ||
                     !world.IsPassable(pt.X, pt.Y) ||
                     world.AnyPlayerNearby(pt.X, pt.Y));

            pt.X -= (setpiece.Size - 1) / 2;
            pt.Y -= (setpiece.Size - 1) / 2;

            setpiece.RenderSetPiece(world, pt);

            Log.Info($"Oryx spawned '{name}' at (X: {pt.X}, Y: {pt.Y}).");
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}