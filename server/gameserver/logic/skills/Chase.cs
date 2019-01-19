#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Chase : CycleBehavior
    {
        private readonly float sightRange;
        private readonly int duration;
        private readonly float range;
        private readonly float speed;
        private Cooldown coolDown;

        public Chase(
            double speed = 5,
            double sightRange = 10.5,
            double range = 1,
            int duration = 0,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.speed = (float)speed / 10;
            this.sightRange = (float)sightRange;
            this.range = (float)range;
            this.duration = duration;
            this.coolDown = coolDown.Normalize(duration == 0 ? 0 : 1000);
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            FollowState s;

            if (state == null)
                s = new FollowState();
            else
                s = (FollowState)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            Entity en = host.GetNearestEntity(sightRange, null);

            if (!(en is Player))
                return;

            Player player = en as Player;
            Vector2 vect;

            switch (s.State)
            {
                case F.DontKnowWhere:
                    if (player != null && s.RemainingTime <= 0)
                    {
                        s.State = F.Acquired;
                        if (duration > 0)
                            s.RemainingTime = duration;
                        goto case F.Acquired;
                    }
                    if (s.RemainingTime > 0)
                        s.RemainingTime -= time.ElapsedMsDelta;
                    break;

                case F.Acquired:
                    if (player == null)
                    {
                        s.State = F.DontKnowWhere;
                        s.RemainingTime = 0;
                        break;
                    }
                    if (s.RemainingTime <= 0 && duration > 0)
                    {
                        s.State = F.DontKnowWhere;
                        s.RemainingTime = coolDown.Next(Random);
                        Status = CycleStatus.Completed;
                        break;
                    }
                    if (s.RemainingTime > 0)
                        s.RemainingTime -= time.ElapsedMsDelta;

                    vect = new Vector2(player.X - host.X, player.Y - host.Y);
                    if (vect.Length > range)
                    {
                        Status = CycleStatus.InProgress;
                        vect.X -= Random.Next(-2, 2) / 2f;
                        vect.Y -= Random.Next(-2, 2) / 2f;
                        vect.Normalize();
                        float dist = host.EntitySpeed(speed, time);
                        host.ValidateAndMove(host.X + vect.X * dist, host.Y + vect.Y * dist);
                        host.UpdateCount++;
                    }
                    else
                    {
                        Status = CycleStatus.Completed;
                        s.State = F.Resting;
                        s.RemainingTime = 0;
                    }
                    break;

                case F.Resting:
                    if (player == null)
                    {
                        s.State = F.DontKnowWhere;
                        if (duration > 0)
                            s.RemainingTime = duration;
                        break;
                    }
                    Status = CycleStatus.Completed;
                    vect = new Vector2(player.X - host.X, player.Y - host.Y);
                    if (vect.Length > range + 1)
                    {
                        s.State = F.Acquired;
                        s.RemainingTime = duration;
                        goto case F.Acquired;
                    }
                    break;
            }

            state = s;
        }

        private enum F
        {
            DontKnowWhere,
            Acquired,
            Resting
        }

        private class FollowState
        {
            public int RemainingTime;
            public F State;
        }
    }
}