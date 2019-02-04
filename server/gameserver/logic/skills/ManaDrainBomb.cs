#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class ManaDrainBomb : Behavior
    {
        private readonly int damage;
        private readonly float radius;
        private readonly double range;
        private Cooldown coolDown;
        private double? direction;
        private ConditionEffectIndex effect;
        private int effectDuration;

        public ManaDrainBomb(
            double radius,
            int damage,
            double range = 5,
            double? direction = null,
            Cooldown coolDown = new Cooldown(),
            ConditionEffectIndex effect = ConditionEffectIndex.Hidden,
            int effectDuration = -1
            )
        {
            this.radius = (float)radius;
            this.damage = damage;
            this.range = range;
            this.direction = direction * Math.PI / 180;
            this.coolDown = coolDown.Normalize();
            this.effect = effect;
            this.effectDuration = effectDuration;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool = (int)state;

            if (cool <= 0)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                    return;

                Entity player = host.GetNearestEntity(range, null);

                if (player != null || direction != null)
                {
                    Position target;

                    if (direction != null)
                        target = new Position
                        {
                            X = host.X + (float)(range * Math.Cos(direction.Value)),
                            Y = host.Y + (float)(range * Math.Sin(direction.Value)),
                        };
                    else
                        target = new Position
                        {
                            X = player.X,
                            Y = player.Y,
                        };

                    host.Owner.BroadcastMessage(new SHOWEFFECT
                    {
                        EffectType = EffectType.Throw,
                        Color = new ARGB(0x9B30FF),
                        TargetId = host.Id,
                        PosA = target
                    }, null);

                    host.Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                    {
                        world.BroadcastMessage(new AOE
                        {
                            Position = target,
                            Radius = radius,
                            Damage = (ushort)damage,
                            EffectDuration = 0,
                            Effects = 0,
                            OriginType = (short)host.ObjectType,
                            Color = new ARGB(0x9B30FF)
                        }, null);

                        world.Aoe(target, radius, true, p =>
                        {
                            if (effect != ConditionEffectIndex.Hidden && effectDuration != -1)
                                (p as Player)?.ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = effect,
                                    DurationMS = effectDuration
                                });
                            (p as IPlayer)?.Damage(damage, host, false, true);
                        });
                    }));
                }

                cool = coolDown.Next(Random);
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}