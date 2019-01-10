#region

using LoESoft.Core;
using LoESoft.Core.config;
using System;
using System.Collections.Concurrent;

#endregion

namespace LoESoft.GameServer.realm
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ISManager : InterServerChannel, IDisposable
    {
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
            public NetworkCode Code;
            public string Type;
        }

        private RealmManager Manager;

        public ISManager(RealmManager manager) : base(manager.Database, manager.InstanceId)
        {
            Manager = manager;

            AddHandler<NetworkMsg>(NETWORK, HandleNetwork);

            Publish(NETWORK, new NetworkMsg()
            {
                Code = NetworkCode.JOIN,
                Type = "World Server"
            });
        }

        private ConcurrentDictionary<string, int> availableInstance = new ConcurrentDictionary<string, int>();

        private long remaining = 2000;

        public void Tick(RealmTime t)
        {
            remaining -= t.ElapsedMsDelta;
            if (remaining < 0)
            {
                Publish(NETWORK, new NetworkMsg() { Code = NetworkCode.PING });
                remaining = 2000;

                foreach (var i in availableInstance.Keys)
                {
                    if (availableInstance.ContainsKey(i) && --availableInstance[i] == 0)
                        availableInstance.TryRemove(i, out int val);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            Publish(NETWORK, new NetworkMsg() { Code = NetworkCode.QUIT });
        }

        private void HandleNetwork(object sender, InterServerEventArgs<NetworkMsg> e)
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Local && e.InstanceId == null)
            {
                Publish(NETWORK, new NetworkMsg()   //for the new instances
                {
                    Code = NetworkCode.JOIN,
                    Type = "World Server"
                });
            }
            else
            {
                switch (e.Content.Code)
                {
                    case NetworkCode.JOIN:
                        if (availableInstance.TryAdd(e.InstanceId, 5))
                        {
                            Publish(NETWORK, new NetworkMsg()   //for the new instances
                            {
                                Code = NetworkCode.JOIN,
                                Type = "World Server"
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
        }
    }
}