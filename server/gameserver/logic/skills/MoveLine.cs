using LoESoft.GameServer.realm;
using Mono.Game;
using System;

namespace LoESoft.GameServer.logic.behaviors
{
    internal class MoveLine : CycleBehavior
    {
        private readonly float _speed;
        private readonly float _direction;

        public MoveLine(double speed, double direction = 0)
        {
            _speed = (float)speed / 10;
            _direction = (float)direction * (float)Math.PI / 180;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            Status = CycleStatus.NotStarted;
            if (host.HasConditionEffect(ConditionEffects.Paralyzed))
                return;

            Status = CycleStatus.InProgress;
            var vect = new Vector2((float)Math.Cos(_direction), (float)Math.Sin(_direction));
            var dist = host.EntitySpeed(_speed, time);
            host.ValidateAndMove(host.X + vect.X * dist, host.Y + vect.Y * dist);
        }
    }
}