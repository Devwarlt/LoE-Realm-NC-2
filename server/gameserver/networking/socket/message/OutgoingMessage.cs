using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LoESoft.GameServer.networking
{
    internal partial class NetworkHandler
    {
        private void ProcessOutgoingMessage(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (!socket.Connected)
                    return;

                int len;

                switch (_outgoingState)
                {
                    case OutgoingStage.Ready:
                        len = (e.UserToken as OutgoingToken).Message.Write(client, _outgoingBuff, 0);

                        _outgoingState = OutgoingStage.Sending;

                        e.SetBuffer(0, len);

                        if (!socket.Connected)
                            return;

                        socket.SendAsync(e);

                        break;

                    case OutgoingStage.Sending:
                        (e.UserToken as OutgoingToken).Message = null;

                        if (IncomingMessage(e, true))
                        {
                            len = (e.UserToken as OutgoingToken).Message.Write(client, _outgoingBuff, 0);

                            _outgoingState = OutgoingStage.Sending;

                            e.SetBuffer(0, len);

                            if (!socket.Connected)
                                return;

                            socket.SendAsync(e);
                        }
                        break;
                }
            }
            catch (Exception ex) { OnError(ex); }
        }

        private bool IncomingMessage(SocketAsyncEventArgs e, bool ignoreSending)
        {
            lock (sendLock)
            {
                if (_outgoingState == OutgoingStage.Ready || (!ignoreSending && _outgoingState == OutgoingStage.Sending))
                    return false;

                if (pending.TryDequeue(out Message packet))
                {
                    (e.UserToken as OutgoingToken).Message = packet;

                    _outgoingState = OutgoingStage.Ready;

                    return true;
                }

                _outgoingState = OutgoingStage.Awaiting;

                return false;
            }
        }

        public void IncomingMessage(Message msg)
        {
            if (!socket.Connected)
                return;

            pending.Enqueue(msg);

            if (IncomingMessage(_outgoing, false))
            {
                int len = (_outgoing.UserToken as OutgoingToken).Message.Write(client, _outgoingBuff, 0);

                _outgoingState = OutgoingStage.Sending;
                _outgoing.SetBuffer(_outgoingBuff, 0, len);

                if (!socket.SendAsync(_outgoing))
                    ProcessOutgoingMessage(null, _outgoing);
            }
        }

        public void IncomingMessage(IEnumerable<Message> msgs)
        {
            if (!socket.Connected)
                return;

            foreach (var i in msgs)
                pending.Enqueue(i);

            if (IncomingMessage(_outgoing, false))
            {
                int len = (_outgoing.UserToken as OutgoingToken).Message.Write(client, _outgoingBuff, 0);

                _outgoingState = OutgoingStage.Sending;
                _outgoing.SetBuffer(_outgoingBuff, 0, len);

                if (!socket.SendAsync(_outgoing))
                    ProcessOutgoingMessage(null, _outgoing);
            }
        }
    }
}