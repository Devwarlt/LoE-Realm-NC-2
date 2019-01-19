#region

using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class ChangeSize : Behavior
    {
        private readonly int rate;
        private readonly int target;

        public ChangeSize(
            int rate,
            int target
            )
        {
            this.rate = rate;
            this.target = target;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            var cool = (int)state;

            if (cool <= 0)
            {
                if (host.Size != target)
                {
                    host.Size += rate;
                    if ((rate > 0 && host.Size > target) ||
                        (rate < 0 && host.Size < target))
                        host.Size = target;
                    host.UpdateCount++;
                }
                cool = 150;
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}