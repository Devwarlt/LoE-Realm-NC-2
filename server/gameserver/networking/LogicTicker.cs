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

                var delay = Math.Max(0, COOLDOWN_DELAY - (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs));

                await Task.Delay(delay);

                t.TickDelta = looptime / COOLDOWN_DELAY;
                t.TickCount += t.TickDelta;
                t.ElapsedMsDelta = t.TickDelta * COOLDOWN_DELAY;

                if (_manager.Terminating)
                    break;

                _manager.InterServer.Tick(t);

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

                CurrentTime.TickDelta += t.TickDelta;

                if (_logic == null || _logic.IsCompleted)
                {
                    t.TickDelta = CurrentTime.TickDelta;
                    t.ElapsedMsDelta = t.TickDelta * COOLDOWN_DELAY;

                    CurrentTime.TickDelta = 0;
                    CurrentTime.TotalElapsedMs = t.TotalElapsedMs;

                    _logic = Task.Factory.StartNew(() =>
                    {
                        foreach (var world in _manager.Worlds.Values.Distinct())
                            world.Tick(t);
                    }).ContinueWith(task
                    => GameServer.log.Error(task.Exception.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted);
                }

                foreach (var client in _manager.ClientManager.Values.Select(client => client.Client).ToList())
                    if (client.Player?.Owner != null)
                        client.Player.Flush();

                looptime += (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs) - t.ElapsedMsDelta;
            } while (true);
        }

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority = PendingPriority.Normal)
            => callback.Invoke(CurrentTime);
    }
}