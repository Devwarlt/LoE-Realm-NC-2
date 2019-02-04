using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace LoESoft.Core.config
{
    public partial class Settings
    {
        public static class APPENGINE
        {
            public static readonly string TITLE = "[LoESoft] (New Chicago) LoE Realm - AppEngine";
            public static readonly string FILE = ProcessFile("appengine");
            public static readonly int PRODUCTION_PORT = 7000;

            public static readonly List<(string name, string dns)> SERVERS = new List<(string, string)> {
                ("Chicago", "loe-nc.portmap.io")
            };

            public static readonly string SAFE_DOMAIN = "https://devwarlt.github.io/";

            public class ServerItem
            {
                public string Name { get; set; }
                public string DNS { get; set; }
                public double Lat { get; set; }
                public double Long { get; set; }
                public double Usage { get; set; }
                public bool AdminOnly { get; set; }

                public XElement ToXml()
                    => new XElement("Server",
                        new XElement("Name", Name),
                        new XElement("DNS", DNS),
                        new XElement("Lat", Lat),
                        new XElement("Long", Long),
                        new XElement("Usage", Usage),
                        new XElement("AdminOnly", AdminOnly)
                        );
            }

            public static List<ServerItem> GetServerItem(TcpClient client)
            {
                var isProduction = SERVER_MODE == ServerMode.Production;
                var gameserver = new List<ServerItem>();

                if (isProduction)
                    for (var i = 0; i < SERVERS.Count; i++)
                        gameserver.Add(new ServerItem()
                        {
                            Name = SERVERS[i].name,
                            DNS = SERVERS[i].dns,
                            Lat = 0,
                            Long = 0,
                            Usage = 0,//GetUsage(SERVERS[i].dns, client),
                            AdminOnly = false
                        });
                else
                    return new List<ServerItem>
                    {
                        new ServerItem()
                        {
                            Name = "Local",
                            DNS = "localhost",
                            Lat = 0,
                            Long = 0,
                            Usage = 0,
                            AdminOnly = false
                        }
                    };

                return gameserver;
            }

            private static double GetUsage(string dns, TcpClient client)
            {
                var attempts = 5;

                string usage = null;

                do
                {
                    client.Connect(dns, PRODUCTION_PORT);

                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        var buffer = Encoding.UTF8.GetBytes("get -s -u");

                        stream.WriteAsync(buffer, 0, buffer.Length);

                        buffer = new byte[4096];

                        var data = stream.ReadAsync(buffer, 0, buffer.Length);
                        data.RunSynchronously();

                        usage = Encoding.UTF8.GetString(buffer, 0, data.Result);
                    }

                    attempts--;
                } while (usage != null || attempts > 0);

                if (usage == null)
                    return -1;
                else
                {
                    try
                    {
                        client.Close();
                        client.Dispose();
                    }
                    catch { }

                    var info = usage.Split(':');
                    var online = double.Parse(info[0]);
                    var max = double.Parse(info[1]);

                    return online / max;
                }
            }

            //Usage
            //[0.0-0.79]: Normal, [0.8-0.99]: Crowded, [1.0]: Full

            /// <summary>
            /// Server Usage function
            /// If AppEngine cannot connect to the server or even server is unreachable
            /// it'll return -1 otherwise AMOUNT OF PLAYERS divided by MAX NUMBER OF
            /// CONNECTIONS.
            /// </summary>
            /// <param name="dns"></param>
            /// <param name="port"></param>
            /// <returns></returns>
            public static double GetUsage(string dns, int port)
            {
                var IPs = Dns.GetHostAddresses(dns);

                if (IsListening(dns, port))
                    try
                    {
                        using (TcpClient tcp = new TcpClient(dns, port))
                        {
                            tcp.NoDelay = false;

                            NetworkStream stream = tcp.GetStream();

                            string[] data = null;

                            byte[] response = new byte[tcp.ReceiveBufferSize];
                            byte[] usage = new byte[5] { 0xae, 0x7a, 0xf2, 0xb2, 0x95 };

                            stream.Write(usage, 0, 5);
                            Array.Resize(ref response, tcp.Client.Receive(response));

                            data = Encoding.ASCII.GetString(response).Split(':');

                            double serverUsage = double.Parse(data[1]) / double.Parse(data[0]);

                            tcp.Close();

                            return serverUsage;
                        }
                    }
                    catch (ObjectDisposedException) { return -1; }
                else
                    return -1;
            }

            public static string SafeRestart(string dns, int port)
            {
                var IPs = Dns.GetHostAddresses(dns);

                if (IsListening(dns, port))
                    try
                    {
                        using (var tcp = new TcpClient(dns, port))
                        {
                            tcp.NoDelay = false;

                            var stream = tcp.GetStream();

                            string data = null;

                            byte[] response = new byte[tcp.ReceiveBufferSize];
                            byte[] usage = new byte[5] { 0xaf, 0x7b, 0xf3, 0xb3, 0x96 };

                            stream.Write(usage, 0, 5);

                            Array.Resize(ref response, tcp.Client.Receive(response));

                            data = Encoding.ASCII.GetString(response);

                            tcp.Close();

                            return data;
                        }
                    }
                    catch (Exception e) { return "An error occurred with server: " + e; }

                return null;
            }

            public static bool IsListening(string dns, int port)
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        if (!socket.BeginConnect(dns, port, null, null).AsyncWaitHandle.WaitOne(3000, true))
                        {
                            socket.Close();
                            return false;
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionRefused || ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            socket.Close();
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
    }
}