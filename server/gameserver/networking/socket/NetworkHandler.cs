using LoESoft.Core.config;
using LoESoft.Core.models;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using static LoESoft.GameServer.networking.Client;

namespace LoESoft.GameServer.networking
{
    internal partial class NetworkHandler : IDisposable
    {
        public int BUFFER_SIZE = 0x20000;
        public int MESSAGE_SIZE = 5;

        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkHandler));

        private readonly Client client;
        private readonly ConcurrentQueue<Message> pending = new ConcurrentQueue<Message>();
        private readonly object sendLock = new object();
        private readonly Socket socket;

        private SocketAsyncEventArgs _outgoing;
        private byte[] _outgoingBuff;
        private OutgoingStage _outgoingState = OutgoingStage.Awaiting;

        private SocketAsyncEventArgs _incoming;
        private byte[] _incomingBuff;
        private IncomingStage _incomingState = IncomingStage.Awaiting;

        public NetworkHandler(Client client, Socket socket)
        {
            this.client = client;
            this.socket = socket;
        }

        public void BeginHandling()
        {
            socket.NoDelay = Settings.NETWORKING.DISABLE_NAGLES_ALGORITHM;
            socket.UseOnlyOverlappedIO = true;
            socket.SendTimeout = 1 * 1000;
            socket.ReceiveTimeout = 1 * 1000;
            socket.Ttl = 112;

            _outgoing = new SocketAsyncEventArgs();
            _outgoing.Completed += ProcessOutgoingMessage;
            _outgoing.UserToken = new OutgoingToken();
            _outgoingBuff = new byte[BUFFER_SIZE];
            _outgoing.SetBuffer(_outgoingBuff, 0, BUFFER_SIZE);

            _incoming = new SocketAsyncEventArgs();
            _incoming.Completed += ProcessIncomingMessage;
            _incoming.UserToken = new IncomingToken();
            _incomingBuff = new byte[BUFFER_SIZE];
            _incoming.SetBuffer(_incomingBuff, 0, BUFFER_SIZE);

            _incomingState = IncomingStage.ReceivingMessage;

            _incoming.SetBuffer(0, 5);

            if (!socket.ReceiveAsync(_incoming))
                ProcessIncomingMessage(null, _incoming);
        }

        private void OnError(Exception ex)
        {
            Log.Error(ex.ToString());

            GameServer.Manager.TryDisconnect(client, DisconnectReason.SOCKET_ERROR_DETECTED);
        }

        public void Dispose()
        {
            _outgoing.Completed -= ProcessOutgoingMessage;
            _outgoing.Dispose();
            _outgoingBuff = null;

            _incoming.Completed -= ProcessIncomingMessage;
            _incoming.Dispose();
            _incomingBuff = null;
        }
    }
}