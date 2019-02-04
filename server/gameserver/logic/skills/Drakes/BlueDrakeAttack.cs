﻿#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;

#endregion

namespace LoESoft.GameServer.logic.behaviors.Drakes
{
    public class BlueDrakeAttack : Behavior
    {
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool = (int)state;
            if (cool <= 0)
            {
                var entities = host.GetNearestEntities(6);

                Enemy en = null;
                foreach (Entity e in entities)
                    if (e is Enemy)
                    {
                        en = e as Enemy;
                        break;
                    }

                if (en != null & en.ObjectDesc.Enemy)
                {
                    en.Owner.BroadcastMessage(new SHOWEFFECT
                    {
                        EffectType = EffectType.Nova,
                        Color = new ARGB(0x003298),
                        TargetId = en.Id,
                        PosA = new Position { X = 1, }
                    }, null);
                    en.Owner.BroadcastMessage(new SHOWEFFECT
                    {
                        EffectType = EffectType.Line,
                        TargetId = host.Id,
                        PosA = new Position { X = en.X, Y = en.Y },
                        Color = new ARGB(0x003298)
                    }, null);
                    en.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Paralyzed,
                        DurationMS = 5000
                    });
                }

                cool = 5000;
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}