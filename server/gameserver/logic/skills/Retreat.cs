#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Retreat : CycleBehavior
    {
        private readonly float range;
        private readonly float speed;

        public Retreat(
            double speed,
            double range = 8
            )
        {
            this.speed = (float)speed / 10;
            this.range = (float)range;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cooldown;
            if (state == null)
                cooldown = 1000;
            else
                cooldown = (int)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            Player player = (Player)host.GetNearestEntity(range, null);
            if (player != null)
            {
                Vector2 vect;
                vect = new Vector2(player.X - host.X, player.Y - host.Y);
                vect.Normalize();
                float dist = host.EntitySpeed(speed, time);
                host.ValidateAndMove(host.X + (-vect.X) * dist, host.Y + (-vect.Y) * dist);
                host.UpdateCount++;

                if (cooldown <= 0)
                {
                    Status = CycleStatus.Completed;
                    cooldown = 1000;
                }
                else
                {
                    Status = CycleStatus.InProgress;
                    cooldown -= time.ElapsedMsDelta;
                }
            }

            state = cooldown;
        }
    }
}