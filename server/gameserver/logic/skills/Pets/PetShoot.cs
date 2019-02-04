#region

using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.logic.skills.Pets
{
    internal class PetShoot : CycleBehavior
    {
        protected readonly double angleOffset;
        protected readonly int count;
        protected readonly int projectileIndex;
        protected readonly double shootAngle;
        protected readonly double? fixedAngle;
        protected readonly double? defaultAngle;
        protected readonly bool special;
        protected readonly int coolDownOffset;
        protected Cooldown coolDown;

        public PetShoot(
            int count = 1,
            double? shootAngle = null,
            int projectileIndex = 0,
            double? fixedAngle = null,
            double angleOffset = 0,
            double? defaultAngle = null,
            bool special = false,
            int coolDownOffset = 0,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.count = count;
            this.shootAngle = count == 1 ? 0 : (shootAngle ?? 360.0 / count) * Math.PI / 180;
            this.fixedAngle = fixedAngle * Math.PI / 180;
            this.angleOffset = angleOffset * Math.PI / 180;
            this.defaultAngle = defaultAngle * Math.PI / 180;
            this.projectileIndex = projectileIndex;
            this.special = special;
            this.coolDownOffset = coolDownOffset;
            this.coolDown = coolDown.Normalize();
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state) => state = special ? coolDownOffset : 0;

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            var player = host.GetPlayerOwner();
            var pet = player.Pet;
            var hatchling = player.HatchlingPet;

            if (hatchling)
                return;

            if (player.Owner == null || pet == null || host == null)
            {
                pet.Owner.LeaveWorld(host);
                return;
            }

            if (host.Owner.SafePlace || !player.EnablePetAttack)
                return;

            var cool = (int?)state ?? -1;

            Status = CycleStatus.NotStarted;

            if (cool <= 0)
            {
                if (player.HasConditionEffect(ConditionEffectIndex.Sick) || player.HasConditionEffect(ConditionEffectIndex.PetDisable))
                    return;

                var stars = player.Stars;

                if (pet.GetNearestEntity(12, false, enemy => enemy is Enemy && pet.Dist(enemy) <= 12) is Enemy target && target.ObjectDesc.Enemy)
                {
                    var desc = pet.ObjectDesc.Projectiles[projectileIndex];
                    var a = fixedAngle ?? (target == null ? defaultAngle.Value : Math.Atan2(target.Y - pet.Y, target.X - pet.X));
                    a += angleOffset;

                    var variance = 0;

                    if (stars == 70)
                        variance = 7000;
                    else
                        variance = player.Stars * 100;

                    cool = special ? cool = coolDown.Next(Random) : (7750 - variance); // max 750ms cooldown if not special

                    var rnd = new Random();
                    var min = 0;
                    var max = 100;
                    var success = stars + 30;
                    var rng = rnd.Next(min, max);

                    if (rng > success)
                    {
                        var _outgoing = new List<Message>();
                        var _effect = new SHOWEFFECT();
                        var _position = new Position();
                        var _notification = new NOTIFICATION();

                        _position.X = .25f;
                        _position.Y = 2 / _position.X;

                        _effect.Color = new ARGB(0xFF0000);
                        _effect.EffectType = EffectType.Flash;
                        _effect.PosA = _position;
                        _effect.TargetId = pet.Id;

                        _outgoing.Add(_effect);

                        _notification.Color = new ARGB(0xFFFFFF);
                        _notification.ObjectId = pet.Id;
                        _notification.Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Miss!\"}}";

                        _outgoing.Add(_notification);

                        pet.Owner.BroadcastMessage(_outgoing, null);

                        state = cool;
                        return;
                    }

                    var dmg = rnd.Next(desc.MinDamage, desc.MaxDamage);
                    var startAngle = a - shootAngle * (count - 1) / 2;
                    var prjPos = new Position() { X = pet.X, Y = pet.Y };
                    var prj = player.CreateProjectile(desc, pet.ObjectType, dmg, time.TotalElapsedMs, prjPos, (float)startAngle);

                    player.Owner.Projectiles.TryAdd(prj, null);

                    player.Owner.EnterWorld(prj);

                    var _outgoingMessages = new List<Message>();
                    var _shoot = new ENEMYSHOOT
                    {
                        BulletId = 0,
                        OwnerId = pet.Id,
                        Position = prjPos,
                        Angle = prj.Angle,
                        Damage = 0,
                        BulletType = 0,
                        AngleInc = (float)shootAngle,
                        NumShots = 0
                    };
                    var _shoot2 = new SERVERPLAYERSHOOT
                    {
                        BulletId = prj.ProjectileId,
                        OwnerId = player.Id,
                        ContainerType = pet.ObjectType,
                        StartingPos = prj.BeginPos,
                        Angle = prj.Angle,
                        Damage = (short)prj.Damage
                    };

                    _outgoingMessages.Add(_shoot); // visual, display no animation only activate pet set alt index
                    _outgoingMessages.Add(_shoot2);

                    player.Owner.BroadcastMessage(_outgoingMessages, null);

                    player.FameCounter.Shoot(prj);

                    Status = CycleStatus.Completed;
                }
                else
                    return;
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