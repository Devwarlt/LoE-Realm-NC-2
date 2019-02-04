#region

using LoESoft.GameServer.realm;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class StayCloseToSpawn : CycleBehavior
    {
        private readonly int range;
        private readonly float speed;

        public StayCloseToSpawn(
            double speed,
            int range = 5
            )
        {
            this.speed = (float)speed / 10;
            this.range = range;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = new Vector2(host.X, host.Y);
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            Vector2 vect = (Vector2)state;
            var l = (vect - new Vector2(host.X, host.Y)).Length;
            if (l > range)
            {
                vect -= new Vector2(host.X, host.Y);
                vect.Normalize();
                float dist = host.EntitySpeed(speed, time);
                host.ValidateAndMove(host.X + vect.X * dist, host.Y + vect.Y * dist);
                host.UpdateCount++;

                Status = CycleStatus.InProgress;
            }
            else
                Status = CycleStatus.Completed;
        }
    }
}