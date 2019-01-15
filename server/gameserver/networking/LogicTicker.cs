#region

using LoESoft.Core.config;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace LoESoft.GameServer.realm
{
    public class LogicTicker
    {
        private RealmManager _manager { get; set; }
        private Task _logic { get; set; }

        public static int COOLDOWN_DELAY => 1000 / Settings.GAMESERVER.TICKETS_PER_SECOND;

        public RealmTime CurrentTime { get; private set; }

        public LogicTicker(RealmManager manager)
        {
            _manager = manager;

            CurrentTime = new RealmTime();
        }

        public async void TickLoop()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            var looptime = 0;
            var t = new RealmTime();
            var watch = Stopwatch.StartNew();

            do
            {
                t.TotalElapsedMs = watch.ElapsedMilliseconds;
                t.TickDelta = looptime / COOLDOWN_DELAY;
                t.TickCount += t.TickDelta;
                t.ElapsedMsDelta = t.TickDelta * COOLDOWN_DELAY;

                if (_manager.Terminating)
                    break;

                TickActions(t);

                looptime += (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs) - t.ElapsedMsDelta;

                await Task.Delay(Math.Max(0, COOLDOWN_DELAY - (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs)));
            } while (true);
        }

        private void TickActions(RealmTime t)
        {
            var clients = _manager.ClientManager.Values.Select(client => client.Client).ToList();

            _manager.InterServer.Tick(t);

            TickTrade(t);

            foreach (var client in clients)
                if (client.Player?.Owner != null)
                    client.Player.Flush();
        }

        private void TickTrade(RealmTime t)
        {
            try
            {
                if (TradeManager.TradingPlayers.Count != 0)
                    TradeManager.TradingPlayers.Where(_ => _.Owner == null)
                    .Select(i => TradeManager.TradingPlayers.Remove(i)).ToArray();
            }
            catch { }

            try
            {
                if (TradeManager.CurrentRequests.Count != 0)
                    TradeManager.CurrentRequests.Where(_ => _.Key.Owner == null || _.Value.Owner == null)
                    .Select(i => TradeManager.CurrentRequests.Remove(i)).ToArray();
            }
            catch { }

            TickWorlds(t);
        }

        private void TickWorlds(RealmTime t)
        {
            CurrentTime.TickDelta += t.TickDelta;

            if (_logic == null || _logic.IsCompleted)
            {
                t.TickDelta = CurrentTime.TickDelta;
                t.ElapsedMsDelta = t.TickDelta * COOLDOWN_DELAY;

                CurrentTime.TickDelta = 0;

                _logic = Task.Factory.StartNew(() =>
                {
                    foreach (var world in _manager.Worlds.Values.Distinct())
                        world.Tick(t);
                }).ContinueWith(task
                => GameServer.log.Error(task.Exception.InnerException),
                TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority = PendingPriority.Normal)
            => callback.Invoke(CurrentTime);
    }
}