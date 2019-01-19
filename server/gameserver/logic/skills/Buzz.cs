#region

using LoESoft.GameServer.realm;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Buzz : CycleBehavior
    {
        private readonly float range;
        private readonly float speed;
        private Cooldown coolDown;

        public Buzz(
            double speed = 5,
            double range = 0.5,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.speed = (float)speed / 10;
            this.range = (float)range;
            this.coolDown = coolDown.Normalize(1);
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = new BuzzStorage();
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            BuzzStorage storage = (BuzzStorage)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            if (storage.RemainingTime > 0)
            {
                storage.RemainingTime -= time.ElapsedMsDelta;
                Status = CycleStatus.NotStarted;
            }
            else
            {
                Status = CycleStatus.InProgress;

                if (storage.RemainingDistance <= 0)
                {
                    do
                    {
                        storage.Direction = new Vector2(Random.Next(-1, 2), Random.Next(-1, 2));
                    } while (storage.Direction.X == 0 && storage.Direction.Y == 0);
                    storage.Direction.Normalize();
                    storage.RemainingDistance = range;
                    Status = CycleStatus.Completed;
                }

                float dist = host.EntitySpeed(speed, time);

                host.ValidateAndMove(host.X + storage.Direction.X * dist, host.Y + storage.Direction.Y * dist);
                host.UpdateCount++;

                storage.RemainingDistance -= dist;
            }

            state = storage;
        }

        private class BuzzStorage
        {
            public Vector2 Direction;
            public float RemainingDistance;
            public int RemainingTime;
        }
    }
}