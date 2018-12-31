using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;

namespace LoESoft.GameServer.networking
{
    public partial class Client
    {
        public ProtocolState State { get; internal set; }
        public Player Player { get; internal set; }

        public bool IsReady()
            => State == ProtocolState.Disconnected ? false :
            (State != ProtocolState.Ready || (Player != null && (Player == null || Player.Owner != null)));

        public void SendMessage(Message msg) => handler?.IncomingMessage(msg);

        public void SendMessage(IEnumerable<Message> msgs) => handler?.IncomingMessage(msgs);
    }
}