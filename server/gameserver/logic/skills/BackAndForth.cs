#region

using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class BackAndForth : CycleBehavior
    {
        private readonly int distance;
        private readonly float speed;

        public BackAndForth(
            double speed = 5,
            int distance = 5
            )
        {
            this.speed = (float)speed / 10;
            this.distance = distance;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            float distance;
            if (state == null)
                distance = this.distance;
            else
                distance = (float)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            float dist = host.EntitySpeed(speed, time);

            if (distance > 0)
            {
                Status = CycleStatus.InProgress;
                host.ValidateAndMove(host.X + dist, host.Y);
                host.UpdateCount++;
                distance -= dist;
                if (distance <= 0)
                {
                    distance = -this.distance;
                    Status = CycleStatus.Completed;
                }
            }
            else
            {
                Status = CycleStatus.InProgress;
                host.ValidateAndMove(host.X - dist, host.Y);
                host.UpdateCount++;
                distance += dist;
                if (distance >= 0)
                {
                    distance = this.distance;
                    Status = CycleStatus.Completed;
                }
            }

            state = distance;
        }
    }
}