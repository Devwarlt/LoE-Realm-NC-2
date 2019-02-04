#region

using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Timed : CycleBehavior
    {
        private readonly Behavior behavior;
        private readonly int period;

        public Timed(
            int period,
            Behavior behavior
            )
        {
            this.behavior = behavior;
            this.period = period;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            behavior.OnStateEntry(host, time);
            state = period;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int period = (int)state;

            behavior.Tick(host, time);
            Status = CycleStatus.InProgress;

            period -= time.ElapsedMsDelta;
            if (period <= 0)
            {
                period = this.period;
                Status = CycleStatus.Completed;
                //......- -
                if (behavior is Prioritize)
                    host.StoredBehaviors[behavior] = -1;
            }

            state = period;
        }
    }
}