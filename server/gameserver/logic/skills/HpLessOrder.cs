#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class HpLessOrder : Behavior
    {
        private readonly float dist;
        private readonly float threshold;
        private readonly ushort children;
        private readonly string targetStateName;
        private State targetState;

        public HpLessOrder(
            double dist,
            double threshold,
            string children,
            string targetStateName
            )
        {
            this.dist = (float)dist;
            this.threshold = (float)threshold;
            this.children = BehaviorDb.InitGameData.IdToObjectType[children];
            this.targetStateName = targetStateName;
        }

        private static bool CheckHp(Entity host, double threshold)
        {
            if (threshold > 1.0)
                return (host as Enemy).HP < threshold;
            return ((host as Enemy).HP / host.ObjectDesc.MaxHP) < threshold;
        }

        private static State FindState(State state, string name)
        {
            if (state.Name == name)
                return state;
            State ret;
            foreach (State i in state.States)
            {
                if ((ret = FindState(i, name)) != null)
                    return ret;
            }
            return null;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (CheckHp(host, threshold))
            {
                if (targetState == null)
                    targetState = FindState(GameServer.Manager.Behaviors.Definitions[children].Item1, targetStateName);
                foreach (Entity i in host.GetNearestEntities(dist, children))
                    if (!i.CurrentState.Is(targetState))
                        i.SwitchTo(targetState);
            }
        }
    }
}