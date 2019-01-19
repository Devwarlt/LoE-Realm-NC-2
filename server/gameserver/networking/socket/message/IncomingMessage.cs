using LoESoft.GameServer.networking.incoming;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static LoESoft.GameServer.networking.Client;

namespace LoESoft.GameServer.networking
{
    internal partial class NetworkHandler
    {
        private const int MaxConnectionLostAttempts = 30;
        private int ConnectionLostAttempts { get; set; } = 0;

        private void ProcessIncomingMessage(object sender, SocketAsyncEventArgs e) =>
            RIMM(e);

        #region "Regular Incoming Message Manager"

        private void RIMM(SocketAsyncEventArgs e)
        {
            try
            {
                if (!socket.Connected)
                {
                    ConnectionLostAttempts++;

                    if (ConnectionLostAttempts == MaxConnectionLostAttempts)
                    {
                        GameServer.Manager.TryDisconnect(client, DisconnectReason.CONNECTION_LOST);
                        return;
                    }
                }
                else
                    ConnectionLostAttempts = 0;

                if (e.SocketError != SocketError.Success)
                {
                    GameServer.Manager.TryDisconnect(client, DisconnectReason.SOCKET_ERROR);
                    return;
                }

                if (_incomingState == IncomingStage.ReceivingMessage)
                    RPRM(e);
                else if (_incomingState == IncomingStage.ReceivingData)
                    RPRD(e);
                else
                {
                    GameServer.Manager.TryDisconnect(client, DisconnectReason.CONNECTION_RESET);
                    return;
                }
            }
            catch (ObjectDisposedException)
            { return; }
        }

        private const int MaxInvalidBytesTransferred = 30;
        private int InvalidBytesTransferred { get; set; } = 0;

        private void RPRM(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred < 5)
            {
                InvalidBytesTransferred++;

                if (InvalidBytesTransferred == MaxInvalidBytesTransferred)
                {
                    GameServer.Manager.TryDisconnect(client, DisconnectReason.INVALID_MESSAGE_LENGTH);
                    return;
                }
            }
            else
                InvalidBytesTransferred = 0;

            if (e.Buffer[0] == 0xae
                && e.Buffer[1] == 0x7a
                && e.Buffer[2] == 0xf2
                && e.Buffer[3] == 0xb2
                && e.Buffer[4] == 0x95)
            {
                var c = Encoding.ASCII.GetBytes($"{GameServer.Manager.MaxClients}:{GameServer.Manager.ClientManager.Count}");
                socket.Send(c);
                return;
            }

            if (e.Buffer[0] == 0xaf
                && e.Buffer[1] == 0x7b
                && e.Buffer[2] == 0xf3
                && e.Buffer[3] == 0xb3
                && e.Buffer[4] == 0x96)
            {
                var c = Encoding.ASCII.GetBytes("Success");
                socket.Send(c);
                GameServer.ForceShutdown();
                return;
            }

            if (e.Buffer[0] == 0x3c
                && e.Buffer[1] == 0x70
                && e.Buffer[2] == 0x6f
                && e.Buffer[3] == 0x6c
                && e.Buffer[4] == 0x69)
            {
                ProcessPolicyFile();
                return;
            }

            int len = (e.UserToken as IncomingToken).Length =
                IPAddress.NetworkToHostOrder(BitConverter.ToInt32(e.Buffer, 0)) - 5;

            if (len < 0)
            {
                e.Dispose();
                return;
            }

            try
            {
                (e.UserToken as IncomingToken).Message = Message.Messages[(MessageID)e.Buffer[4]].CreateInstance();

                _incomingState = IncomingStage.ReceivingData;

                e.SetBuffer(0, len);

                socket.ReceiveAsync(e);
            }
            catch { e.Dispose(); }
        }

        private void RPRD(SocketAsyncEventArgs e)
        {
            // Burst of bytes are not ready yet, then keep them in a loop until dispatch properly
            if (e.BytesTransferred < (e.UserToken as IncomingToken).Length)
                return;

            var msg = (e.UserToken as IncomingToken).Message;
            var cont = client.IsReady();

            try
            {
                msg.Read(client, e.Buffer, 0, (e.UserToken as IncomingToken).Length);

                if (cont)
                {
                    if (GameServer.Manager.Terminating)
                    {
                        GameServer.Manager.TryDisconnect(client, DisconnectReason.STOPPING_REALM_MANAGER);
                        return;
                    }

                    if (client.State == ProtocolState.Disconnected)
                        GameServer.Manager.TryDisconnect(client, DisconnectReason.NETWORK_TICKER_DISCONNECT);
                    else
                        try
                        {
                            if (MessageHandler.Handlers.TryGetValue(msg.ID, out IMessage handler))
                                handler.Handle(client, (IncomingMessage)msg);
                        }
                        catch { }
                }
            }
            catch { }
            finally
            { _incomingState = IncomingStage.ProcessingMessage; }

            if (cont && socket.Connected)
            {
                _incomingState = IncomingStage.ReceivingMessage;

                e.SetBuffer(0, 5);
                socket.ReceiveAsync(e);
            }
            else
                e.Dispose();
        }

        #endregion "Regular Incoming Message Manager"
    }
}