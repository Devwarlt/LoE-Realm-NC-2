#region

using CA.Extensions.Concurrent;
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

        public static List<string> Realms = new List<string>
        {
            "Djinn",
            "Medusa",
            "Beholder",
        };

        public static ConcurrentDictionary<string, bool> OldschoolPlayers = new ConcurrentDictionary<string, bool>();

        private readonly List<string> OldschoolPlayer = new List<string>()
        {
            "Norga", "Bilisha", "Maurth", "Blasphemy", "NPEtoPPEx", "Hallow",
            "K", "Banana", "IAmlawa", "FrostIof", "Adwubz", "LoliLust",
            "Fade", "Blunt", "stgg", "MERICA", "Dshoopy", "baconey",
            "Bituloss", "EthaNGold", "yasuo", "Zen", "GhostMaree",
            "Sebafra", "Backpackek", "Dev", "Zemagaia", "Six"
        };

        public const int MAX_REALM_PLAYERS = 85;

        public ClientManager GetManager { get; private set; }
        public ConcurrentDictionary<int, World> Worlds { get; private set; }
        public ConcurrentDictionary<string, World> LastWorld { get; private set; }
        public Random Random { get; }
        public BehaviorDb Behaviors { get; private set; }
        public ChatManager Chat { get; private set; }
        public CommandManager Commands { get; private set; }
        public EmbeddedData GameData { get; private set; }
        public NPCs NPCs { get; private set; }
        public string InstanceId { get; private set; }
        public LogicTicker Logic { get; private set; }
        public int MaxClients { get; private set; }
        public PortalMonitor Monitor { get; private set; }
        public Database Database { get; private set; }
        public bool Terminating { get; private set; }
        public int TPS { get; private set; }

        public ManualResetEvent TickerReady { get; } = new ManualResetEvent(false);

        private ConcurrentDictionary<string, Vault> Vaults { get; set; }

        private int nextWorldId;

        public RealmManager(Database db)
        {
            GetManager = new ClientManager();
            MaxClients = Settings.NETWORKING.MAX_CONNECTIONS;
            TPS = Settings.GAMESERVER.TICKETS_PER_SECOND;
            Worlds = new ConcurrentDictionary<int, World>();
            LastWorld = new ConcurrentDictionary<string, World>();
            Vaults = new ConcurrentDictionary<string, Vault>();
            Random = new Random();
            Database = db;
        }

        #region "Initialize, Run and Stop"

        public void Initialize()
        {
            foreach (var oldschool in OldschoolPlayer)
                OldschoolPlayers.TryAdd(oldschool, false);

            GameData = new EmbeddedData();
            Behaviors = new BehaviorDb();
            Chat = new ChatManager();
            Commands = new CommandManager();
            NPCs = new NPCs();

            Log.Info($"\t- {NPCs.Database.Count}\tNPC{(NPCs.Database.Count > 1 ? "s" : "")}.");
            Log._("Message", Message.Messages.Count);

            Settings.DISPLAY_SUPPORTED_VERSIONS();

            Log.Info("Initializing GameServer...");
            Log.Info("Initializing GameServer... OK!\n");

            Player.HandleQuests(GameData);
            Merchant.HandleMerchant(GameData);

            AddWorld((int)WorldID.NEXUS_ID, Worlds[0] = new Nexus());
            AddWorld((int)WorldID.MARKET, new ClothBazaar());
            AddWorld((int)WorldID.TEST_ID, new Test());
            AddWorld((int)WorldID.TUT_ID, new Tutorial(true));
            AddWorld((int)WorldID.DAILY_QUEST_ID, new DailyQuestRoom());
            AddWorld((int)WorldID.DRASTA_CITADEL_ID, new DrastaCitadel());
            AddWorld((int)WorldID.DREAM_ISLAND, new DreamIsland());

            Monitor = new PortalMonitor(this, Worlds[0]);

            if (Realm.AllRealmEvents.Count == 0)
                foreach (var realmevent in Realm.RealmEventCache)
                    Realm.AllRealmEvents.Add(realmevent.Name);

            AddWorld(GameWorld.AutoName(1, true));
        }

        public void Run()
        {
            Logic = new LogicTicker();

            var logic = new Task(() => Logic.TickLoop(), TaskCreationOptions.LongRunning);
            logic.ContinueWith(GameServer.Restart, TaskContinuationOptions.OnlyOnFaulted);
            logic.Start();

            TickerReady.Set();
        }

        public void Stop()
        {
            Terminating = true;

            var clients = GetManager.Clients.ValueWhereAsParallel(_ => _ != null);
            for (var i = 0; i < clients.Length; i++)
                TryDisconnect(clients[i], DisconnectReason.STOPPING_REALM_MANAGER);

            GameData.Dispose();
        }

        #endregion

        #region "Connection handlers"

        public ConnectionProtocol TryConnect(Client client) => GetManager.TryConnect(client);

        public void TryDisconnect(Client client, DisconnectReason reason = DisconnectReason.UNKNOW_ERROR_INSTANCE)
            => GetManager.TryDisconnect(client, reason);

        #endregion

        #region "World Utils"

        public void AddWorld(int id, World world)
        {
            if (world.Manager != null)
                throw new InvalidOperationException("World already added.");

            world.Id = id;
            Worlds[id] = world;

            OnWorldAdded(world);
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

        public bool RemoveWorld(World world, bool renewRealm = false)
        {
            if (world.Manager == null)
                throw new InvalidOperationException("World is not added.");

            if (Worlds.TryRemove(world.Id, out world))
            {
                OnWorldRemoved(world);
                return true;
            }
            else
                return false;
        }

        public World GetWorld(int id)
        {
            if (!Worlds.TryGetValue(id, out World ret))
                return null;

            if (ret.Id == 0)
                return null;

            return ret;
        }

        public bool RemoveVault(string accountId) => Vaults.TryRemove(accountId, out Vault dummy);

        private void OnWorldAdded(World world)
        {
            world.Manager = this;

            if (world is IRealm)
                Monitor.AddRealm(world);

            Log.Warn($"World {world.Id}({world.Name}) added. {Worlds.Count} Worlds existing.");
        }

        private void OnWorldRemoved(World world)
        {
            var isrealm = world is IRealm;
            var name = world.Name;

            Log.Warn($"World {world.Id}({world.Name}) removed.");

            Monitor.RemoveWorld(world);

            if (isrealm)
                AddWorld(GameWorld.AutoName(1, true, name));
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
