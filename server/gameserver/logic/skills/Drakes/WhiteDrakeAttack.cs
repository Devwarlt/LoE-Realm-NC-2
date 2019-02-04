#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using Mono.Game;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors.Drakes
{
    internal class WhiteDrakeAttack : Behavior
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
                var entity = host.GetPlayerOwner();

                if (!entity.HasConditionEffect(ConditionEffectIndex.Sick))
                {
                    try
                    {
                        var distance = Vector2.Distance(new Vector2(host.X, host.Y),
                            new Vector2(entity.X, entity.Y));

                        if (distance < 10)
                        {
                            var hp = entity.HP;
                            var maxHp = entity.Stats[0] + entity.Boost[0];
                            hp = Math.Min(hp + 25, maxHp);

                            if (hp != entity.HP)
                            {
                                var n = hp - entity.HP;
                                entity.HP = hp;
                                entity.UpdateCount++;
                                entity.Owner.BroadcastMessage(new SHOWEFFECT
                                {
                                    EffectType = EffectType.Heal,
                                    TargetId = entity.Id,
                                    Color = new ARGB(0xFFFFFF)
                                }, null);
                                entity.Owner.BroadcastMessage(new SHOWEFFECT
                                {
                                    EffectType = EffectType.Line,
                                    TargetId = host.Id,
                                    PosA = new Position { X = entity.X, Y = entity.Y },
                                    Color = new ARGB(0xFFFFFF)
                                }, null);
                                entity.Owner.BroadcastMessage(new NOTIFICATION
                                {
                                    ObjectId = entity.Id,
                                    Text = "+" + n,
                                    Color = new ARGB(0xff00ff00)
                                }, null);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                cool = 500;
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}