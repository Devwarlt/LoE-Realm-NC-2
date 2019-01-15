using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;
using System.Timers;

namespace LoESoft.GameServer.networking
{
    public partial class Client
    {
        public static bool AccessDenied { get; set; } = false;

        public ProtocolState State { get; internal set; }
        public Player Player { get; internal set; }
        public bool InvDropClockInitialized { get; set; }
        public bool InvSwapClockInitialized { get; set; }
        public Timer InvDropClock { get; internal set; } = new Timer(1000) { AutoReset = true };
        public Timer InvSwapClock { get; internal set; } = new Timer(1000) { AutoReset = true };
        public bool CanInvDrop { get; set; } = true;
        public bool CanInvSwap { get; set; } = true;

        public bool IsReady()
            => State == ProtocolState.Disconnected ? false :
            (State != ProtocolState.Ready || (Player != null && (Player == null || Player.Owner != null)));

        public void SendMessage(Message msg) => handler?.IncomingMessage(msg);

        public void SendMessage(IEnumerable<Message> msgs) => handler?.IncomingMessage(msgs);
    }
}