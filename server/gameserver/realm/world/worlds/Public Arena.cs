#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity;
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

        public PublicArena()
        {
            Name = "Public Arena";
            Dungeon = true;
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
        }

        protected override void Init() =>
            LoadMap("arena", MapType.Wmap);

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            if (Players.Count == 0)
                return;

            CheckOutOfBounds();

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

                    foreach (KeyValuePair<int, Player> i in Players)
                    {
                        if (i.Value.Client == null)
                            continue;

                        i.Value.Client.SendMessage(new IMMINENT_ARENA_WAVE
                        {
                            CurrentRuntime = time.ElapsedMsDelta,
                            Wave = wave
                        });
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
            List<string> newEntities = new List<string>();

            if ((currentWave % 2 == 0) && (currentWave < 10))
                for (int i = 0; i < currentWave / 2; i++)
                    newEntities.Add(EntityWeak[rng.Next(EntityWeak.Count - 1)]);

            if (currentWave % 3 == 0)
                for (int i = 0; i < currentWave / 3; i++)
                    newEntities.Add(EntityNormal[rng.Next(EntityNormal.Count - 1)]);

            if ((currentWave % 2 == 0) && (currentWave >= 10))
                for (int i = 0; i < currentWave / 4; i++)
                    newEntities.Add(EntityGod[rng.Next(EntityGod.Count - 1)]);

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
                    Position position = new Position { X = rng.Next(12, Map.Width) - 6, Y = rng.Next(12, Map.Height) - 6 };
                    Entity enemy = Entity.Resolve(GameServer.Manager.GameData.IdToObjectType[entity]);

                    enemy.Move(position.X, position.Y);
                    EnterWorld(enemy);
                }

                entities.Clear();
            }
            catch (Exception)
            { return; }
        }

        private int CheckIfEmpty()
        {
            int quantity = Enemies.Count;

            foreach (Enemy enemy in Enemies.Values)
                if (enemy.IsPet)
                    quantity--;

            return quantity;
        }

        private bool IsEmpty() =>
            CheckIfEmpty() == 0;

        public bool OutOfBounds(float x, float y) =>
            (Map.Height >= y && Map.Width >= x && x > -1 && y > 0) ?
                Map[(int)x, (int)y].Region == TileRegion.Outside_Arena :
                true;

        protected void CheckOutOfBounds()
        {
            foreach (KeyValuePair<int, Enemy> i in Enemies)
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
            "White Demon",
            // Dream Island
            "Muzzlereaper",
            "Guzzlereaper",
            "Silencer",
            "Nightmare",
            "Lost Prisoner Soul",
            "Eyeguard of Surrender"
        };

        protected readonly List<string> EntityQuest = new List<string>
        {
            "Crystal Prisoner",
            "Grand Sphinx",
            "Stheno the Snake Queen",
            "Frog King",
            "Cube God",
            "Skull Shrine",
            "Lord of the Lost Lands",
            "Pentaract Tower",
            "Oryx the Mad God 2",
            "Gigacorn"
        };

        #endregion "Entities"
    }
}