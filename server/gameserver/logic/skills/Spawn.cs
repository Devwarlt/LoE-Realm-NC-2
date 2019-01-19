#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Spawn : Behavior
    {
        private readonly ushort children;
        private readonly int initialSpawn;
        private readonly int maxChildren;
        private Cooldown coolDown;

        public Spawn(
            string children,
            int maxChildren = 5,
            double initialSpawn = 0.5,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.children = BehaviorDb.InitGameData.IdToObjectType[children];
            this.maxChildren = maxChildren;
            this.initialSpawn = (int)(maxChildren * initialSpawn);
            this.coolDown = coolDown.Normalize(0);
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = new SpawnState
            {
                CurrentNumber = initialSpawn,
                RemainingTime = coolDown.Next(Random)
            };
            for (int i = 0; i < initialSpawn; i++)
            {
                Entity entity = Entity.Resolve(children);

                entity.Move(
                    host.X + (float)(Random.NextDouble() * 0.5),
                    host.Y + (float)(Random.NextDouble() * 0.5));
                if (host is Enemy && entity is Enemy)
                    (entity as Enemy).Terrain = (host as Enemy).Terrain;
                host.Owner.EnterWorld(entity);
            }
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            SpawnState spawn = (SpawnState)state;

            if (spawn.RemainingTime <= 0 && spawn.CurrentNumber < maxChildren)
            {
                Entity entity = Entity.Resolve(children);

                entity.Move(host.X, host.Y);
                if (host is Enemy)
                    (entity as Enemy).Terrain = (host as Enemy).Terrain;
                host.Owner.EnterWorld(entity);
                spawn.RemainingTime = coolDown.Next(Random);
                spawn.CurrentNumber++;
            }
            else
                spawn.RemainingTime -= time.ElapsedMsDelta;
        }

        private class SpawnState
        {
            public int CurrentNumber;
            public int RemainingTime;
        }
    }
}