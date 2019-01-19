#region

using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.transitions
{
    public class TimedTransition : Transition
    {
        private readonly bool randomized;
        private readonly int coolDown;

        public TimedTransition(int coolDown, string targetState, bool randomized = false)
            : base(targetState)
        {
            this.coolDown = coolDown;
            this.randomized = randomized;
        }

        protected override bool TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool;
            if (state == null)
                cool = randomized ? Random.Next(coolDown) : coolDown;
            else
                cool = (int)state;

            bool ret = false;
            if (cool <= 0)
            {
                ret = true;
                cool = coolDown;
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
            return ret;
        }
    }
}