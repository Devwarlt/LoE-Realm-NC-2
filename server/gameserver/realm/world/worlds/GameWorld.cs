#region

using CA.Extensions.Concurrent;
using LoESoft.Core.config;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public interface IRealm { }

    internal class GameWorld : World, IDungeon, IRealm
    {
        private readonly int mapId;
        private readonly bool oryxPresent;
        private readonly string displayname;

        public string OriginName { get; set; }

        public GameWorld(int mapId, string name, bool oryxPresent)
        {
            displayname = name;
            Name = name;
            OriginName = name;
            Background = 0;
            Difficulty = -1;
            MaxPlayersCount = 20;

            this.oryxPresent = oryxPresent;
            this.mapId = mapId;
        }

        public bool IsRealmClosed { get; set; }
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

        public static GameWorld AutoName(int mapId, bool oryxPresent, string oldName = null)
        {
            if (oldName != null)
                RealmManager.OldschoolPlayers[oldName] = false;

            var players = RealmManager.OldschoolPlayers.Where(p => !p.Value).ToList();
            var name = players[GameServer.Random.Next(0, players.Count)].Key;

            RealmManager.OldschoolPlayers[name] = true;

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
                                    var revent = Realm.RealmEventCache[Overseer.rand.Next(0, Realm.RealmEventCache.Count)];
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
                            await Task.Delay(20 * 60 * 1000);

                            foreach (var player in GetPlayers())
                            {
                                Overseer.SendMsg(player, "I HAVE CLOSED THIS REALM!", "#Oryx the Mad God");
                                Overseer.SendMsg(player, "YOU WILL NOT LIVE TO SEE THE LIGHT OF DAY!", "#Oryx the Mad God");
                            }

                            var clients = GameServer.Manager.GetManager.Clients.ValueWhereAsParallel(_ => _ != null && _.Player != null);
                            for (var i = 0; i < clients.Length; i++)
                                clients[i].Player?.GazerDM($"Oryx is preparing to close realm '{Name}' in 30 seconds.");

                            await Task.Delay(30 * 1000);

                            IsRealmClosed = true;

                            var wc = GameServer.Manager.AddWorld(new WineCellar());
                            foreach (var player in GetPlayers())
                            {
                                Overseer.SendMsg(player, "MY MINIONS HAVE FAILED ME!", "#Oryx the Mad God");
                                Overseer.SendMsg(player, "BUT NOW YOU SHALL FEEL MY WRATH!", "#Oryx the Mad God");
                                Overseer.SendMsg(player, "COME MEET YOUR DOOM AT THE WALLS OF MY WINE CELLAR!", "#Oryx the Mad God");

                                player.Client.SendMessage(new SHOWEFFECT { EffectType = EffectType.Jitter });
                                player.ApplyConditionEffect(new ConditionEffect
                                {
                                    DurationMS = 15000,
                                    Effect = ConditionEffectIndex.Invincible
                                });
                            }

                            await Task.Delay(10 * 1000);

                            foreach (var player in GetPlayers())
                                player.Client.SendMessage(new RECONNECT
                                {
                                    Host = "",
                                    Port = Settings.GAMESERVER.GAME_PORT,
                                    GameId = wc.Id,
                                    Name = wc.Name,
                                    Key = wc.PortalKey
                                });

                            await Task.Delay(5 * 1000);

                            IsRealmClosed = false;

                            Overseer.UniqueEvents.Clear();

                            break;
                        } while (true);

                        AutoEvents.Dispose();

                        Manager.RemoveWorld(this);
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
                AutoOryx.Dispose();
            }

            base.Dispose();
        }
    }
}
