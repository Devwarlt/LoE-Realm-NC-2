#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#endregion

namespace LoESoft.GameServer.realm
{
    using Timer = System.Timers.Timer;

    public class LogicTicker : IDisposable
    {
        private RealmManager _manager { get; set; }
        private Stopwatch _watcher { get; set; } = new Stopwatch();
        private long _deltaTime { get; set; } = 0;
        private long _ticks { get; set; } = 0;

        public const int COOLDOWN_DELAY = 200;

        public RealmTime CurrentTime { get; private set; }

        public LogicTicker(RealmManager manager) => _manager = manager;

        private Timer[] _timers { get; set; }

        public void Handle()
        {
            _watcher.Start();
            _timers = new Timer[4];

            for (var i = 0; i < 4; i++)
                _timers[i] = new Timer(COOLDOWN_DELAY) { AutoReset = true };

            _timers[0].Elapsed += LogicTicker_Elapsed_Timer;
            _timers[1].Elapsed += LogicTicker_Elapsed_WorldTimer;
            _timers[2].Elapsed += LogicTicker_Elapsed_TradingTimer;
            _timers[3].Elapsed += LogicTicker_Elapsed_RequestTimer;

            foreach (var timer in _timers)
                timer.Start();
        }

        private void LogicTicker_Elapsed_RequestTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (TradeManager.CurrentRequests.Count != 0)
                try
                {
                    TradeManager.CurrentRequests.Where(_ => _.Key.Owner == null || _.Value.Owner == null)
                    .Select(i => TradeManager.CurrentRequests.Remove(i)).ToArray();
                }
                catch { }
            else
                Thread.Sleep(COOLDOWN_DELAY * 10);
        }

        private void LogicTicker_Elapsed_TradingTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (TradeManager.TradingPlayers.Count != 0)
                try
                {
                    TradeManager.TradingPlayers.Where(_ => _.Owner == null)
                    .Select(i => TradeManager.TradingPlayers.Remove(i)).ToArray();
                }
                catch { }
            else
                Thread.Sleep(COOLDOWN_DELAY * 10);
        }

        private void LogicTicker_Elapsed_WorldTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
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
            else
                Thread.Sleep(COOLDOWN_DELAY * 10);
        }

        private void LogicTicker_Elapsed_Timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            var elapsed = _watcher.ElapsedMilliseconds;
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

            var delta = _watcher.ElapsedMilliseconds - elapsed;

            if (delta < COOLDOWN_DELAY)
                Thread.Sleep(COOLDOWN_DELAY - (int)delta);
        }

        public void Dispose()
        {
            _watcher.Stop();

            foreach (var timer in _timers)
                timer.Stop();
        }

        public void AddPendingAction(Action<RealmTime> callback)
            => callback?.Invoke(CurrentTime);

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority)
            => callback?.Invoke(CurrentTime);
    }
}