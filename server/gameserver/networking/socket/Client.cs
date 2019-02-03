#region

using LoESoft.Core;
using LoESoft.Core.config;
using System;
using System.Net.Sockets;

#endregion

namespace LoESoft.GameServer.networking
{
    public partial class Client : IDisposable
    {
        public string AccountId { get; set; }
        public DbChar Character { get; internal set; }
        public DbAccount Account { get; internal set; }
        public wRandom Random { get; internal set; }
        public bool EventNotification { get; internal set; }
        public int TargetWorld { get; internal set; }
        public string ConnectedBuild { get; internal set; }
        public Socket Socket { get; internal set; }
        public RC4 IncomingCipher { get; private set; }
        public RC4 OutgoingCipher { get; private set; }

        private NetworkHandler handler;
        private bool disposed;

        public Client(Socket skt)
        {
            Socket = skt;

            IncomingCipher = new RC4(Settings.NETWORKING.INCOMING_CIPHER);
            OutgoingCipher = new RC4(Settings.NETWORKING.OUTGOING_CIPHER);

            handler = new NetworkHandler(this, Socket);
            handler.BeginHandling();

            AccountId = "-1";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "handler")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (disposed)
                return;

            try
            {
                IncomingCipher = null;
                OutgoingCipher = null;
                Socket = null;
                Character = null;
                Account = null;

                if (Player.PetID != 0 && Player.Pet != null)
                    Player.Owner?.LeaveWorld(Player.Pet);

                Player = null;
                Random = null;
                ConnectedBuild = null;
            }
            catch
            { return; }
            finally
            { disposed = true; }
        }
    }
}