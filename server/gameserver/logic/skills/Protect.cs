#region

using LoESoft.GameServer.realm;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Protect : CycleBehavior
    {
        private readonly float sightRange;
        private readonly ushort target;
        private readonly float protectRange;
        private readonly float reprotectRange;
        private readonly float speed;

        public Protect(
            double speed,
            string target,
            double sightRange = 12,
            double protectRange = 4,
            double reprotectRange = 1
            )
        {
            this.speed = (float)speed / 10;
            this.target = BehaviorDb.InitGameData.IdToObjectType[target];
            this.sightRange = (float)sightRange;
            this.protectRange = (float)protectRange;
            this.reprotectRange = (float)reprotectRange;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            ProtectState s;
            if (state == null)
                s = ProtectState.DontKnowWhere;
            else
                s = (ProtectState)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            Entity entity = host.GetNearestEntity(sightRange, target);
            Vector2 vect;
            switch (s)
            {
                case ProtectState.DontKnowWhere:
                    if (entity != null)
                    {
                        s = ProtectState.Protecting;
                        goto case ProtectState.Protecting;
                    }
                    break;

                case ProtectState.Protecting:
                    if (entity == null)
                    {
                        s = ProtectState.DontKnowWhere;
                        break;
                    }
                    vect = new Vector2(entity.X - host.X, entity.Y - host.Y);
                    if (vect.Length > reprotectRange)
                    {
                        Status = CycleStatus.InProgress;
                        vect.Normalize();
                        float dist = host.EntitySpeed(speed, time);
                        host.ValidateAndMove(host.X + vect.X * dist, host.Y + vect.Y * dist);
                        host.UpdateCount++;
                    }
                    else
                    {
                        Status = CycleStatus.Completed;
                        s = ProtectState.Protected;
                    }
                    break;

                case ProtectState.Protected:
                    if (entity == null)
                    {
                        s = ProtectState.DontKnowWhere;
                        break;
                    }
                    Status = CycleStatus.Completed;
                    vect = new Vector2(entity.X - host.X, entity.Y - host.Y);
                    if (vect.Length > protectRange)
                    {
                        s = ProtectState.Protecting;
                        goto case ProtectState.Protecting;
                    }
                    break;
            }

            state = s;
        }

        private enum ProtectState
        {
            DontKnowWhere,
            Protecting,
            Protected,
        }
    }
}