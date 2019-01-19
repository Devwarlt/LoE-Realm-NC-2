#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Charge : CycleBehavior
    {
        private readonly float range;
        private readonly float speed;
        private Cooldown coolDown;

        public Charge(
            double speed = 5,
            float range = 10,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.speed = (float)speed / 10;
            this.range = range;
            this.coolDown = coolDown.Normalize(4000);
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            ChargeState s = (state == null) ? new ChargeState() : (ChargeState)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            Player player = (Player)host.GetNearestEntity(range, null);

            if (player == null)
                return;

            float length = new Vector2(player.X - host.X, player.Y - host.Y).Length;

            if (s.RemainingTime <= 0)
            {
                if (s.Direction == Vector2.Zero)
                {
                    if (player.X != host.X && player.Y != host.Y)
                    {
                        s.Direction = new Vector2(player.X - host.X, player.Y - host.Y);
                        float dist = s.Direction.Length;
                        s.Direction.Normalize();
                        s.RemainingTime = (int)(dist / host.EntitySpeed(speed, time) * 1000);
                        Status = CycleStatus.InProgress;
                    }
                }
                else
                {
                    s.Direction = Vector2.Zero;
                    s.RemainingTime = coolDown.Next(Random);
                    Status = CycleStatus.Completed;
                }
            }

            if (s.Direction != Vector2.Zero)
            {
                float dist = host.EntitySpeed(speed, time);
                host.ValidateAndMove(host.X + s.Direction.X * dist, host.Y + s.Direction.Y * dist);
                host.UpdateCount++;
                Status = CycleStatus.InProgress;
            }

            if (length <= 0.5)
            {
                s.Direction = Vector2.Zero;
                s.RemainingTime = 1000;
                Status = CycleStatus.Completed;
            }

            s.RemainingTime -= time.ElapsedMsDelta;

            state = s;
        }

        private class ChargeState
        {
            public Vector2 Direction;
            public int RemainingTime;
        }
    }
}