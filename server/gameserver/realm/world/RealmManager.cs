#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using LoESoft.GameServer.logic;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.realm.commands;
using LoESoft.GameServer.realm.entity.merchant;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm
{
    public class RealmManager
    {
        public static Dictionary<string, int> QuestPortraits = new Dictionary<string, int>();
        public static List<string> CurrentRealmNames = new List<string>();

        public static List<string> Realms = new List<string>
        {
            "Djinn",
            "Medusa",
            "Beholder",
        };

        public const int MAX_REALM_PLAYERS = 85;

        public ConcurrentDictionary<string, ClientData> ClientManager { get; private set; }
        public ConcurrentDictionary<int, World> Worlds { get; private set; }
        public ConcurrentDictionary<string, World> LastWorld { get; private set; }
        public Random Random { get; }
        public BehaviorDb Behaviors { get; private set; }
        public ChatManager Chat { get; private set; }
        public ISManager InterServer { get; private set; }
        public CommandManager Commands { get; private set; }
        public EmbeddedData GameData { get; private set; }
        public string InstanceId { get; private set; }
        public LogicTicker Logic { get; private set; }
        public int MaxClients { get; private set; }
        public RealmPortalMonitor Monitor { get; private set; }
        public Database Database { get; private set; }
        public bool Terminating { get; private set; }
        public int TPS { get; private set; }
        public System.Timers.Timer SaveMonitor { get; private set; }

        private ConcurrentDictionary<string, Vault> Vaults { get; set; }

#pragma warning restore CS0649 // Field 'RealmManager.network' is never assigned to, and will always have its default value null
        private int nextWorldId;

        public RealmManager(Database db)
        {
            MaxClients = Settings.NETWORKING.MAX_CONNECTIONS;
            TPS = Settings.GAMESERVER.TICKETS_PER_SECOND;
            ClientManager = new ConcurrentDictionary<string, ClientData>();
            Worlds = new ConcurrentDictionary<int, World>();
            LastWorld = new ConcurrentDictionary<string, World>();
            Vaults = new ConcurrentDictionary<string, Vault>();
            Random = new Random();
            Database = db;
            SaveMonitor = new System.Timers.Timer(60000) { AutoReset = true };
            SaveMonitor.Elapsed += delegate
            {
                if (ClientManager.Keys.Count != 0)
                    ClientManager.Values.Select(client => { client.Client?.Save(); return client; }).ToList();
            };
        }

        #region "Initialize, Run and Stop"

        public void Initialize()
        {
            GameData = new EmbeddedData();
            SaveMonitor.Start();
            Behaviors = new BehaviorDb(this);

            Player.HandleQuests(GameData);
            Merchant.HandleMerchant(GameData);

            AddWorld((int)WorldID.NEXUS_ID, Worlds[0] = new Nexus());
            AddWorld((int)WorldID.MARKET, new ClothBazaar());
            AddWorld((int)WorldID.TEST_ID, new Test());
            AddWorld((int)WorldID.TUT_ID, new Tutorial(true));
            AddWorld((int)WorldID.DAILY_QUEST_ID, new DailyQuestRoom());
            AddWorld((int)WorldID.DRASTA_CITADEL_ID, new DrastaCitadel());
            AddWorld((int)WorldID.DREAM_ISLAND, new DreamIsland());

            Monitor = new RealmPortalMonitor(this);

            AddWorld(GameWorld.AutoName(1, true));

            InterServer = new ISManager(this);
            Chat = new ChatManager(this);
            Commands = new CommandManager(this);

            var npcs = new NPCs();
            npcs.Initialize(this);

            Log.Info($"\t- {NPCs.Database.Count}\tNPC{(NPCs.Database.Count > 1 ? "s" : "")}.");
        }

        public void Run()
        {
            Logic = new LogicTicker(this);

            var logic = new Task(() => Logic.TickLoop(), TaskCreationOptions.LongRunning);
            logic.ContinueWith(GameServer.Restart, TaskContinuationOptions.OnlyOnFaulted);
            logic.Start();
        }

        public void Stop()
        {
            SaveMonitor.Stop();
            Terminating = true;

            var saveAccountUnlock = new List<Client>();

            foreach (var cData in ClientManager.Values)
            {
                saveAccountUnlock.Add(cData.Client);
                TryDisconnect(cData.Client, DisconnectReason.STOPPING_REALM_MANAGER);
            }

            GameData.Dispose();
        }

        #endregion

        #region "Connection handlers"

        /** Disconnect Handler (LoESoft Games)
	    * Author: DV
	    * Original Idea: Miniguy
	    */

        public ConnectionProtocol TryConnect(Client client)
        {
            if (AccessDenied)
                return new ConnectionProtocol(false, ErrorIDs.ACCESS_DENIED_DUE_RESTART); // Prevent account in use issue along restart.
            else
            {
                try
                {
                    var _cData = new ClientData
                    {
                        ID = client.Account.AccountId,
                        Client = client,
                        DNS = client.Socket.RemoteEndPoint.ToString().Split(':')[0],
                        Registered = DateTime.Now
                    };

                    if (_cData.Client.Account.Banned)
                        return new ConnectionProtocol(false, ErrorIDs.ACCOUNT_BANNED);

                    if (ClientManager.Count >= MaxClients) // When server is full.
                        return new ConnectionProtocol(false, ErrorIDs.SERVER_FULL);

                    if (ClientManager.ContainsKey(_cData.ID))
                    {
                        if (_cData.Client != null)
                        {
                            TryDisconnect(ClientManager[_cData.ID].Client, DisconnectReason.OLD_CLIENT_DISCONNECT); // Old client.

                            return new ConnectionProtocol(ClientManager.TryAdd(_cData.ID, _cData), ErrorIDs.NORMAL_CONNECTION); // Normal connection with reconnect type.
                        }

                        return new ConnectionProtocol(false, ErrorIDs.LOST_CONNECTION); // User dropped connection while reconnect.
                    }

                    return new ConnectionProtocol(ClientManager.TryAdd(_cData.ID, _cData), ErrorIDs.NORMAL_CONNECTION); // Normal connection with reconnect type.
                }
                catch (Exception e) { Log.Error($"An error occurred.\n{e}"); }

                return new ConnectionProtocol(false, ErrorIDs.LOST_CONNECTION); // User dropped connection while reconnect.
            }
        }

        public void TryDisconnect(Client client, DisconnectReason reason = DisconnectReason.UNKNOW_ERROR_INSTANCE)
        {
            if (client == null)
                return;
            DisconnectHandler(client, reason == DisconnectReason.UNKNOW_ERROR_INSTANCE ? DisconnectReason.REALM_MANAGER_DISCONNECT : reason);
        }

        public void DisconnectHandler(Client client, DisconnectReason reason)
        {
            try
            {
                if (ClientManager.ContainsKey(client.Account.AccountId))
                {
                    ClientManager.TryRemove(client.Account.AccountId, out ClientData _disposableCData);

                    Log.Info($"[({(int)reason}) {reason.ToString()}] Disconnect player '{_disposableCData.Client.Account.Name} (Account ID: {_disposableCData.Client.Account.AccountId})'.");

                    _disposableCData.Client.Save();
                    _disposableCData.Client.State = ProtocolState.Disconnected;
                    _disposableCData.Client.Socket.Close();
                    _disposableCData.Client.Dispose();
                }
                else
                {
                    Log.Info($"[({(int)reason}) {reason.ToString()}] Disconnect player '{client.Account.Name} (Account ID: {client.Account.AccountId})'.");

                    client.Save();
                    client.State = ProtocolState.Disconnected;
                    client.Dispose();
                }
            }
            catch (NullReferenceException) { }
        }

        #endregion

        #region "World Utils"

        public World AddWorld(int id, World world)
        {
            if (world.Manager != null)
                throw new InvalidOperationException("World already added.");
            world.Id = id;
            Worlds[id] = world;
            OnWorldAdded(world);
            return world;
        }

        public World AddWorld(World world)
        {
            if (world.Manager != null)
                throw new InvalidOperationException("World already added.");
            world.Id = Interlocked.Increment(ref nextWorldId);
            Worlds[world.Id] = world;
            OnWorldAdded(world);
            return world;
        }

        public bool RemoveWorld(World world)
        {
            if (world.Manager == null)
                throw new InvalidOperationException("World is not added.");
            if (Worlds.TryRemove(world.Id, out World dummy))
            {
                try
                {
                    OnWorldRemoved(world);
                    world.Dispose();
                    GC.Collect();
                }
                catch (Exception) { }
                return true;
            }
            return false;
        }

        public void CloseWorld(World world)
        {
            Monitor.WorldRemoved(world);
        }

        public World GetWorld(int id)
        {
            if (!Worlds.TryGetValue(id, out World ret))
                return null;
            if (ret.Id == 0)
                return null;
            return ret;
        }

        public bool RemoveVault(string accountId)
        {
            return Vaults.TryRemove(accountId, out Vault dummy);
        }

        private void OnWorldAdded(World world)
        {
            if (world.Manager == null)
                world.Manager = this;

            if (world is GameWorld)
                Monitor.WorldAdded(world);
        }

        private void OnWorldRemoved(World world)
        {
            world.Manager = null;
            if (world is GameWorld)
                Monitor.WorldRemoved(world);
        }

        #endregion

        #region "Player Utils"

        public Player FindPlayer(string name)
        {
            if (name.Split(' ').Length > 1)
                name = name.Split(' ')[1];

            return (from i in Worlds
                    where i.Key != 0
                    from e in i.Value.Players
                    where string.Equals(e.Value.Client.Account.Name, name, StringComparison.CurrentCultureIgnoreCase)
                    select e.Value).FirstOrDefault();
        }

        public Player FindPlayerRough(string name)
        {
            Player dummy;
            foreach (KeyValuePair<int, World> i in Worlds)
                if (i.Key != 0)
                    if ((dummy = i.Value.GetUniqueNamedPlayerRough(name)) != null)
                        return dummy;
            return null;
        }

        public Vault PlayerVault(Client processor)
        {
            if (!Vaults.TryGetValue(processor.Account.AccountId, out Vault v))
                Vaults.TryAdd(processor.Account.AccountId, v = (Vault)AddWorld(new Vault(false, processor)));
            else
                v.Reload(processor);
            return v;
        }

        #endregion
    }

    public enum PendingPriority
    {
        Emergent,
        Destruction,
        Networking,
        Normal,
        Creation,
    }

    public class RealmTime
    {
        public long TickCount { get; set; }
        public long TotalElapsedMs { get; set; }
        public int TickDelta { get; set; }
        public int ElapsedMsDelta { get; set; }
    }

    public class TimeEventArgs : EventArgs
    {
        public TimeEventArgs(RealmTime time)
        {
            Time = time;
        }

        public RealmTime Time { get; private set; }
    }

    public class ConnectionProtocol
    {
        public bool Connected { get; private set; }
        public ErrorIDs ErrorID { get; private set; }

        public ConnectionProtocol(
            bool connected,
            ErrorIDs errorID
            )
        {
            Connected = connected;
            ErrorID = errorID;
        }
    }

    public class ClientData
    {
        public string ID { get; set; }
        public Client Client { get; set; }
        public string DNS { get; set; }
        public DateTime Registered { get; set; }
    }
}