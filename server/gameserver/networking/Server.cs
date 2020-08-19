#region

using CA.Extensions.Concurrent;
using LoESoft.Core.config;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.networking
{
    internal class Server
    {
        public Server()
            => Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                UseOnlyOverlappedIO = true
            };

        private Socket Socket { get; set; }
        private readonly object lockPort = new object();

        public void Start()
        {
            bool connected = false;

            do
            {
                lock (lockPort)
                {
                    try
                    {
                        Socket.Bind(new IPEndPoint(IPAddress.Any, Settings.GAMESERVER.GAME_PORT));
                        Socket.Listen(0xFF);

                        connected = true;
                    }
                    catch { }
                }

                Thread.Sleep(100);
            } while (!connected);

            BeginAccept(Socket);
        }

        private void BeginAccept(Socket skt) => skt.BeginAccept(OnConnectionReceived, skt);

        private void OnConnectionReceived(IAsyncResult result)
        {
            try
            {
                var serverSocket = (Socket)result.AsyncState;
                var clientSocket = serverSocket.EndAccept(result);

                if (clientSocket != null)
                    new Client(clientSocket);

                BeginAccept(serverSocket);
            }
            catch { }
        }

        public void Stop()
        {
            var clients = GameServer.Manager.GetManager.Clients.ValueWhereAsParallel(_ => _ != null && _.Player != null);
            for (var i = 0; i < clients.Length; i++)
            {
                var client = clients[i];
                client.Save();
                GameServer.Manager.TryDisconnect(client, DisconnectReason.STOPING_SERVER);
            }

            Socket.Close();
        }
    }
}
