using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;

namespace LoESoft.GameServer.networking
{
    public partial class Client
    {
        public static bool AccessDenied { get; set; } = false;

        public ProtocolState State { get; internal set; }
        public Player Player { get; internal set; }
        public long InvDropEntryMS { get; set; } = -1;
        public long InvSwapEntryMS { get; set; } = -1;
        public long GlandsRegularEntryMS { get; set; } = -1;
        public long GlandsVIPEntryMS { get; set; } = -1;
        public long RealmRegularEntryMS { get; set; } = -1;
        public long RealmVIPEntryMS { get; set; } = -1;
        public bool CanInvDrop { get; set; } = true;
        public bool CanInvSwap { get; set; } = true;
        public bool CanGlands { get; set; } = true;
        public bool CanRealm { get; set; } = true;

        public bool IsReady()
            => State == ProtocolState.Disconnected ? false :
            (State != ProtocolState.Ready || (Player != null && (Player == null || Player.Owner != null)));

        public void SendMessage(Message msg) => handler?.IncomingMessage(msg);

        public void SendMessage(IEnumerable<Message> msgs) => handler?.IncomingMessage(msgs);
    }
}