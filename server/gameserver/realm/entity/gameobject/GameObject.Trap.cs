﻿#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    internal class Trap : GameObject
    {
        private const int LIFETIME = 10;

        private readonly int dmg;
        private readonly int duration;
        private readonly ConditionEffectIndex effect;
        private readonly Player player;
        private readonly float radius;

        private int p;
        private int t;

        public Trap(Player player, float radius, int dmg, ConditionEffectIndex eff, float effDuration)
            : base(0x0711, LIFETIME * 1000, true, true, false)
        {
            this.player = player;
            this.radius = radius;
            this.dmg = dmg;
            effect = eff;
            duration = (int)(effDuration * 1000);
        }

        public override void Tick(RealmTime time)
        {
            if (t / 500 == p)
            {
                Owner.BroadcastMessage(new SHOWEFFECT
                {
                    EffectType = EffectType.Ring,
                    Color = new ARGB(0xff9000ff),
                    TargetId = Id,
                    PosA = new Position { X = radius / 2 }
                }, null);

                p++;

                if (p == LIFETIME * 2)
                {
                    Explode(time);
                    return;
                }
            }
            t += time.ElapsedMsDelta;

            bool monsterNearby = false;
            this.Aoe(radius / 2, false, enemy =>
            {
                if (!enemy.IsPet)
                    monsterNearby = true;
            });

            if (monsterNearby)
                Explode(time);

            base.Tick(time);
        }

        private void Explode(RealmTime time)
        {
            Owner.BroadcastMessage(new SHOWEFFECT
            {
                EffectType = EffectType.Nova,
                Color = new ARGB(0xff9000ff),
                TargetId = Id,
                PosA = new Position { X = radius }
            }, null);

            this.Aoe(radius, false, enemy =>
            {
                if (enemy.IsPet)
                    return;

                (enemy as Enemy).Damage(player, time, dmg, false, new ConditionEffect
                {
                    Effect = effect,
                    DurationMS = duration
                });
            });
            Owner.LeaveWorld(this);
        }
    }
}