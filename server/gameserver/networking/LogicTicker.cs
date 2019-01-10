#region

using LoESoft.Core.config;
using LoESoft.Core.models;
using System;
using System.Collections.Concurrent;
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
        private ConcurrentQueue<Action<RealmTime>>[] _pendings { get; set; }
        private ManualResetEvent _mre { get; set; }
        private Task _logic { get; set; }

        public static int COOLDOWN_DELAY => 666 / Settings.GAMESERVER.TICKETS_PER_SECOND;

        public RealmTime CurrentTime { get; private set; }

        public LogicTicker(RealmManager manager)
        {
            _manager = manager;
            _pendings = new ConcurrentQueue<Action<RealmTime>>[5];

            for (var i = 0; i < 5; i++)
                _pendings[i] = new ConcurrentQueue<Action<RealmTime>>();

            _mre = new ManualResetEvent(false);

            CurrentTime = new RealmTime();
        }

        public void TickLoop()
        {
            var looptime = 0;
            var t = new RealmTime();
            var watch = Stopwatch.StartNew();

            do
            {
                t.TotalElapsedMs = watch.ElapsedMilliseconds;

                _mre.WaitOne(Math.Max(0, COOLDOWN_DELAY - (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs)));

                t.TickDelta = looptime / COOLDOWN_DELAY;
                t.TickCount += t.TickDelta;
                t.ElapsedMsDelta = t.TickDelta * COOLDOWN_DELAY;

                if (_manager.Terminating)
                    break;

                TickActions(t);

                looptime += (int)(watch.ElapsedMilliseconds - t.TotalElapsedMs) - t.ElapsedMsDelta;
            }
            while (true);
        }

        private void TickActions(RealmTime t)
        {
            var clients = _manager.ClientManager.Values.Select(client => client.Client).ToList();

            foreach (var pending in _pendings)
                while (pending.TryDequeue(out Action<RealmTime> action))
                    try
                    { action(t); }
                    catch { };

            _manager.InterServer.Tick(t);

            TickTrade(t);

            foreach (var client in clients)
                if (client.Player?.Owner != null)
                    client.Player.Flush();
        }

        private void TickWorlds(RealmTime t)
        {
            CurrentTime.TickDelta += t.TickDelta;

            if (_logic == null || _logic.IsCompleted)
            {
                t.TickDelta = CurrentTime.TickDelta;
                t.ElapsedMsDelta = t.TickDelta * COOLDOWN_DELAY;

                if (t.ElapsedMsDelta < COOLDOWN_DELAY)
                    return;

                CurrentTime.TickDelta = 0;

                _logic = Task.Factory.StartNew(() =>
                {
                    foreach (var world in _manager.Worlds.Values.Distinct())
                        world.Tick(t);
                }).ContinueWith(task =>
                Log.Error(task.Exception.InnerException.ToString()),
                TaskContinuationOptions.OnlyOnFaulted);
            }
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

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority = PendingPriority.Normal)
            => _pendings[(int)priority].Enqueue(callback);
    }
}