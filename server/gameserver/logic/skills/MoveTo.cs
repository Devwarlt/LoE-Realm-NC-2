#region

using LoESoft.GameServer.realm;
using Mono.Game;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class MoveTo : CycleBehavior
    {
        private readonly float speed;
        private readonly float baseX;
        private readonly float baseY;
        private readonly bool isMapPosition;
        private readonly bool instant;
        private bool once;
        private bool returned;
        private float X;
        private float Y;

        public MoveTo(
            float X,
            float Y,
            double speed = 2,
            bool once = false,
            bool isMapPosition = false,
            bool instant = false
            )
        {
            this.isMapPosition = isMapPosition;
            this.speed = (float)speed / 10;
            this.once = once;
            this.instant = instant;
            baseX = X;
            baseY = Y;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            if (!isMapPosition)
            {
                X = baseX + host.X;
                Y = baseY + host.Y;
            }
            else
            {
                X = baseX;
                Y = baseY;
            }

            if (instant)
            {
                host.Move(X, Y);
                host.UpdateCount++;
            }
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (instant)
                return;

            if (!returned)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                    return;

                float spd = host.EntitySpeed(speed, time);

                if (Math.Abs(X - host.X) > 0.5 || Math.Abs(Y - host.Y) > 0.5)
                {
                    Vector2 vect = new Vector2(X, Y) - new Vector2(host.X, host.Y);
                    vect.Normalize();
                    vect *= spd;
                    host.Move(host.X + vect.X, host.Y + vect.Y);
                    host.UpdateCount++;

                    if (host.X == X && host.Y == Y && once)
                    {
                        once = true;
                        returned = true;
                    }
                }
            }
        }
    }
}