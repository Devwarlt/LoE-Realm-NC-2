#region

using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Sequence : Behavior
    {
        private readonly CycleBehavior[] children;

        public Sequence(
            params CycleBehavior[] children
            )
        {
            this.children = children;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            foreach (CycleBehavior i in children)
                i.OnStateEntry(host, time);
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int index;
            if (state == null)
                index = 0;
            else
                index = (int)state;

            children[index].Tick(host, time);
            if (children[index].Status == CycleStatus.Completed || children[index].Status == CycleStatus.NotStarted)
            {
                index++;
                if (index == children.Length)
                    index = 0;
            }

            state = index;
        }
    }
}