using LoESoft.GameServer.realm.entity.player;
using System;
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
        public bool GlandsClockInitialized { get; set; }
        public bool RealmClockInitialized { get; set; }
        public Timer InvDropClock { get; internal set; } = new Timer(1000) { AutoReset = true };
        public Timer InvSwapClock { get; internal set; } = new Timer(1000) { AutoReset = true };
        public Timer GlandsRegularClock { get; internal set; } = new Timer(30000) { AutoReset = true };
        public Timer GlandsVIPClock { get; internal set; } = new Timer(10000) { AutoReset = true };
        public Timer RealmRegularClock { get; internal set; } = new Timer(30000) { AutoReset = true };
        public Timer RealmVIPClock { get; internal set; } = new Timer(10000) { AutoReset = true };
        public bool CanInvDrop { get; set; } = true;
        public bool CanInvSwap { get; set; } = true;
        public bool CanGlands { get; set; } = true;
        public bool CanRealm { get; set; } = true;
        public int ElapsedGlandsRegularClock { get; set; } = 0;
        public int ElapsedGlandsVIPClock { get; set; } = 0;
        public int ElapsedRealmRegularClock { get; set; } = 0;
        public int ElapsedRealmVIPClock { get; set; } = 0;
        public DateTime LastGlandsRegularEntry { get; set; }
        public DateTime LastGlandsVIPEntry { get; set; }
        public DateTime LastRealmRegularEntry { get; set; }
        public DateTime LastRealmVIPEntry { get; set; }

        public bool IsReady()
            => State == ProtocolState.Disconnected ? false :
            (State != ProtocolState.Ready || (Player != null && (Player == null || Player.Owner != null)));

        public void SendMessage(Message msg) => handler?.IncomingMessage(msg);

        public void SendMessage(IEnumerable<Message> msgs) => handler?.IncomingMessage(msgs);
    }
}