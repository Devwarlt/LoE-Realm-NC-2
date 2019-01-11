#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Reproduce : Behavior
    {
        private readonly ushort? name;
        private readonly int max;
        private readonly double range;
        private readonly double radius;
        private Cooldown coolDown;

        public Reproduce(
            string name = null,
            double range = 10,
            int max = 5,
            double radius = 0,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.name = name == null ? null : (ushort?)BehaviorDb.InitGameData.IdToObjectType[name];
            this.range = range;
            this.max = max;
            this.coolDown = coolDown.Normalize(60000);
            this.radius = radius == -1 ? range : radius;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool;

            if (state == null)
                cool = coolDown.Next(Random);
            else
                cool = (int)state;

            if (cool <= 0)
            {
                int count = host.CountEntity(range, name ?? host.ObjectType);

                if (count < max)
                {
                    Entity entity = Entity.Resolve(name ?? host.ObjectType);

                    double targetX = host.X;
                    double targetY = host.Y;

                    int i = 0;
                    do
                    {
                        double angle = Random.NextDouble() * 2 * Math.PI;
                        targetX = host.X + radius * 0.5 * Math.Cos(angle);
                        targetY = host.Y + radius * 0.5 * Math.Sin(angle);
                        i++;
                    } while (targetX < host.Owner.Map.Width &&
                             targetY < host.Owner.Map.Height &&
                             targetX > 0 && targetY > 0 &&
                             host.Owner.Map[(int)targetX, (int)targetY].Terrain !=
                             host.Owner.Map[(int)host.X, (int)host.Y].Terrain &&
                             i < 10);

                    entity.Move((float)targetX, (float)targetY);
                    (entity as Enemy).Terrain = (host as Enemy).Terrain;
                    host.Owner.EnterWorld(entity);
                }
                cool = coolDown.Next(Random);
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}