using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using Mono.Game;
using System;

namespace LoESoft.GameServer.logic.skills.Pets
{
    internal class PetMPHealing : Behavior
    {
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
            base.OnStateEntry(host, time, ref state);
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            Player player = host.GetPlayerOwner();
            Entity pet = player.Pet;
            bool hatchling = player.HatchlingPet;

            if (hatchling)
                return;

            if (player.Owner == null || pet == null || host == null)
            {
                pet.Owner.LeaveWorld(host);
                return;
            }

            int cool = (int)state;
            if (cool <= 0)
            {
                if (!player.HasConditionEffect(ConditionEffectIndex.Sick) && !player.HasConditionEffect(ConditionEffectIndex.PetDisable))
                {
                    float distance = Vector2.Distance(new Vector2(pet.X, pet.Y), new Vector2(player.X, player.Y));

                    if (distance < 20)
                    {
                        int maxmp = player.Stats[1] + player.Boost[1];
                        int stars = player.Stars;
                        int mpVariance = GetMP(pet.ObjectDesc.MPTier, stars);

                        cool = 8000 - (player.Stars * 100);

                        int newmp = Math.Min(maxmp, player.MP + mpVariance);

                        if (newmp != player.MP)
                        {
                            if (player.HasConditionEffect(ConditionEffectIndex.Sick) || player.HasConditionEffect(ConditionEffectIndex.PetDisable))
                            {
                                player.Owner.BroadcastMessage(new SHOWEFFECT
                                {
                                    EffectType = EffectType.Line,
                                    TargetId = host.Id,
                                    PosA = new Position { X = player.X, Y = player.Y },
                                    Color = new ARGB(0xffffffff)
                                }, null);
                                player.Owner.BroadcastMessage(new NOTIFICATION
                                {
                                    ObjectId = player.Id,
                                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"No Effect\"}}",
                                    Color = new ARGB(0xFF0000)
                                }, null);
                                state = cool;
                                return;
                            }

                            int n = newmp - player.MP;
                            player.MP = newmp;
                            player.UpdateCount++;
                            player.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Heal,
                                TargetId = player.Id,
                                Color = new ARGB(0x6084e0)
                            }, null);
                            player.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Line,
                                TargetId = host.Id,
                                PosA = new Position { X = player.X, Y = player.Y },
                                Color = new ARGB(0xffffffff)
                            }, null);
                            player.Owner.BroadcastMessage(new NOTIFICATION
                            {
                                ObjectId = player.Id,
                                Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + n + "\"}}",
                                Color = new ARGB(0x6084e0)
                            }, null);
                        }
                    }
                }
            }
            else
            {
                //Log.Write(nameof(PetMPHealing), $"{cool} ms", ConsoleColor.DarkCyan);
                cool -= time.ElapsedMsDelta;
            }

            state = cool;
        }

        private static int GetMP(int tier, int stars)
        {
            Random rnd = new Random();

            Tuple<int, int, double> formula = MinMaxBonus(tier, stars);

            return (int)(rnd.Next(formula.Item1, formula.Item2) * formula.Item3);
        }

        public static Tuple<int, int, double> MinMaxBonus(int tier, int stars)
        {
            int min = 0;
            int max = 0;

            double bonus = (1 + (stars == 70 ? 0.35 : stars / 200)); // at least 35% bonus

            switch (tier)
            {
                case 0:
                    {
                        min = 1;
                        max = 2;
                    }
                    break;

                case 1:
                    {
                        min = 2;
                        max = 5;
                    }
                    break;

                case 2:
                    {
                        min = 5;
                        max = 9;
                    }
                    break;

                case 3:
                    {
                        min = 9;
                        max = 11;
                    }
                    break;

                case 4:
                    {
                        min = 11;
                        max = 17;
                    }
                    break;

                case 5:
                    {
                        min = 17;
                        max = 23;
                    }
                    break;

                case 6:
                    {
                        min = 23;
                        max = 29;
                    }
                    break;

                case 7:
                    {
                        min = 29;
                        max = 37;
                    }
                    break;
            }
            return Tuple.Create(min, max, bonus);
        }
    }
}