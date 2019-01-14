#region

using LoESoft.Core;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;

#endregion

namespace LoESoft.AppEngine
{
    internal class ISManager : InterServerChannel, IDisposable
    {
        private ILog log = LogManager.GetLogger(nameof(ISManager));

        public const string NETWORK = "network";
        public const string CHAT = "chat";
        public const string CONTROL = "control";   //maybe later...

        private enum NetworkCode
        {
            JOIN,
            PING,
            QUIT
        }

        private struct NetworkMsg
        {
            public NetworkCode Code { get; set; }
            public string Type { get; set; }
        }

        public ISManager() : base(AppEngine.Database, AppEngine.InstanceId)
        {
            AddHandler<NetworkMsg>(NETWORK, HandleNetwork);
            //AddHandler<Message>(CHAT, HandleChat);

            Publish(NETWORK, new NetworkMsg()
            {
                Code = NetworkCode.JOIN,
                Type = "Account Server"
            });
        }

        private ConcurrentDictionary<string, int> availableInstance = new ConcurrentDictionary<string, int>();

        private System.Timers.Timer tmr = new System.Timers.Timer(2000);

        public void Run()
        {
            tmr.Elapsed += (sender, e) => Tick();
            tmr.Start();
        }

        private void Tick()
        {
            Publish(NETWORK, new NetworkMsg() { Code = NetworkCode.PING });

            foreach (var i in availableInstance.Keys)
            {
                if (availableInstance.ContainsKey(i) && --availableInstance[i] == 0)
                {
                    int val;
                    availableInstance.TryRemove(i, out val);
                }
            }
        }

        public void Dispose()
        {
            tmr.Stop();
            tmr.Dispose();
            Publish(NETWORK, new NetworkMsg() { Code = NetworkCode.QUIT });
        }

        private void HandleNetwork(object sender, InterServerEventArgs<NetworkMsg> e)
        {
            switch (e.Content.Code)
            {
                case NetworkCode.JOIN:
                    do Thread.Sleep(250);
                    while (e.InstanceId != null);

                    if (availableInstance.TryAdd(e.InstanceId, 5))
                    {
                        Publish(NETWORK, new NetworkMsg()   //for the new instances
                        {
                            Code = NetworkCode.JOIN,
                            Type = "Account Server"
                        });
                    }
                    else
                        availableInstance[e.InstanceId] = 5;
                    break;

                case NetworkCode.PING:
                    availableInstance[e.InstanceId] = 5;
                    break;

                case NetworkCode.QUIT:
                    int dummy;
                    availableInstance.TryRemove(e.InstanceId, out dummy);
                    break;
            }
        }

        //Chat

        private const char TELL = 't';
        private const char GUILD = 'g';
        private const char ANNOUNCE = 'a';

        private struct Message
        {
            public char Type { get; set; }
            public string Inst { get; set; }

            public int ObjId { get; set; }
            public int Stars { get; set; }
            public string From { get; set; }

            public string To { get; set; }
            public string Text { get; set; }
        }

        private void HandleChat(object sender, InterServerEventArgs<Message> e)
        {
            switch (e.Content.Type)
            {
                case TELL:
                    {
                        string from = Database.ResolveIgn(e.Content.From);
                        string to = Database.ResolveIgn(e.Content.To);
                        log.Info($"<{from} -> {to}> {e.Content.Text}");
                    }
                    break;

                case GUILD:
                    {
                        string from = Database.ResolveIgn(e.Content.From);
                        log.Info($"<{from} -> Guild> {e.Content.Text}");
                    }
                    break;

                case ANNOUNCE:
                    {
                        log.Info($"<Announcement> {e.Content.Text}");
                    }
                    break;
            }
        }
    }
}