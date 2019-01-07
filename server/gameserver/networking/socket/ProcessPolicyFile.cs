using LoESoft.Core;
using LoESoft.Core.config;
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
                wtr.WriteNullTerminatedString(Settings.NETWORKING.INTERNAL.CROSS_DOMAIN_POLICY);
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