#region

using LoESoft.Core.config;
using LoESoft.Core.models;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.terrain;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace LoESoft.GameServer.realm
{
    public interface IDungeon { }

    public enum WorldID : int
    {
        TUT_ID = -1,
        NEXUS_ID = -2,
        NEXUS_LIMBO = -3,
        VAULT_ID = -5,
        TEST_ID = -6,
        GAUNTLET = -7,
        WC = -8,
        ARENA = -9,
        MARKET = -11,
        DAILY_QUEST_ID = -13,
        DRASTA_CITADEL_ID = -14,
        DREAM_ISLAND = -15
    }

    public abstract class World : IDisposable
    {
        protected World()
        {
            Players = new ConcurrentDictionary<int, Player>();
            Enemies = new ConcurrentDictionary<int, Enemy>();
            Entities = new ConcurrentDictionary<int, Entity>();
            Quests = new ConcurrentDictionary<int, Enemy>();
            GameObjects = new ConcurrentDictionary<int, GameObject>();
            Timers = new List<WorldTimer>();
            ClientXml = ExtraXml = Empty<string>.Array;
            SafePlace = false;
            AllowTeleport = true;
            ShowDisplays = true;
            MaxPlayers = -1;
            SetMusic("main");
            Timers.Add(new WorldTimer(120 * 1000, (w, t) =>
            {
                canBeClosed = true;

                if (NeedsPortalKey)
                    PortalKeyExpired = true;
            }));
        }

        public void AddReconnectToPlayer(string id, Tuple<float, float> position) => ReconnectRequests.TryAdd(id, position);

        public Tuple<float, float> RemovePositionFromReconnect(string id)
        {
            if (ReconnectRequests.ContainsKey(id))
            {
                ReconnectRequests.TryRemove(id, out Tuple<float, float> position);
                return position;
            }

            return null;
        }

        private ConcurrentDictionary<string, Tuple<float, float>> ReconnectRequests { get; set; } = new ConcurrentDictionary<string, Tuple<float, float>>();

        public void BeginInit()
        {
            Seed = GameServer.Manager.Random.NextUInt32();
            PortalKey = Utils.RandomBytes(NeedsPortalKey ? 16 : 0);

            var worldtick = new Task(() => WorldTick());
            worldtick.Start();

            Init();
        }

        public bool IsTickRunning { get; set; }
        private ManualResetEvent _continueTick { get; set; } = new ManualResetEvent(true);

        public void EnableWorldTick() => _continueTick.Set();

        public void DisableWorldTick() => _continueTick.Reset();

        private async void WorldTick()
        {
            await Task.Delay(1000 / Settings.GAMESERVER.TICKETS_PER_SECOND); // 200 ms (5 TPS)

            _continueTick.WaitOne();

            IsTickRunning = true;

            try { Tick(GameServer.Manager.Logic.GameTime); }
            catch { }

            WorldTick();
        }

        public virtual void Tick(RealmTime time)
        {
            if (IsLimbo)
                return;

            if (Timers.Count != 0)
                for (var i = 0; i < Timers.Count; i++)
                    try
                    {
                        if (Timers[i] == null)
                            continue;

                        if (!Timers[i].Tick(this, time))
                            continue;

                        Timers.RemoveAt(i);

                        i--;
                    }
                    catch { }

            if (Players.Count != 0)
                foreach (var player in Players.Values)
                    player.Tick(time);

            if (EnemiesCollision != null)
            {
                var collisions = EnemiesCollision.GetActiveChunks(PlayersCollision).ToList();

                if (collisions.Count != 0)
                    foreach (var collision in collisions)
                        collision.Tick(time);

                if (GameObjects.Count != 0)
                    foreach (var gameobject in GameObjects.Values.Where(x => x is Decoy))
                        gameobject.Tick(time);
            }
            else
            {
                if (Enemies.Count != 0)
                    foreach (var enemy in Enemies.Values.Where(enemy => enemy != null))
                        enemy.Tick(time);

                if (GameObjects.Count != 0)
                    foreach (var gameobject in GameObjects.Values.Where(objects => objects != null))
                        gameobject.Tick(time);
            }

            if (Projectiles.Count != 0)
                foreach (var projectile in Projectiles.Keys.Where(projectile => projectile != null))
                    projectile.Tick(time);

            if (Players.Count != 0 || !canBeClosed || !IsDungeon())
                return;

            if (this is Vault vault)
                GameServer.Manager.RemoveVault(vault.AccountId);

            GameServer.Manager.RemoveWorld(this);
        }

        private int entityInc;
        private bool canBeClosed;
        public string ExtraVar = "Default";
        public bool IsLimbo { get; protected set; }
        public int Id { get; internal set; }
        public int Difficulty { get; protected set; }
        public string Name { get; protected set; }
        public string ClientWorldName { get; protected set; }
        public byte[] PortalKey { get; private set; }
        public bool PortalKeyExpired { get; private set; }
        public uint Seed { get; private set; }
        public virtual bool NeedsPortalKey => false;
        public ConcurrentDictionary<int, Entity> Entities { get; private set; }
        public ConcurrentDictionary<int, Player> Players { get; private set; }
        public ConcurrentDictionary<int, Enemy> Enemies { get; private set; }
        public ConcurrentDictionary<int, GameObject> GameObjects { get; private set; }
        public List<WorldTimer> Timers { get; }
        public int Background { get; protected set; }
        public CollisionMap<Entity> EnemiesCollision { get; private set; }
        public CollisionMap<Entity> PlayersCollision { get; private set; }
        public byte[,] Obstacles { get; private set; }
        public bool SafePlace { get; protected set; }
        public bool AllowTeleport { get; protected set; }
        public bool ShowDisplays { get; protected set; }
        public string[] ClientXml { get; protected set; }
        public string[] ExtraXml { get; protected set; }
        public bool Dungeon { get; protected set; }
        public bool Cave { get; protected set; }
        public bool Shaking { get; protected set; }
        public int MaxPlayers { get; protected set; }
        public Wmap Map { get; private set; }
        public ConcurrentDictionary<int, Enemy> Quests { get; }
        public ConcurrentDictionary<Projectile, object> Projectiles { get; set; } = new ConcurrentDictionary<Projectile, object>();

        public virtual World GetInstance(Client psr) => null;

        public bool IsPassable(int x, int y)
        {
            if (!Map.Contains(x, y))
                return false;
            WmapTile tile = Map[x, y];
            if (tile.TileDesc.NoWalk)
                return false;
            if (GameServer.Manager.GameData.ObjectDescs.TryGetValue(tile.ObjType, out ObjectDesc desc))
            {
                if (!desc.Static)
                    return false;
                if (desc.OccupySquare || desc.EnemyOccupySquare || desc.FullOccupy)
                    return false;
            }
            return true;
        }

        public int GetNextEntityId() => Interlocked.Increment(ref entityInc);

        public string SBName { get; set; }

        public string GetDisplayName()
        {
            if (!string.IsNullOrEmpty(SBName))
            {
                return SBName[0] == '{' ? Name : SBName;
            }
            return Name;
        }

        public bool Delete()
        {
            lock (this)
            {
                if (Players.Count > 0)
                    return false;
                Id = 0;
            }
            Map = null;
            Players = null;
            Enemies = null;
            Entities = null;
            Projectiles = null;
            GameObjects = null;
            return true;
        }

        protected abstract void Init();

        public string[] Music { get; set; }
        public string[] DefaultMusic { get; set; }

        public void SwitchMusic(params string[] music)
        {
            if (music.Length == 0)
                Music = DefaultMusic;
            else
                Music = music;
            BroadcastMessage(new SWITCH_MUSIC
            {
                Music = Music[new wRandom().Next(0, Music.Length)]
            }, null);
        }

        public void SetMusic(params string[] music)
        {
            Music = music;
            DefaultMusic = music;
        }

        public string GetMusic(wRandom rand = null)
        {
            if (Music.Length == 0)
                return "null";
            if (rand == null)
                rand = new wRandom();
            return Music[rand.Next(0, Music.Length)];
        }

        private void FromWorldMap(Stream dat)
        {
            var map = new Wmap(GameServer.Manager.GameData);
            Map = map;
            entityInc = 0;
            entityInc += Map.Load(dat, 0);

            int w = Map.Width, h = Map.Height;
            Obstacles = new byte[w, h];
            for (var y = 0; y < h; y++)
                for (var x = 0; x < w; x++)
                {
                    try
                    {
                        var tile = Map[x, y];
                        if (GameServer.Manager.GameData.Tiles[tile.TileId].NoWalk)
                            Obstacles[x, y] = 3;
                        if (GameServer.Manager.GameData.ObjectDescs.TryGetValue(tile.ObjType, out ObjectDesc desc))
                        {
                            if (desc.Class == "Wall" ||
                                desc.Class == "ConnectedWall" ||
                                desc.Class == "CaveWall")
                                Obstacles[x, y] = 2;
                            else if (desc.OccupySquare || desc.EnemyOccupySquare)
                                Obstacles[x, y] = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            EnemiesCollision = new CollisionMap<Entity>(0, w, h);
            PlayersCollision = new CollisionMap<Entity>(1, w, h);

            Projectiles.Clear();
            GameObjects.Clear();
            Enemies.Clear();
            Players.Clear();
            Entities.Clear();

            foreach (var i in Map.InstantiateEntities())
            {
                if (i.ObjectDesc != null &&
                    (i.ObjectDesc.OccupySquare || i.ObjectDesc.EnemyOccupySquare))
                    Obstacles[(int)(i.X - 0.5), (int)(i.Y - 0.5)] = 2;

                EnterWorld(i);
            }
        }

        public virtual int EnterWorld(Entity entity)
        {
            if (entity is Player player)
            {
                if (Settings.GetEventRate() != 1 && !player.Client.EventNotification)
                {
                    player.SendInfo(Settings.EVENT_MESSAGE);
                    player.Client.EventNotification = true;
                }

                TryAdd(player);
            }
            else
            {
                if (entity is Enemy enemy)
                    TryAdd(enemy);
                else
                {
                    if (entity is Projectile projectile)
                        TryAdd(projectile);
                    else
                    {
                        if (entity is GameObject gameObject)
                        {
                            TryAdd(gameObject);

                            if (entity is Decoy)
                                PlayersCollision.Insert(entity);
                            else
                                EnemiesCollision.Insert(entity);
                        }
                        else
                            return entity.Id;
                    }
                }
            }

            return entity.Id;
        }

        public virtual void LeaveWorld(Entity entity)
        {
            if (entity is Player)
                TryRemove(entity as Player);
            else
            {
                if (entity is Enemy)
                    TryRemove(entity as Enemy);
                else
                {
                    if (entity is Projectile)
                        TryRemove(entity as Projectile);
                    else
                    {
                        if (entity is GameObject)
                        {
                            TryRemove(entity as GameObject);

                            if (entity is Decoy)
                                PlayersCollision.Remove(entity);
                            else
                                EnemiesCollision.Remove(entity);
                        }
                    }
                }
            }

            try
            { entity.Dispose(); }
            catch { }

            entity = null;
        }

        private void TryAdd(Player player)
        {
            player.Id = GetNextEntityId();

            if (!Players.TryAdd(player.Id, player) || !Entities.TryAdd(player.Id, player))
                return;

            player.Init(this);

            PlayersCollision.Insert(player);
        }

        private void TryRemove(Player player)
        {
            if (!Players.TryRemove(player.Id, out Player dummy) || !Entities.TryRemove(player.Id, out Entity entity))
                return;

            PlayersCollision.Remove(player);
        }

        private void TryAdd(Enemy enemy)
        {
            enemy.Id = GetNextEntityId();

            if (enemy.ObjectDesc.Quest)
            {
                if (!Quests.TryAdd(enemy.Id, enemy) || !Enemies.TryAdd(enemy.Id, enemy) || !Entities.TryAdd(enemy.Id, enemy))
                    return;
            }
            else
            {
                if (!Enemies.TryAdd(enemy.Id, enemy) || !Entities.TryAdd(enemy.Id, enemy))
                    return;
            }

            enemy.Init(this);

            EnemiesCollision.Insert(enemy);
        }

        private void TryRemove(Enemy enemy)
        {
            if (enemy.ObjectDesc.Quest)
            {
                if (!Quests.TryRemove(enemy.Id, out Enemy dummy) || !Enemies.TryRemove(enemy.Id, out dummy) || !Entities.TryRemove(enemy.Id, out Entity entity))
                    return;
            }
            else
            {
                if (!Enemies.TryRemove(enemy.Id, out Enemy dummy) || !Entities.TryRemove(enemy.Id, out Entity entity))
                    return;
            }

            EnemiesCollision.Remove(enemy);
        }

        private void TryAdd(Projectile projectile)
        {
            projectile.Init(this);

            Projectiles.TryAdd(projectile, null);
        }

        private void TryRemove(Projectile projectile) => Projectiles.TryRemove(projectile, out object val);

        private void TryAdd(GameObject gameObject)
        {
            gameObject.Id = GetNextEntityId();

            if (!GameObjects.TryAdd(gameObject.Id, gameObject) || !Entities.TryAdd(gameObject.Id, gameObject))
                return;

            gameObject.Init(this);
        }

        private void TryRemove(GameObject gameObject)
        {
            if (string.IsNullOrEmpty(gameObject.Name) || string.IsNullOrWhiteSpace(gameObject.Name))
                return;

            if (!GameObjects.TryRemove(gameObject.Id, out GameObject dummy) || !Entities.TryRemove(gameObject.Id, out Entity entity))
                return;
        }

        public Entity GetEntity(int id)
        {
            if (Players.TryGetValue(id, out Player ret1))
                return ret1;

            if (Enemies.TryGetValue(id, out Enemy ret2))
                return ret2;

            if (GameObjects.TryGetValue(id, out GameObject ret3))
                return ret3;

            return null;
        }

        public Player GetPlayerByName(string name) =>
            (from i in Players
             where i.Value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
             select i.Value)
            .FirstOrDefault();

        public Player GetUniqueNamedPlayerRough(string name) =>
            (from i in Players
             where i.Value.CompareName(name)
             select i.Value)
            .FirstOrDefault();

        public void BroadcastMessage(Message msg, Player exclude)
        {
            foreach (var i in Players.Where(i => i.Value != exclude))
                i.Value.Client.SendMessage(msg);
        }

        public void BroadcastMessageSync(Message msg, Predicate<Player> exclude)
        {
            foreach (var i in Players.Where(i => exclude(i.Value)))
                i.Value.Client.SendMessage(msg);
        }

        public void BroadcastMessage(IEnumerable<Message> msgs, Player exclude)
        {
            foreach (var i in Players.Where(i => i.Value != exclude))
                i.Value.Client.SendMessage(msgs);
        }

        public void BroadcastMessageSync(IEnumerable<Message> msgs, Predicate<Player> exclude)
        {
            foreach (var i in Players.Where(i => exclude(i.Value)))
                i.Value.Client.SendMessage(msgs);
        }

        public bool IsFull =>
            MaxPlayers != -1
            && Players.Keys.Count >= MaxPlayers;

        public bool IsDungeon() =>
            !(this is IDungeon);

        protected void LoadMap(string embeddedResource, MapType type)
        {
            if (embeddedResource == null)
                return;

            string mapType = type == MapType.Json ? "json" : "wmap";
            string resource = embeddedResource.Replace($".{mapType}", "");
            var stream = typeof(RealmManager).Assembly.GetManifestResourceStream($"LoESoft.GameServer.realm.world.maps.{mapType}.{resource}.{mapType}");
            if (stream == null)
                throw new ArgumentException($"{mapType.ToUpper()} map resource " + nameof(resource) + " not found!");

            switch (type)
            {
                case MapType.Wmap:
                    FromWorldMap(stream);
                    break;

                case MapType.Json:
                    FromWorldMap(new MemoryStream(Json2Wmap.Convert(GameServer.Manager.GameData, new StreamReader(stream).ReadToEnd())));
                    break;

                default:
                    throw new ArgumentException("Invalid MapType");
            }
        }

        protected void LoadMap(string json)
        {
            FromWorldMap(new MemoryStream(Json2Wmap.Convert(GameServer.Manager.GameData, json)));
        }

        public void ChatReceived(string text)
        {
            foreach (var en in Enemies)
                en.Value.OnChatTextReceived(text);
            foreach (var en in GameObjects)
                en.Value.OnChatTextReceived(text);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public virtual void Dispose()
        {
            Map.Dispose();
            Players.Clear();
            Enemies.Clear();
            Quests.Clear();
            Projectiles.Clear();
            GameObjects.Clear();
            Timers.Clear();
            EnemiesCollision = null;
            PlayersCollision = null;
        }
    }

    public enum MapType
    {
        Wmap,
        Json
    }
}