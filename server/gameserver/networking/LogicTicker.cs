#region

using LoESoft.Core.config;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

#endregion

namespace LoESoft.GameServer.realm
{
    //using Timer = System.Timers.Timer;

    public class LogicTicker : IDisposable
    {
        private RealmManager _manager { get; set; }
        private ConcurrentQueue<Action<RealmTime>>[] _pendings { get; set; }
        private Thread _logic { get; set; }

        public static int COOLDOWN_DELAY => 1000 / Settings.GAMESERVER.TICKETS_PER_SECOND;

        public RealmTime CurrentTime { get; private set; }

        public LogicTicker(RealmManager manager)
        {
            _manager = manager;
            _pendings = new ConcurrentQueue<Action<RealmTime>>[5];

            for (var i = 0; i < 5; i++)
                _pendings[i] = new ConcurrentQueue<Action<RealmTime>>();
        }

        public void Handle()
        {
            _logic = new Thread(() =>
            {
                var watch = new Stopwatch();
                var t = new RealmTime();
                var mspt = COOLDOWN_DELAY;

                long dt = 0;
                long count = 0;
                long xa = 0;

                watch.Start();

                do
                {
                    if (_manager.Terminating)
                        break;

                    var times = dt / mspt;

                    dt -= times * mspt;

                    times++;

                    var b = watch.ElapsedMilliseconds;

                    count += times;

                    t.TotalElapsedMs = b;
                    t.TickCount = count;
                    t.TickDelta = (int)count;
                    t.ElapsedMsDelta = (int)(times * mspt);

                    xa += t.ElapsedMsDelta;

                    foreach (var pending in _pendings)
                        while (pending.TryDequeue(out Action<RealmTime> action))
                        {
                            if (_manager.Terminating)
                                break;

                            try { action(t); }
                            catch { }
                        }

                    try
                    {
                        foreach (var world in _manager.Worlds.Values.Distinct())
                            if (world == null)
                                continue;
                            else
                                world.Tick(t);
                    }
                    catch { }

                    try
                    {
                        foreach (var request in TradeManager.CurrentRequests)
                            if (request.Key.Owner == null || request.Value.Owner == null)
                                continue;
                            else
                                TradeManager.CurrentRequests.Remove(request);
                    }
                    catch { }

                    try
                    {
                        foreach (var trading in TradeManager.TradingPlayers)
                            if (trading.Owner == null)
                                continue;
                            else
                                TradeManager.TradingPlayers.Remove(trading);
                    }
                    catch { }

                    Thread.Sleep(mspt);

                    dt += Math.Max(0, watch.ElapsedMilliseconds - b - mspt);
                }
                while (true);
            })
            {
                Name = "Logic Ticker Thread",
                CurrentCulture = CultureInfo.InvariantCulture,
                Priority = ThreadPriority.Highest
            };
            _logic.Start();
        }

        public void Dispose() => _logic.Abort();

        public void AddPendingAction(Action<RealmTime> callback)
            => AddPendingAction(callback, PendingPriority.Normal);

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority)
            => _pendings[(int)priority].Enqueue(callback);
    }
}