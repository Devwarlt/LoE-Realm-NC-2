#region

using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.world;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class PentaractStar : Behavior
    {
        private Cooldown cool;

        public PentaractStar(
            Cooldown coolDown
            )
        {
            cool = coolDown;
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
                var entities = host.GetNearestEntities(28, 0x0d5e)
                    .Concat(host.GetNearestEntities(28, 0x0d60))
                    .ToArray();
                if (entities.Length != 5)
                    return;

                var packets = new List<Message>();
                if (!entities.Any(_ => _.ObjectType == 0x0d5e))
                {
                    var players = new HashSet<Entity>();
                    foreach (var i in entities.SelectMany(_ => (_ as Enemy).DamageCounter.GetPlayerData()))
                        if (i.Item1.Quest == host)
                            players.Add(i.Item1);
                    foreach (var i in players)
                        packets.Add(new NOTIFICATION
                        {
                            ObjectId = i.Id,
                            Color = new ARGB(0xFF00FF00),
                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Quest Complete!\"}}"
                        });

                    if (host.Owner is GameWorld)
                        (host.Owner as GameWorld).EnemyKilled(host as Enemy,
                            (entities.Last() as Enemy).DamageCounter.Parent.LastHitter);
                    new Decay(0).Tick(host, time);
                    foreach (var i in entities)
                        new Suicide().Tick(i, time);
                }
                else
                {
                    var hasCorpse = entities.Any(_ => _.ObjectType == 0x0d60);
                    for (var i = 0; i < entities.Length; i++)
                        for (var j = i + 1; j < entities.Length; j++)
                        {
                            packets.Add(new SHOWEFFECT
                            {
                                TargetId = entities[i].Id,
                                EffectType = EffectType.Stream,
                                Color = new ARGB(hasCorpse ? 0xFFFFFF : 0xffff0000),
                                PosA = new Position
                                {
                                    X = entities[j].X,
                                    Y = entities[j].Y
                                },
                                PosB = new Position
                                {
                                    X = entities[i].X,
                                    Y = entities[i].Y
                                }
                            });
                        }
                }
                host.Owner.BroadcastMessage(packets, null);
                cool = this.cool.Next(Random);
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
        }
    }
}