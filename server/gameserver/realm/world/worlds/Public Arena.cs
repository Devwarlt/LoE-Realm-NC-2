#region

using CA.Extensions.Concurrent;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public interface IArena { }

    public class PublicArena : World, IArena
    {
        private bool ready = true;
        private bool waiting = false;
        private int wave = 1;
        private List<string> entities = new List<string>();
        private Random rng = new Random();

        private readonly Dictionary<int, Action<Player>> WaveReward = new Dictionary<int, Action<Player>>()
        {
            { 10, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 10:");
                player.SendInfo("- 1,000 EXP");
                player.SendInfo("- 50 Fame");
                player.Experience += 1000;
                player.FakeExperience += 1000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 50);
            } },
            { 20, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 20:");
                player.SendInfo("- 5,000 EXP");
                player.SendInfo("- 100 Fame");
                player.Experience += 5000;
                player.FakeExperience += 5000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 100);
            } },
            { 30, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 30:");
                player.SendInfo("- 10,000 EXP");
                player.SendInfo("- 250 Fame");
                player.Experience += 10000;
                player.FakeExperience += 10000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 250);
            } },
            { 40, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 40:");
                player.SendInfo("- 25,000 EXP");
                player.SendInfo("- 500 Fame");
                player.Experience += 25000;
                player.FakeExperience += 25000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 500);
            } },
            { 50, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 50:");
                player.SendInfo("- 50,000 EXP");
                player.SendInfo("- 1,000 Fame");
                player.Experience += 50000;
                player.FakeExperience += 50000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 1000);
            } },
            { 60, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 60:");
                player.SendInfo("- 100,000 EXP");
                player.SendInfo("- 2,500 Fame");
                player.Experience += 100000;
                player.FakeExperience += 100000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 2500);
            } },
            { 70, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 70:");
                player.SendInfo("- 250,000 EXP");
                player.SendInfo("- 5,000 Fame");
                player.Experience += 250000;
                player.FakeExperience += 250000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 5000);
            } },
            { 80, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 80:");
                player.SendInfo("- 500,000 EXP");
                player.SendInfo("- 10,000 Fame");
                player.Experience += 500000;
                player.FakeExperience += 500000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 10000);
            } },
            { 90, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 90:");
                player.SendInfo("- 1,000,000 EXP");
                player.SendInfo("- 25,000 Fame");
                player.Experience += 1000000;
                player.FakeExperience += 1000000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 25000);
            } },
            { 100, (player) => {
                player.SendInfo("[Public Arena] Reward of wave 100:");
                player.SendInfo("- 2,500,000 EXP");
                player.SendInfo("- 50,000 Fame");
                player.Experience += 2500000;
                player.FakeExperience += 2500000;

                GameServer.Manager.Database.UpdateFame(player.Client.Account, 50000);
            } }
        };

        private readonly List<(int fromX, int fromY, int toX, int toY)> AvailablePositions
            = new List<(int fromX, int fromY, int toX, int toY)>()
        {
                (16, 16, 22, 22), // top-left square
                (41, 16, 47, 22), // top-right square
                (16, 41, 22, 47), // bottom-left square
                (41, 41, 47, 47) // bottom-right square
        };

        public PublicArena()
        {
            Name = "Public Arena";
            Dungeon = true;
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
            MaxPlayersCount = 20;
        }

        protected override void Init() => LoadMap("pub-arena-v1", MapType.Json);

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            if (Players.Count == 0)
                return;

            InitArena(time);
        }

        protected void InitArena(RealmTime time)
        {
            if (IsEmpty())
            {
                if (ready)
                {
                    if (waiting)
                        return;

                    ready = false;

                    foreach (var i in Players)
                    {
                        if (i.Value.Client == null)
                            continue;

                        i.Value.Client.SendMessage(new IMMINENT_ARENA_WAVE
                        {
                            CurrentRuntime = time.ElapsedMsDelta,
                            Wave = wave
                        });

                        if (WaveReward.ContainsKey(wave))
                            WaveReward[wave].Invoke(i.Value);
                    }

                    waiting = true;

                    Timers.Add(new WorldTimer(5000, (world, t) =>
                    {
                        ready = false;
                        waiting = false;
                        PopulateArena();
                    }));

                    wave++;
                }

                ready = true;
            }
        }

        private List<string> SeedArena(int currentWave)
        {
            var newEntities = new List<string>();

            if ((currentWave % 2 == 0) && (currentWave < 10))
                for (int i = 0; i < currentWave / 2; i++)
                    newEntities.Add(EntityWeak[rng.Next(EntityWeak.Count - 1)]);

            if (currentWave % 3 == 0)
                for (int i = 0; i < currentWave / 2; i++)
                    newEntities.Add(EntityNormal[rng.Next(EntityNormal.Count - 1)]);

            if ((currentWave % 2 == 0) && (currentWave >= 10))
                for (int i = 0; i < currentWave / 2; i++)
                    newEntities.Add(EntityGod[rng.Next(EntityGod.Count - 1)]);

            if ((currentWave % 2 == 0) && (currentWave >= 20))
                for (int i = 0; i < currentWave / 8; i++)
                    newEntities.Add(DreamMinions[rng.Next(DreamMinions.Count - 1)]);

            if ((currentWave % 5 == 0) && (currentWave >= 20))
                newEntities.Add(EntityQuest[rng.Next(EntityQuest.Count - 1)]);

            return newEntities;
        }

        private void PopulateArena()
        {
            try
            {
                entities = SeedArena(wave);

                while (entities.Count == 0)
                    entities = SeedArena(wave + 1);

                foreach (string entity in entities)
                {
                    var (fromX, fromY, toX, toY) = AvailablePositions[rng.Next(AvailablePositions.Count - 1)];
                    var position = new Position
                    {
                        X = rng.Next(fromX, toX),
                        Y = rng.Next(fromY, toY)
                    };
                    var enemy = Entity.Resolve(GameServer.Manager.GameData.IdToObjectType[entity]);

                    if (EntityQuest.Contains(entity))
                        enemy.Move(32f, 32f);
                    else
                        enemy.Move(position.X, position.Y);

                    EnterWorld(enemy);
                }

                entities.Clear();
            }
            catch (Exception)
            { return; }
        }

        private bool IsEmpty() => Enemies.ValueWhereAsParallel(_ => !_.IsPet).Length == 0;

        public bool OutOfBounds(float x, float y)
            => (y < Map.Height && y > 0 && x < Map.Width && x > 0) ?
            Map[(int)x, (int)y].Region == TileRegion.Outside_Arena : true;

        protected void CheckOutOfBounds()
        {
            foreach (var i in Enemies)
                if (OutOfBounds(i.Value.X, i.Value.Y))
                    LeaveWorld(i.Value);
        }

        #region "Entities"

        protected readonly List<string> EntityWeak = new List<string>
        {
            "Flamer King",
            "Lair Skeleton King",
            "Native Fire Sprite",
            "Native Ice Sprite",
            "Native Magic Sprite",
            "Nomadic Shaman",
            "Ogre King",
            "Orc King",
            "Red Spider",
            "Sand Phantom",
            "Swarm",
            "Tawny Warg",
            "Vampire Bat",
            "Wasp Queen",
            "Weretiger"
        };

        protected readonly List<string> EntityNormal = new List<string>
        {
            "Aberrant of Oryx",
            "Abomination of Oryx",
            "Adult White Dragon",
            "Assassin of Oryx",
            "Great Lizard",
            "Minotaur",
            "Monstrosity of Oryx",
            "Phoenix Reborn",
            "Shambling Sludge",
            "Urgle"
        };

        protected readonly List<string> EntityGod = new List<string>
        {
            "Beholder",
            "Ent God",
            "Flying Brain",
            "Djinn",
            "Ghost God",
            "Leviathan",
            "Medusa",
            "Slime God",
            "Sprite God",
            "White Demon"
        };

        protected readonly List<string> DreamMinions = new List<string>
        {
            "Muzzlereaper",
            "Guzzlereaper",
            "Silencer",
            "Nightmare"
        };

        protected readonly List<string> EntityQuest = new List<string>
        {
            "Mysterious Crystal",
            "Grand Sphinx",
            "Stheno the Snake Queen",
            "Frog King",
            "Cube God",
            "Skull Shrine",
            "Lord of the Lost Lands",
            "Pentaract Tower",
            "Oryx the Mad God 2",
            "Maurth the Succubus Princess",
            "Eyeguard of Surrender",
            "Lost Prisoner Soul",
            "shtrs Defense System",
            "Undertaker the Great Juggernaut",
            "Lucky Djinn",
            "Lucky Ent God",
            "Dyno Bot"
        };

        #endregion "Entities"
    }
}
