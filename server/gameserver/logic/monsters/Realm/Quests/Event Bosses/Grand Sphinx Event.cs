using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesGrandSphinxEvent = () => Behav()
            // credit: mike for making this
            .Init("Grand Sphinx",
            new State(
                new DropPortalOnDeath("Tomb of the Ancients Portal", 20, PortalDespawnTimeSec: 45),
                new StayCloseToSpawn(5.5, 12),
                new Wander(5.5),
                new Spawn("Horrid Reaper", maxChildren: 6),
                new State("start_the_fun",
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new PlayerWithinTransition(11, "Warning0")
                    ),
                new State("Warning0",
                    new Flashing(0x00FF00, 0.2, 10),
                    new TimedTransition(2000, "flame_on")
                    ),
                new State("flame_on",
                    new RemCond(ConditionEffectIndex.Invulnerable), // ok
                    new Shoot(20, 3, index: 0, coolDown: 600, aim: 0.4, shootAngle: 30),
                    new Shoot(20, 3, index: 0, coolDown: 600, aim: 0.4, shootAngle: 120, angleOffset: 0.3),
                    new TimedTransition(7000, "Warning")
                    ),
                new State("Warning",
                    new Taunt(1.00, "You hide like cowards... but you can't hide from this!"),
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new Flashing(0x00FF00, 0.2, 15),
                    new TimedTransition(3000, "dead_zed")
                    ),
                new State("dead_zed",
                    new State("dead_zed1",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 40, index: 1),
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 220, index: 1),
                        new TimedTransition(1000, "dead_zed2")
                        ),
                    new State("dead_zed2",
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 80, index: 1),
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 260, index: 1),
                        new TimedTransition(1000, "dead_zed3")
                        ),
                    new State("dead_zed3",
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 120, index: 1),
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 320, index: 1),
                        new TimedTransition(1000, "dead_zed4")
                        ),
                    new State("dead_zed4",
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 160, index: 1),
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 360, index: 1),
                        new TimedTransition(1000, "dead_zed5")
                        ),
                    new State("dead_zed5",
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 200, index: 1),
                        new Shoot(10, 11, 7, coolDown: 1600, defaultAngle: 40, index: 1),
                        new TimedTransition(1000, "dead_zed1")
                        ),
                    new TimedTransition(8000, "Warning2")
                    ),
                new State("Warning2",
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new Flashing(0x00FF00, 0.2, 15),
                    new TimedTransition(3000, "burn_baby_burn")
                    ),
                new State("burn_baby_burn",
                    new RemCond(ConditionEffectIndex.Invulnerable), // ok
                    new Shoot(10, 2, 10, coolDown: 2000, coolDownOffset: 0, index: 2),
                    new Shoot(10, 2, 10, coolDown: 2000, coolDownOffset: 500, index: 2),
                    new Shoot(10, 9, 40, coolDown: 2000, defaultAngle: 20, angleOffset: 1, index: 2),
                    new Shoot(10, 2, 10, coolDown: 2000, coolDownOffset: 1500, index: 2),
                    new Shoot(10, 2, 10, coolDown: 2000, coolDownOffset: 2, index: 2),
                    new Shoot(10, 8, 10, coolDown: 2000, coolDownOffset: 2500, index: 2),
                    new TimedTransition(7600, "Warning0"),
                    new HpLessTransition(0.4, "no_more_reaper")
                    ),
                new State("no_more_reaper",
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new EntityOrder(50, "Horrid Reaper", "quick_die"),
                    new TimedTransition(3000, "flame_on")
                )
            ),
            new Drops(
                new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                new BlueBag(new[] { Potions.POTION_OF_VITALITY, Potions.POTION_OF_WISDOM }, new[] { true, true }),
                new WhiteBag("Helm of the Juggernaut")
                )
            )

        .Init("Horrid Reaper",
            new State(
                new AddCond(ConditionEffectIndex.Invulnerable), // ok
                new State("break_time",
                    new Protect(10, "Grand Sphinx", 18, 10, 10),
                    new Wander(30),
                    new TimedTransition(2000, "slow_follow")
                    ),
                new State("slow_follow",
                    new Protect(10, "Grand Sphinx", 18, 10, 10),
                    new Chase(5, 10, 2.5),
                    new Shoot(10, 1, index: 1, coolDown: 400, aim: 0.4, coolDownOffset: 600),
                    new TimedTransition(4500, "fast_follow")
                    ),
                new State("fast_follow",
                    new Protect(10, "Grand Sphinx", 18, 10, 10),
                    new Chase(8, 10, 2.5),
                    new Shoot(10, 1, index: 0, coolDown: 200, aim: 0.4, coolDownOffset: 400),
                    new TimedTransition(3000, "break_time2")
                    ),
                new State("break_time2",
                    new Protect(10, "Grand Sphinx", 18, 10, 10),
                    new Wander(30),
                    new TimedTransition(3500, "splash_damage")
                    ),
                new State("splash_damage",
                    new Shoot(10, 6, index: 0, coolDown: 600, shootAngle: 60),
                    new TimedTransition(6500, "slow_follow"),
                    new EntityNotExistsTransition("Grand Sphinx", 50, "die")
                    ),
                new State("die",
                    new Decay(0)
                    ),
                new State("quick_die",
                    new Taunt("OOaoaoAaAoaAAOOAoaaoooaa!!!"),
                    new Decay(1000)
                    )
                )
            )

        ;
    }
}