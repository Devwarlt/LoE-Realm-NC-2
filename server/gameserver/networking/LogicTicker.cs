#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#endregion

namespace LoESoft.GameServer.realm
{
    public class LogicTicker : IDisposable
    {
        private RealmManager _manager { get; set; }
        private Stopwatch _watcher { get; set; } = new Stopwatch();
        private long _deltaTime { get; set; } = 0;
        private long _ticks { get; set; } = 0;

        public const int COOLDOWN_DELAY = 133;

        public RealmTime CurrentTime { get; private set; }

        public LogicTicker(RealmManager manager) => _manager = manager;

        public void Handle()
        {
            _watcher.Start();

            do
            {
                if (_manager.Terminating)
                    break;

                var elapsedticks = _deltaTime / COOLDOWN_DELAY;

                _deltaTime -= elapsedticks * COOLDOWN_DELAY;

                elapsedticks++;

                _ticks += elapsedticks;

                CurrentTime = new RealmTime()
                {
                    TotalElapsedMs = _watcher.ElapsedMilliseconds,
                    TickCount = _ticks,
                    TickDelta = (int)elapsedticks,
                    ElapsedMsDelta = (int)(elapsedticks * COOLDOWN_DELAY)
                };

                _manager.InterServer.Tick(CurrentTime);

                _deltaTime += Math.Max(0, _watcher.ElapsedMilliseconds - CurrentTime.TotalElapsedMs - COOLDOWN_DELAY);

                if (_manager.Worlds.Values.Count != 0)
                    try
                    {
                        _manager.Worlds.Values.Distinct().Select(i =>
                        {
                            i.Tick(CurrentTime);
                            return i;
                        }).ToArray();
                    }
                    catch { }

                if (TradeManager.TradingPlayers.Count != 0)
                    try
                    {
                        TradeManager.TradingPlayers.Where(_ => _.Owner == null)
                        .Select(i => TradeManager.TradingPlayers.Remove(i)).ToArray();
                    }
                    catch { }

                if (TradeManager.CurrentRequests.Count != 0)
                    try
                    {
                        TradeManager.CurrentRequests.Where(_ => _.Key.Owner == null || _.Value.Owner == null)
                        .Select(i => TradeManager.CurrentRequests.Remove(i)).ToArray();
                    }
                    catch { }

                Thread.Sleep(COOLDOWN_DELAY);
            } while (true);
        }

        public void Dispose() => _watcher.Stop();

        public void AddPendingAction(Action<RealmTime> callback)
            => callback?.Invoke(CurrentTime);

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority)
            => callback?.Invoke(CurrentTime);
    }
}