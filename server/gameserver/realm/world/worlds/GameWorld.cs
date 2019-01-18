#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using System;
using System.Threading.Tasks;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public interface IRealm { }

    internal class GameWorld : World, IDungeon, IRealm
    {
        private readonly int mapId;
        private readonly bool oryxPresent;
        private readonly string displayname;

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
        private Task AutoEvents { get; set; }
        private Task AutoOryx { get; set; }

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
            var name = RealmManager.Realms[new Random().Next(RealmManager.Realms.Count)];

            RealmManager.Realms.Remove(name);
            RealmManager.CurrentRealmNames.Add(name);

            return new GameWorld(mapId, name, oryxPresent);
        }

        private bool IsRunning { get; set; }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            if (Overseer != null)
            {
                Overseer.Tick(time);

                if (!IsRunning)
                {
                    IsRunning = true;

                    AutoEvents = new Task(async () =>
                    {
                        do
                        {
                            if (Overseer != null)
                            {
                                if (Overseer.ActualRunningEvents.Count < 5)
                                {
                                    var revent = Overseer.RealmEventCache[Overseer.rand.Next(0, Overseer.RealmEventCache.Count)];
                                    var success = true;

                                    if (!Overseer.UniqueEvents.Contains(revent.Name))
                                    {
                                        if (revent.Probability != 1)
                                            if (revent.Probability > Overseer.rand.NextDouble())
                                                success = false;

                                        if (success)
                                        {
                                            if (revent.Once)
                                                Overseer.UniqueEvents.Add(revent.Name);

                                            Overseer.ActualRunningEvents.Add(revent.Name);
                                            Overseer.SpawnEvent(revent.Name, revent.MapSetPiece);
                                            Overseer.BroadcastMsg(revent.Message);
                                        }
                                    }

                                    await Task.Delay(1 * 1000);
                                }
                                else
                                    await Task.Delay(30 * 1000);
                            }
                            else
                                await Task.Delay(5 * 1000);
                        }
                        while (true);
                    }, TaskCreationOptions.LongRunning);
                    AutoEvents.ContinueWith(task => GameServer.log.Error(task.Exception.InnerException),
                        TaskContinuationOptions.OnlyOnFaulted);
                    AutoOryx = new Task(async () =>
                    {
                        do
                        {
                            await Task.Delay(10 * 60 * 1000);

                            foreach (var i in Players.Values)
                            {
                                Overseer.SendMsg(i, "I HAVE CLOSED THIS REALM!", "#Oryx the Mad God");
                                Overseer.SendMsg(i, "YOU WILL NOT LIVE TO SEE THE LIGHT OF DAY!", "#Oryx the Mad God");
                            }

                            foreach (var i in GameServer.Manager.ClientManager.Values)
                                i.Client.Player?.GazerDM($"Oryx is preparing to close realm '{Name}' in 15 seconds.");

                            await Task.Delay(15 * 1000);

                            var wc = GameServer.Manager.AddWorld(new WineCellar());
                            wc.Manager = GameServer.Manager;

                            foreach (var i in Players.Values)
                            {
                                Overseer.SendMsg(i, "MY MINIONS HAVE FAILED ME!", "#Oryx the Mad God");
                                Overseer.SendMsg(i, "BUT NOW YOU SHALL FEEL MY WRATH!", "#Oryx the Mad God");
                                Overseer.SendMsg(i, "COME MEET YOUR DOOM AT THE WALLS OF MY WINE CELLAR!", "#Oryx the Mad God");

                                i.Client.SendMessage(new SHOWEFFECT { EffectType = EffectType.Jitter });
                            }

                            Timers.Add(new WorldTimer(5000, (w, t) =>
                            {
                                foreach (var i in Players.Values)
                                {
                                    if (wc == null)
                                        GameServer.Manager.TryDisconnect(i.Client, DisconnectReason.RECONNECT_TO_CASTLE);

                                    i.Client.SendMessage(new RECONNECT
                                    {
                                        Host = "",
                                        Port = Settings.GAMESERVER.PORT,
                                        GameId = wc.Id,
                                        Name = wc.Name,
                                        Key = wc.PortalKey
                                    });
                                }
                            }));
                        } while (true);
                    }, TaskCreationOptions.LongRunning);
                    AutoOryx.ContinueWith(task => GameServer.log.Error(task.Exception.InnerException),
                        TaskContinuationOptions.OnlyOnFaulted);
                    AutoEvents.Start();
                    AutoOryx.Start();
                }
            }
        }

        public void EnemyKilled(Enemy enemy, Player killer)
        {
            if (Overseer != null)
                Overseer.HandleRealmEvent(enemy, killer);
        }

        public override int EnterWorld(Entity entity)
        {
            var ret = base.EnterWorld(entity);

            if (entity is Player)
                Overseer.OnPlayerEntered(entity as Player);

            return ret;
        }

        public override void Dispose()
        {
            if (Overseer != null)
            {
                Overseer.Dispose();

                AutoEvents.Dispose();
            }

            base.Dispose();
        }
    }
}