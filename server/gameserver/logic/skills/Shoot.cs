#region

using LoESoft.Core.models;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Shoot : CycleBehavior
    {
        protected readonly double angleOffset;
        protected readonly int coolDownOffset;
        protected readonly int shoots;
        protected readonly double aim;
        protected readonly int index;
        protected readonly double range;
        protected readonly double shootAngle;
        protected float? direction;
        protected Cooldown coolDown;
        protected double? defaultAngle;
        protected bool rotateEffect;
        protected readonly int rotateRadius;
        protected readonly uint rotateColor;
        protected double? rotateAngle;

        public Shoot(
            double range = 8,
            int shoots = 1,
            double? shootAngle = null,
            int index = 0,
            double? direction = null,
            double angleOffset = 0,
            double? defaultAngle = null,
            double aim = 0,
            int coolDownOffset = 0,
            Cooldown coolDown = new Cooldown(),
            bool rotateEffect = false,
            int rotateRadius = 2,
            uint rotateColor = 0xFFFFFF,
            double? rotateAngle = null
            )
        {
            this.range = range;
            this.shoots = shoots;
            this.index = index;
            this.aim = aim;
            this.coolDownOffset = coolDownOffset;
            this.coolDown = coolDown.Normalize();
            this.rotateEffect = rotateEffect;
            this.rotateRadius = rotateRadius;
            this.rotateColor = rotateColor;
            this.shootAngle = shoots == 1 ? 0 : (shootAngle ?? 360.0 / shoots) * Math.PI / 180;
            this.direction = (float?)(direction * Math.PI / 180);
            this.angleOffset = (float)(angleOffset * Math.PI / 180);
            this.defaultAngle = (float?)(defaultAngle * Math.PI / 180);
            this.rotateAngle = (float?)(rotateAngle * Math.PI / 180);
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = coolDownOffset;
        }

        private static Position EnemyShootHistory(Entity host)
        {
            Position? history = host.TryGetHistory(1);

            if (history == null)
                return new Position { X = host.X, Y = host.Y };

            return new Position { X = history.Value.X, Y = history.Value.Y };
        }

        private static double Predict(Entity host, Entity target, ProjectileDesc desc)
        {
            Position? history = target.TryGetHistory(1);
            if (history == null)
                return 0;

            double originalAngle = Math.Atan2(history.Value.Y - host.Y, history.Value.X - host.X);
            double newAngle = Math.Atan2(target.Y - host.Y, target.X - host.X);

            float bulletSpeed = desc.Speed / 100;
            double angularVelo = (newAngle - originalAngle) / (100 / 1000f);
            return angularVelo * bulletSpeed;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            try
            {
                int cool = (int?)state ?? -1;
                Status = CycleStatus.NotStarted;

                if (cool <= 0)
                {
                    if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                        return;
                    int count = shoots;
                    if (host.HasConditionEffect(ConditionEffectIndex.Dazed))
                        count = Math.Max(1, count / 2);

                    Entity player = host.GetNearestEntity(range, null);
                    if (player != null || defaultAngle != null || direction != null)
                    {
                        ProjectileDesc desc = host.ObjectDesc.Projectiles[index];

                        double a = direction ??
                                   (player == null ? defaultAngle.Value : Math.Atan2(player.Y - host.Y, player.X - host.X));
                        a += angleOffset;
                        /*if (aim != 0 && player != null)
                            a += Predict(host, player, desc) * aim;*/

                        int dmg;
                        if (host is Character)
                            dmg = (host as Character).Random.Next(desc.MinDamage, desc.MaxDamage);
                        else
                            dmg = Random.Next(desc.MinDamage, desc.MaxDamage);

                        double startAngle = a - shootAngle * (count - 1) / 2;
                        byte prjId = 0;
                        Position prjPos = EnemyShootHistory(host);
                        for (int i = 0; i < count; i++)
                        {
                            Projectile prj = host.CreateProjectile(
                                desc, host.ObjectType, dmg, time.TotalElapsedMs,
                                prjPos, (float)(startAngle + shootAngle * i));
                            host.Owner.EnterWorld(prj);
                            if (i == 0)
                                prjId = prj.ProjectileId;
                        }
                        if (rotateEffect)
                        {
                            Position target;
                            if (rotateAngle != null)
                                target = new Position
                                {
                                    X = host.X + (float)(Math.Cos(rotateAngle.Value)),
                                    Y = host.Y + (float)(Math.Sin(rotateAngle.Value)),
                                };
                            else
                                target = new Position
                                {
                                    X = player.X,
                                    Y = player.Y,
                                };

                            host.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Coneblast,
                                Color = new ARGB(rotateColor),
                                TargetId = host.Id,
                                PosA = target,
                                PosB = new Position { X = rotateRadius }, //radius
                            }, null);
                        }

                        host.Owner.BroadcastMessage(new ENEMYSHOOT
                        {
                            BulletId = prjId,
                            OwnerId = host.Id,
                            Position = prjPos,
                            Angle = (float)startAngle,
                            Damage = (short)(dmg * (host.HasConditionEffect(ConditionEffectIndex.Weak) ? 0.5d : 1d)),
                            BulletType = (byte)desc.BulletType,
                            AngleInc = (float)shootAngle,
                            NumShots = (byte)count,
                        }, null);
                    }
                    cool = coolDown.Next(Random);
                    Status = CycleStatus.Completed;
                }
                else
                {
                    cool -= time.ElapsedMsDelta;
                    Status = CycleStatus.InProgress;
                }

                state = cool;
            }
            catch (IndexOutOfRangeException)
            {
                Log.Warn($"Projectile index '{index}' doesn't exist in Shoot behavior of entity {host.Name} (state: {host.CurrentState.Name}).");
                return;
            }
        }
    }
}