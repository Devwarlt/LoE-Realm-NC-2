#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.terrain;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class SnakePitTowerShoot : Shoot
    {
        public SnakePitTowerShoot()
            : base(range: 25, shoots: 1, index: 0, coolDown: 2000)
        {
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            base.OnStateEntry(host, time, ref state);
            WmapTile tile = host.Owner.Map[(int)host.X + 1, (int)host.Y].Clone();
            if (tile.ObjType != 0)
                direction = 140;
            else
                direction = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (state == null)
                return;
            int cool = (int)state;
            Status = CycleStatus.NotStarted;

            if (cool <= 0)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                    return;

                Entity player = host.GetNearestEntity(range, null);

                WmapTile tile = host.Owner.Map[(int)host.X + 1, (int)host.Y].Clone();

                if (tile.ObjType != 0)
                    direction = (float?)(180 * Math.PI / 180);
                else
                    direction = (float?)(0 * Math.PI / 180);

                if (player != null || defaultAngle != null || direction != null)
                {
                    ProjectileDesc desc = host.ObjectDesc.Projectiles[index];

                    double a = direction ??
                               (player == null ? defaultAngle.Value : Math.Atan2(player.Y - host.Y, player.X - host.X));
                    a += angleOffset;

                    int dmg;
                    if (host is Character)
                        dmg = (host as Character).Random.Next(desc.MinDamage, desc.MaxDamage);
                    else
                        dmg = Random.Next(desc.MinDamage, desc.MaxDamage);

                    double startAngle = a - shootAngle * (shoots - 1) / 2;
                    byte prjId = 0;
                    Position prjPos = new Position { X = host.X, Y = host.Y };
                    for (int i = 0; i < shoots; i++)
                    {
                        Projectile prj = host.CreateProjectile(
                            desc, host.ObjectType, dmg, time.TotalElapsedMs,
                            prjPos, (float)(startAngle + shootAngle * i));
                        host.Owner.EnterWorld(prj);
                        if (i == 0)
                            prjId = prj.ProjectileId;
                    }

                    host.Owner.BroadcastMessage(new ENEMYSHOOT
                    {
                        BulletId = prjId,
                        OwnerId = host.Id,
                        Position = prjPos,
                        Angle = (float)startAngle,
                        Damage = (short)dmg,
                        BulletType = (byte)(desc.BulletType),
                        AngleInc = (float)shootAngle,
                        NumShots = (byte)shoots,
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
    }
}