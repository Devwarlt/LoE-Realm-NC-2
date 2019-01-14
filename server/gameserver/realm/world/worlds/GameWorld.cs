#region

using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using log4net;
using System;
using System.Threading;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public interface IRealm { }

    internal class GameWorld : World, IDungeon, IRealm
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GameWorld));

        private readonly int mapId;
        private readonly bool oryxPresent;
        private string displayname;

        public GameWorld(int mapId, string name, bool oryxPresent)
        {
            displayname = name;
            Name = name;
            Background = 0;
            Difficulty = -1;
            this.oryxPresent = oryxPresent;
            this.mapId = mapId;
        }

        public Realm Overseer { get; set; }

        protected override void Init()
        {
            LoadMap("world" + mapId, MapType.Wmap);
            SetPieces.ApplySetPieces(this);
            if (oryxPresent)
                Overseer = new Realm(this);
            else
                Overseer = null;
        }

        public static GameWorld AutoName(int mapId, bool oryxPresent)
        {
            string name = RealmManager.Realms[new Random().Next(RealmManager.Realms.Count)];
            RealmManager.Realms.Remove(name);
            RealmManager.CurrentRealmNames.Add(name);
            return new GameWorld(mapId, name, oryxPresent);
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);
            if (Overseer != null)
                Overseer.Tick(time);
        }

        public void EnemyKilled(Enemy enemy, Player killer)
        {
            if (Overseer != null)
            {
                Overseer.HandleRealmEvent(enemy, killer);

                _autoEvents = new Thread(() =>
                {
                    do
                    {
                        if (Overseer.ActualRunningEvents.Count <= 5)
                        {
                            var revent = Overseer.RealmEventCache[Overseer.rand.Next(0, Overseer.RealmEventCache.Count)];
                            var success = true;

                            if (revent.Probability != 1)
                                if (revent.Probability > Overseer.rand.NextDouble())
                                    success = false;

                            if (success)
                            {
                                Overseer.ActualRunningEvents.Add(revent.Name);
                                Overseer.SpawnEvent(revent.Name, revent.MapSetPiece);
                                Overseer.BroadcastMsg(revent.Message);
                            }
                        }

                        Thread.Sleep(15 * 1000);
                    }
                    while (!Overseer.RealmClosed);
                })
                { IsBackground = true };
                _autoClose = new Thread(() =>
                {
                    Thread.Sleep(30 * 60 * 1000);

                    if (!Overseer.RealmClosed)
                        Overseer.InitCloseRealm();
                })
                { IsBackground = true };
                _autoEvents.Start();
                _autoClose.Start();
            }
        }

        private Thread _autoEvents { get; set; }
        private Thread _autoClose { get; set; }

        public override int EnterWorld(Entity entity)
        {
            int ret = base.EnterWorld(entity);

            if (entity is Player)
                Overseer.OnPlayerEntered(entity as Player);

            return ret;
        }

        public override void Dispose()
        {
            if (Overseer != null)
            {
                Overseer.Dispose();
                _autoEvents.Abort();
                _autoClose.Abort();
            }
            base.Dispose();
        }
    }
}