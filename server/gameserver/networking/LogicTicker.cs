#region

using LoESoft.Core.config;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace LoESoft.GameServer.realm
{
    public class LogicTicker
    {
        private Task _logic { get; set; }

        public RealmTime GameTime { get; private set; }

        public LogicTicker() => GameTime = new RealmTime();

        public async void TickLoop()
        {
            var looptime = 0;
            var watch = Stopwatch.StartNew();
            var cooldown = 1000 / Settings.GAMESERVER.TICKETS_PER_SECOND;

            do
            {
                if (GameServer.Manager.Terminating)
                    break;

                GameTime.TotalElapsedMs = watch.ElapsedMilliseconds;

                await Task.Delay(Math.Max(0, cooldown - (int)(watch.ElapsedMilliseconds - GameTime.TotalElapsedMs)));

                GameTime.TickDelta = looptime / cooldown;
                GameTime.TickCount += GameTime.TickDelta;
                GameTime.ElapsedMsDelta = GameTime.TickDelta * cooldown;

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

                GameServer.Manager.Monitor.Tick(GameTime);

                looptime += (int)(watch.ElapsedMilliseconds - GameTime.TotalElapsedMs) - GameTime.ElapsedMsDelta;
            } while (true);
        }
    }
}