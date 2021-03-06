﻿#region

using LoESoft.Core;
using LoESoft.Core.config;
using System;
using System.Net;
using System.Net.Sockets;

#endregion

namespace LoESoft.GameServer.networking
{
    internal class PolicyServer
    {
        private readonly TcpListener listener;
        private bool started;

        public PolicyServer() => listener = new TcpListener(IPAddress.Any, Settings.POLICYSERVER.PORT);

        private static void ServePolicyFile(IAsyncResult ar)
        {
            try
            {
                var srv = ar.AsyncState as TcpListener;
                var cli = srv.EndAcceptTcpClient(ar);

                srv.BeginAcceptTcpClient(ServePolicyFile, ar.AsyncState);

                var s = cli.GetStream();
                var rdr = new NReader(s);
                var wtr = new NWriter(s);

                if (rdr.ReadNullTerminatedString() == "<policy-file-request/>")
                {
                    wtr.WriteNullTerminatedString(
                        @"<cross-domain-policy>" +
                        @"<allow-access-from domain=""*"" to-ports=""*"" />" +
                        @"</cross-domain-policy>");
                    wtr.Write((byte)'\r');
                    wtr.Write((byte)'\n');
                }

                cli.Close();
            }
            catch { }
        }

        public void Start()
        {
            try
            {
                listener.Start();
                listener.BeginAcceptTcpClient(ServePolicyFile, listener);
                started = true;
            }
            catch
            {
                started = false;

                GameServer.ForceShutdown();
            }
        }

        public void Stop()
        {
            if (started)
                listener.Stop();
        }
    }
}