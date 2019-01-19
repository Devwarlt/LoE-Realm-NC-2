#region

using LoESoft.GameServer.realm;
using Mono.Game;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    internal class Circle : CycleBehavior
    {
        private class OrbitState
        {
            public float Speed;
            public float Radius;
            public int Direction;
        }

        private float speed;
        private float sightRange;
        private float radius;
        private ushort? target;
        private float speedVariance;
        private float radiusVariance;
        private bool? orbitClockwise;

        public Circle(
            double speed = 5,
            double radius = 5,
            double sightRange = 10,
            string target = null,
            double? speedVariance = null,
            double? radiusVariance = null,
            bool? orbitClockwise = false
            )
        {
            this.speed = (float)speed;
            this.radius = (float)radius;
            this.sightRange = (float)sightRange;
            this.target = target == null ? null : (ushort?)BehaviorDb.InitGameData.IdToObjectType[target];
            this.speedVariance = (float)(speedVariance ?? speed * 0.1);
            this.radiusVariance = (float)(radiusVariance ?? speed * 0.1);
            this.orbitClockwise = orbitClockwise;
        }

        protected override void OnStateEntry(realm.Entity host, realm.RealmTime time, ref object state)
        {
            int orbitDir;
            if (orbitClockwise == null)
                orbitDir = (Random.NextDouble() > .5) ? 1 : -1;
            else
                orbitDir = ((bool)orbitClockwise) ? 1 : -1;

            state = new OrbitState()
            {
                Speed = speed + speedVariance * (float)(Random.NextDouble() * 2 - 1),
                Radius = radius + radiusVariance * (float)(Random.NextDouble() * 2 - 1),
                Direction = orbitDir
            };
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            OrbitState s = (OrbitState)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffects.Paralyzed))
                return;

            Entity entity = host.GetNearestEntity(sightRange, target);

            if (entity != null)
            {
                double angle;
                if (host.Y == entity.Y && host.X == entity.X)
                    angle = Math.Atan2(host.Y - entity.Y + (Random.NextDouble() * 2 - 1), host.X - entity.X + (Random.NextDouble() * 2 - 1));
                else
                    angle = Math.Atan2(host.Y - entity.Y, host.X - entity.X);
                float angularSpd = s.Direction * host.EntitySpeed(s.Speed, time) / s.Radius;
                angle += angularSpd;

                double x = entity.X + Math.Cos(angle) * s.Radius;
                double y = entity.Y + Math.Sin(angle) * s.Radius;
                Vector2 vect = new Vector2((float)x, (float)y) - new Vector2(host.X, host.Y);
                vect.Normalize();
                vect *= host.EntitySpeed(s.Speed, time);

                host.ValidateAndMove(host.X + vect.X, host.Y + vect.Y);
                host.UpdateCount++;

                Status = CycleStatus.InProgress;
            }

            state = s;
        }
    }
}