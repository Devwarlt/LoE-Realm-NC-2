#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class TossObject : Behavior
    {
        private readonly ushort child;
        private readonly int coolDownOffset;
        private readonly bool randomToss;
        private readonly double range;
        private double? angle;
        private Cooldown coolDown;
        private readonly bool ignoreStun;
        private readonly bool invisiToss;

        public TossObject(
            string child,
            double range = 5,
            double? angle = null,
            Cooldown coolDown = new Cooldown(),
            int coolDownOffset = 0,
            bool randomToss = false,
            bool ignoreStun = false,
            bool invisiToss = false
            )
        {
            this.child = BehaviorDb.InitGameData.IdToObjectType[child];
            this.range = range;
            this.angle = angle * Math.PI / 180;
            this.coolDown = coolDown.Normalize();
            this.coolDownOffset = coolDownOffset;
            this.randomToss = randomToss;
            this.ignoreStun = ignoreStun;
            this.invisiToss = invisiToss;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = coolDownOffset;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool = (int)state;

            if (cool <= 0)
            {
                if (!ignoreStun)
                    if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                        return;
                double? tossAngle = randomToss ? Random.Next(0, 360) * Math.PI / 180 : angle;
                Entity en = null;
                if (tossAngle == null)
                    en = host.GetNearestEntity(range, null);
                if (tossAngle == null && en == null)
                    return;

                Position target = tossAngle == null ?
                new Position
                {
                    X = en.X,
                    Y = en.Y
                } :
                new Position
                {
                    X = host.X + (float)(range * Math.Cos(tossAngle.Value)),
                    Y = host.Y + (float)(range * Math.Sin(tossAngle.Value)),
                };

                if (!host.Owner.IsPassable((int)target.X, (int)target.Y))
                    return;

                if (host.Owner.Map[(int)target.X, (int)target.Y].TileDesc.NoWalk)
                    return;

                if (!invisiToss)
                    host.Owner.BroadcastMessage(new SHOWEFFECT
                    {
                        EffectType = EffectType.Throw,
                        Color = new ARGB(0xffffbf00),
                        TargetId = host.Id,
                        PosA = target
                    }, null);
                host.Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                {
                    Entity entity = Entity.Resolve(child);
                    entity.Move(target.X, target.Y);
                    if (entity is Enemy && host is Enemy)
                        (entity as Enemy).Terrain = (host as Enemy).Terrain;
                    world.EnterWorld(entity);
                }));
                cool = coolDown.Next(Random);
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}