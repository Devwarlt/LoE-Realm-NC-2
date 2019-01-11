using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ MountainsSpecial = () => Behav()
            .Init("Arena Horseman Anchor",
                new State(
                    new AddCond(ConditionEffectIndex.Invincible)
                    )
            )

            .Init("Arena Headless Horseman",
                new State(
                    new Spawn("Arena Horseman Anchor", 1, 1),
                    new State("EverythingIsCool",
                        new HpLessTransition(0.1, "End"),
                        new State("Circle",
                            new Shoot(15, 3, shootAngle: 25, index: 0, coolDown: 1000),
                            new Shoot(15, index: 1, coolDown: 1000),
                            new Circle(10, 5, 10, "Arena Horseman Anchor"),
                            new TimedTransition(8000, "Shoot")
                            ),
                        new State("Shoot",
                            new ReturnToSpawn(false, 15),
                            new AddCond(ConditionEffectIndex.Invincible),
                            new Flashing(0xF0E68C, 1, 6),
                            new Shoot(15, 8, index: 2, coolDown: 1500),
                            new Shoot(15, index: 1, coolDown: 2500),
                            new TimedTransition(6000, "Circle")
                            )
                        ),
                    new State("End",
                        new Prioritize(
                            new Chase(15, 20, 1),
                            new Wander(15)
                            ),
                        new Flashing(0xF0E68C, 1, 1000),
                        new Shoot(15, 3, shootAngle: 25, index: 0, coolDown: 1000),
                        new Shoot(15, index: 1, coolDown: 1000)
                        ),
                    new TransformOnDeath("Haunted Cemetery Portal", probability: .4)
                    )
            )

            .Init("Mysterious Crystal",
                new State(
                    new State("Waiting",
                        new PlayerWithinTransition(10, "Idle")
                        ),
                    new State("Idle",
                        new Taunt(0.1, "Break the crystal for great rewards..."),
                        new Taunt(0.1, "Help me..."),
                        new HpLessTransition(0.9999, "Instructions"),
                        new TimedTransition(10000, "Idle")
                        ),
                    new State("Instructions",
                        new Flashing(0xffffffff, 2, 100),
                        new Taunt(0.8, "Fire upon this crystal with all your might for 5 seconds"),
                        new Taunt(0.8, "If your attacks are weak, the crystal magically heals"),
                        new Taunt(0.8, "Gather a large group to smash it open"),
                        new HpLessTransition(0.998, "Evaluation")
                        ),
                    new State("Evaluation",
                        new State("Comment1",
                            new Taunt(true, "Sweet treasure awaits for powerful adventurers!"),
                            new Taunt(0.4, "Yes!  Smash my prison for great rewards!"),
                            new TimedTransition(5000, "Comment2")
                            ),
                        new State("Comment2",
                            new Taunt(0.3,
                                "If you are not very strong, this could kill you",
                                "If you are not yet powerful, stay away from the Crystal",
                                "New adventurers should stay away",
                                "That's the spirit. Lay your fire upon me.",
                                "So close..."
                                ),
                            new TimedTransition(5000, "Comment3")
                            ),
                        new State("Comment3",
                            new Taunt(0.4,
                                "I think you need more people...",
                                "Call all your friends to help you break the crystal!"
                                ),
                            new TimedTransition(10000, "Comment2")
                            ),
                        new HealGroup(1, "Crystals", coolDown: 5000),
                        new HpLessTransition(0.95, "StartBreak"),
                        new TimedTransition(60000, "Fail")
                        ),
                    new State("Fail",
                        new Taunt("Perhaps you need a bigger group. Ask others to join you!"),
                        new Flashing(0xff000000, 5, 1),
                        new Shoot(10, shoots: 16, shootAngle: 22.5, direction: 0, coolDown: 100000),
                        new HealGroup(1, "Crystals", coolDown: 1000),
                        new TimedTransition(5000, "Idle")
                        ),
                    new State("StartBreak",
                        new Taunt("You cracked the crystal! Soon we shall emerge!"),
                        new ChangeSize(-2, 80),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff000000, 2, 10),
                        new TimedTransition(4000, "BreakCrystal")
                        ),
                    new State("BreakCrystal",
                        new Taunt("This your reward! Imagine what evil even Oryx needs to keep locked up!"),
                        new Shoot(0, shoots: 16, shootAngle: 22.5, direction: 0, coolDown: 100000),
                        new Spawn("Crystal Prisoner", maxChildren: 1, initialSpawn: 1, coolDown: 100000),
                        new Decay(0)
                        )
                    )
            )

            .Init("Crystal Prisoner",
                new State(
                    new TransformOnDeath("Deadwater Docks", probability: 1),
                    new Spawn("Crystal Prisoner Steed", maxChildren: 3, initialSpawn: 0, coolDown: 200),
                    new State("pause",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new TimedTransition(2000, "start_the_fun")
                        ),
                    new State("start_the_fun",
                        new Taunt("I'm finally free! Yesss!!!"),
                        new TimedTransition(1500, "Daisy_attack")
                        ),
                    new State("Daisy_attack",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Prioritize(
                            new StayCloseToSpawn(0.3, range: 7),
                            new Wander(3)
                            ),
                        new State("Quadforce1",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 0, coolDown: 300),
                            new TimedTransition(200, "Quadforce2")
                            ),
                        new State("Quadforce2",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 15, coolDown: 300),
                            new TimedTransition(200, "Quadforce3")
                            ),
                        new State("Quadforce3",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 30, coolDown: 300),
                            new TimedTransition(200, "Quadforce4")
                            ),
                        new State("Quadforce4",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 45, coolDown: 300),
                            new TimedTransition(200, "Quadforce5")
                            ),
                        new State("Quadforce5",
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 60, coolDown: 300),
                            new TimedTransition(200, "Quadforce6")
                            ),
                        new State("Quadforce6",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 75, coolDown: 300),
                            new TimedTransition(200, "Quadforce7")
                            ),
                        new State("Quadforce7",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 90, coolDown: 300),
                            new TimedTransition(200, "Quadforce8")
                            ),
                        new State("Quadforce8",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(10, index: 3, coolDown: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, direction: 105, coolDown: 300),
                            new TimedTransition(200, "Quadforce1")
                            ),
                        new HpLessTransition(0.3, "Whoa_nelly"),
                        new TimedTransition(18000, "Warning")
                        ),
                    new State("Warning",
                        new Prioritize(
                            new StayCloseToSpawn(5, range: 7),
                            new Wander(5)
                            ),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 15),
                        new Chase(4, sightRange: 9, range: 2),
                        new TimedTransition(3000, "Summon_the_clones")
                        ),
                    new State("Summon_the_clones",
                        new Prioritize(
                            new StayCloseToSpawn(8.5, range: 7),
                            new Wander(8.5)
                            ),
                        new Shoot(10, index: 0, coolDown: 1000),
                        new Spawn("Crystal Prisoner Clone", maxChildren: 4, initialSpawn: 0, coolDown: 200),
                        new TossObject("Crystal Prisoner Clone", range: 5, angle: 0, coolDown: 100000),
                        new TossObject("Crystal Prisoner Clone", range: 5, angle: 240, coolDown: 100000),
                        new TossObject("Crystal Prisoner Clone", range: 7, angle: 60, coolDown: 100000),
                        new TossObject("Crystal Prisoner Clone", range: 7, angle: 300, coolDown: 100000),
                        new State("invulnerable_clone",
                            new TimedTransition(3000, "vulnerable_clone")
                            ),
                        new State("vulnerable_clone",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(1200, "invulnerable_clone")
                            ),
                        new TimedTransition(16000, "Warning2")
                        ),
                    new State("Warning2",
                        new Prioritize(
                            new StayCloseToSpawn(8.5, range: 7),
                            new Wander(8.5)
                            ),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 25),
                        new TimedTransition(5000, "Whoa_nelly")
                        ),
                    new State("Whoa_nelly",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Prioritize(
                            new StayCloseToSpawn(6, range: 7),
                            new Wander(6)
                            ),
                        new Shoot(10, index: 3, shoots: 3, shootAngle: 120, coolDown: 900),
                        new Shoot(10, index: 2, shoots: 3, shootAngle: 15, direction: 40, coolDown: 1600, coolDownOffset: 0),
                        new Shoot(10, index: 2, shoots: 3, shootAngle: 15, direction: 220, coolDown: 1600, coolDownOffset: 0),
                        new Shoot(10, index: 2, shoots: 3, shootAngle: 15, direction: 130, coolDown: 1600, coolDownOffset: 800),
                        new Shoot(10, index: 2, shoots: 3, shootAngle: 15, direction: 310, coolDown: 1600, coolDownOffset: 800),
                        new State("invulnerable_whoa",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2600, "vulnerable_whoa")
                            ),
                        new State("vulnerable_whoa",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(1200, "invulnerable_whoa")
                            ),
                        new TimedTransition(10000, "Absolutely_Massive")
                        ),
                    new State("Absolutely_Massive",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(13, 260),
                        new Prioritize(
                            new StayCloseToSpawn(0.2, range: 7),
                            new Wander(0.2)
                            ),
                        new Shoot(10, index: 1, shoots: 9, shootAngle: 40, direction: 40, coolDown: 2000, coolDownOffset: 400),
                        new Shoot(10, index: 1, shoots: 9, shootAngle: 40, direction: 60, coolDown: 2000, coolDownOffset: 800),
                        new Shoot(10, index: 1, shoots: 9, shootAngle: 40, direction: 50, coolDown: 2000, coolDownOffset: 1200),
                        new Shoot(10, index: 1, shoots: 9, shootAngle: 40, direction: 70, coolDown: 2000, coolDownOffset: 1600),
                        new State("invulnerable_mass",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2600, "vulnerable_mass")
                            ),
                        new State("vulnerable_mass",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(1000, "invulnerable_mass")
                            ),
                        new TimedTransition(14000, "Start_over_again")
                        ),
                    new State("Start_over_again",
                        new ChangeSize(-20, 100),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 15),
                        new TimedTransition(3000, "Daisy_attack")
                        )
                    ),
                new Threshold(0.015,
                    new TierLoot(2, ItemType.Potion, 0.07)
                    ),
                new Threshold(0.03,
                    new ItemLoot("Crystal Wand", 0.05),
                    new ItemLoot("Crystal Sword", 0.06)
                    )
            )

            .Init("Crystal Prisoner Clone",
                new State(
                    new Prioritize(
                        new StayCloseToSpawn(8.5, range: 5),
                        new Wander(8.5)
                        ),
                    new Shoot(10, coolDown: 1400),
                    new State("taunt",
                        new Taunt(0.09, "I am everywhere and nowhere!"),
                        new TimedTransition(1000, "no_taunt")
                        ),
                    new State("no_taunt",
                        new TimedTransition(1000, "taunt")
                        ),
                    new Decay(17000)
                    )
            )

            .Init("Crystal Prisoner Steed",
                new State(
                    new State("change_position_fast",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Prioritize(
                            new StayCloseToSpawn(36, range: 12),
                            new Wander(36)
                            ),
                        new TimedTransition(800, "attack")
                        ),
                    new State("attack",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Shoot(10, aim: 0.3, coolDown: 500),
                        new State("keep_distance",
                            new Prioritize(
                                new StayCloseToSpawn(10, range: 12),
                                new Circle(10, 9, target: "Crystal Prisoner", radiusVariance: 0)
                                ),
                            new TimedTransition(2000, "go_anywhere")
                            ),
                        new State("go_anywhere",
                            new Prioritize(
                                new StayCloseToSpawn(10, range: 12),
                                new Wander(10)
                                ),
                            new TimedTransition(2000, "keep_distance")
                            )
                        )
                    )
            )
        ;
    }
}