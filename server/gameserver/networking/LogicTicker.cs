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

        public RealmTime GameTime { get; private set; }

        public LogicTicker(RealmManager manager)
        {
            _manager = manager;

            GameTime = new RealmTime();
        }

        public async void TickLoop()
        {
            var looptime = 0;
            var t = new RealmTime();
            var watch = Stopwatch.StartNew();
            var cooldown = 1000 / Settings.GAMESERVER.TICKETS_PER_SECOND;

            do
            {
                t.TotalElapsedMs = watch.ElapsedMilliseconds;

                var delay = Math.Max(0, cooldown - (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs));

                await Task.Delay(delay);

                t.TickDelta = looptime / cooldown;
                t.TickCount += t.TickDelta;
                t.ElapsedMsDelta = t.TickDelta * cooldown;

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

                GameTime.TickDelta += t.TickDelta;

                if (_logic == null || _logic.IsCompleted)
                {
                    t.TickDelta = GameTime.TickDelta;
                    t.ElapsedMsDelta = t.TickDelta * cooldown;

                    GameTime.TickDelta = 0;
                    GameTime.TotalElapsedMs = t.TotalElapsedMs;

                    _logic = Task.Factory.StartNew(() =>
                    {
                        foreach (var world in _manager.Worlds.Values.Distinct())
                            world.Tick(t);
                    }).ContinueWith(task
                    => GameServer.log.Error(task.Exception.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted);
                }

                looptime += (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs) - t.ElapsedMsDelta;
            } while (true);
        }
    }
}