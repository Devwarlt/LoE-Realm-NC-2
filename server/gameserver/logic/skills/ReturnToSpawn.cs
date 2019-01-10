#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using Mono.Game;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class ReturnToSpawn : CycleBehavior
    {
        private readonly float speed;
        private bool once;
        private readonly float distance;
        private bool returned;

        public ReturnToSpawn(
            bool once = false,
            double speed = 2,
            float distance = 1
            )
        {
            this.once = once;
            this.speed = (float)speed / 10;
            this.distance = distance;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (!returned)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                    return;

                var dist = host.EntitySpeed(speed, time);
                var pos = (host as Enemy).SpawnPoint;
                var tx = pos.X;
                var ty = pos.Y;

                if (Math.Abs(tx - host.X) > distance || Math.Abs(ty - host.Y) > distance)
                {
                    var x = host.X;
                    var y = host.Y;
                    var vect = new Vector2(tx, ty) - new Vector2(host.X, host.Y);

                    vect.Normalize();
                    vect *= dist;
                    host.Move(host.X + vect.X, host.Y + vect.Y);
                    host.UpdateCount++;
                }

                if (host.X == pos.X && host.Y == pos.Y && once)
                {
                    once = true;
                    returned = true;
                }
            }
        }
    }
}