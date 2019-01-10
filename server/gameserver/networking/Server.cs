#region

using LoESoft.Core.config;
using LoESoft.GameServer.realm;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.networking
{
    internal class Server
    {
        public Server(RealmManager manager)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                UseOnlyOverlappedIO = true
            };
            Manager = manager;
        }

        private Socket Socket { get; set; }

        public RealmManager Manager { get; private set; }

        public void Start()
        {
            Socket.Bind(new IPEndPoint(IPAddress.Any, Settings.GAMESERVER.PORT));
            Socket.Listen(0xFF);
            Beginaccept(Socket);
        }

        private void Beginaccept(Socket skt)
        {
            skt.BeginAccept(OnConnectionRecieved, skt);
        }

        private void OnConnectionRecieved(IAsyncResult result)
        {
            try
            {
                var socket = (Socket)result.AsyncState;
                var clientSocket = socket.EndAccept(result);

                if (clientSocket != null)
                    new Client(Manager, clientSocket);

                Beginaccept(socket);
            }
            catch { }
        }

        public void Stop()
        {
            foreach (ClientData cData in Manager.ClientManager.Values.ToArray())
            {
                cData.Client.Save();
                Manager.TryDisconnect(cData.Client, DisconnectReason.STOPING_SERVER);
            }

            Socket.Close();
        }
    }
}