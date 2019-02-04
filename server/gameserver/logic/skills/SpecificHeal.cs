#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class SpecificHeal : Behavior
    {
        private readonly string group;
        private readonly int amount;
        private readonly double range;
        private Cooldown coolDown;

        public SpecificHeal(
            double range,
            int amount,
            string group,
            Cooldown coolDown = new Cooldown()
            )
        {
            this.range = (float)range;
            this.amount = amount;
            this.group = group;
            this.coolDown = coolDown.Normalize();
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
                if (group == "Self")
                {
                    Enemy entity = host as Enemy;
                    int newHp = amount;
                    if (newHp != entity.HP)
                    {
                        int n = newHp;
                        entity.HP = entity.HP + newHp;
                        entity.UpdateCount++;
                        entity.Owner.BroadcastMessage(new SHOWEFFECT
                        {
                            EffectType = EffectType.Heal,
                            TargetId = entity.Id,
                            Color = new ARGB(0xffffffff)
                        }, null);
                        entity.Owner.BroadcastMessage(new NOTIFICATION
                        {
                            ObjectId = entity.Id,
                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + n + "\"}}",
                            Color = new ARGB(0xff00ff00)
                        }, null);
                    }
                }
                else
                {
                    foreach (Enemy entity in host.GetNearestEntitiesByGroup(range, group).OfType<Enemy>())
                    {
                        int newHp = amount;
                        if (newHp != entity.HP)
                        {
                            int n = newHp;
                            entity.HP = entity.HP + newHp;
                            entity.UpdateCount++;
                            entity.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Heal,
                                TargetId = entity.Id,
                                Color = new ARGB(0xffffffff)
                            }, null);
                            entity.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Line,
                                TargetId = host.Id,
                                PosA = new Position { X = entity.X, Y = entity.Y },
                                Color = new ARGB(0xffffffff)
                            }, null);
                            entity.Owner.BroadcastMessage(new NOTIFICATION
                            {
                                ObjectId = entity.Id,
                                Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + n + "\"}}",
                                Color = new ARGB(0xff00ff00)
                            }, null);
                        }
                    }
                }
                cool = coolDown.Next(Random);
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}