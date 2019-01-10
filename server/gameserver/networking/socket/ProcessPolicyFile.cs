using LoESoft.Core;
using System;
using System.Net.Sockets;

namespace LoESoft.GameServer.networking
{
    internal partial class NetworkHandler
    {
        private void ProcessPolicyFile()
        {
            if (socket == null)
                return;

            try
            {
                var s = new NetworkStream(socket);
                var wtr = new NWriter(s);
                wtr.WriteNullTerminatedString(@"<cross-domain-policy>
    <allowed-access-from domain=""*"" to-ports=""*"" />
</cross-domain-policy>");
                wtr.Write((byte)'\r');
                wtr.Write((byte)'\n');
            }
            catch (Exception e)
            {
                GameServer.log.Error(e.ToString());
            }
        }
    }
}