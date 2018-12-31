#region

using System;
using System.Diagnostics;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm
{
    using Timer = System.Timers.Timer;

    public class LogicTicker : IDisposable
    {
        private RealmManager _manager { get; set; }
        private Timer[] _timers { get; set; }
        private Stopwatch _watcher { get; set; } = new Stopwatch();
        private long _deltaTime { get; set; } = 0;
        private long _ticks { get; set; } = 0;

        private const int COOLDOWN_DELAY = 33; // 33 milliseconds (30 Hz)

        public RealmTime CurrentTime { get; private set; }

        public LogicTicker(RealmManager manager)
        {
            _manager = manager;
            _timers = new Timer[4];

            for (var i = 0; i < 4; i++)
                _timers[i] = new Timer(COOLDOWN_DELAY) { AutoReset = true };
        }

        public void Handle()
        {
            _watcher.Start();

            _timers[0].Elapsed += delegate // realm time thread
            {
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
            };
            _timers[1].Elapsed += delegate // world tick thread
            {
                _manager.Worlds.Values.Distinct().Select(i =>
                {
                    i.Tick(CurrentTime);
                    return i;
                }).ToArray();
            };
            _timers[2].Elapsed += delegate // trade thread
            {
                TradeManager.TradingPlayers.Where(_ => _.Owner == null)
                .Select(i => TradeManager.TradingPlayers.Remove(i)).ToArray();
            };
            _timers[3].Elapsed += delegate // requests thread
            {
                TradeManager.CurrentRequests.Where(_ => _.Key.Owner == null || _.Value.Owner == null)
                .Select(i => TradeManager.CurrentRequests.Remove(i)).ToArray();
            };
            _timers.Select(timer =>
            {
                timer.Start();
                return timer;
            }).ToList();
        }

        public void Dispose()
        {
            _watcher.Stop();
            _timers.Select(timer =>
            {
                timer.Stop();
                return timer;
            }).ToList();
        }

        public void AddPendingAction(Action<RealmTime> callback)
            => callback?.Invoke(CurrentTime);

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority)
            => callback?.Invoke(CurrentTime);
    }
}