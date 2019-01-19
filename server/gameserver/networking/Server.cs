#region

using LoESoft.Core.config;
using System;
using System.Linq;
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
                        Socket.Bind(new IPEndPoint(IPAddress.Any, Settings.GAMESERVER.PORT));
                        Socket.Listen(0xFF);

                        connected = true;
                    }
                    catch { }
                }

                Thread.Sleep(100);
            } while (!connected);

            Beginaccept(Socket);
        }

        private void Beginaccept(Socket skt) => skt.BeginAccept(OnConnectionRecieved, skt);

        private void OnConnectionRecieved(IAsyncResult result)
        {
            try
            {
                var socket = (Socket)result.AsyncState;
                var clientSocket = socket.EndAccept(result);

                if (clientSocket != null)
                    new Client(clientSocket);

                Beginaccept(socket);
            }
            catch { }
        }

        public void Stop()
        {
            foreach (var cData in GameServer.Manager.ClientManager.Values.ToArray())
            {
                cData.Client.Save();
                GameServer.Manager.TryDisconnect(cData.Client, DisconnectReason.STOPING_SERVER);
            }

            Socket.Close();
        }
    }
}