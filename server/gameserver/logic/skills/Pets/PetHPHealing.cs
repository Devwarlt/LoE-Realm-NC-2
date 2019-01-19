using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using Mono.Game;
using System;

namespace LoESoft.GameServer.logic.skills.Pets
{
    internal class PetHPHealing : Behavior
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
                        int maxhp = player.Stats[0] + player.Boost[0];
                        int stars = player.Stars;
                        int hpVariance = GetHP(pet.ObjectDesc.HPTier, stars);

                        cool = 8000 - (player.Stars * 100);

                        int newhp = Math.Min(maxhp, player.HP + hpVariance);

                        if (newhp != player.HP)
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

                            int n = newhp - player.HP;
                            player.HP = newhp;
                            player.UpdateCount++;
                            player.Owner.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Heal,
                                TargetId = player.Id,
                                Color = new ARGB(0xffffffff)
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
                                Color = new ARGB(0xff00ff00)
                            }, null);
                        }
                    }
                }
            }
            else
            {
                //Log.Write(nameof(PetHPHealing), $"{cool} ms", ConsoleColor.DarkRed);
                cool -= time.ElapsedMsDelta;
            }

            state = cool;
        }

        private static int GetHP(int tier, int stars)
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
                        min = 2;
                        max = 9;
                    }
                    break;

                case 1:
                    {
                        min = 9;
                        max = 13;
                    }
                    break;

                case 2:
                    {
                        min = 13;
                        max = 18;
                    }
                    break;

                case 3:
                    {
                        min = 18;
                        max = 24;
                    }
                    break;

                case 4:
                    {
                        min = 24;
                        max = 32;
                    }
                    break;

                case 5:
                    {
                        min = 32;
                        max = 44;
                    }
                    break;

                case 6:
                    {
                        min = 44;
                        max = 54;
                    }
                    break;

                case 7:
                    {
                        min = 54;
                        max = 74;
                    }
                    break;
            }
            return Tuple.Create(min, max, bonus);
        }

        private int CalculateCooldown(Player player, ref int cooldown) => 8000 - (player.Stars * 100);
    }
}