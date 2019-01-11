using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    public class GodParams
    {
        public const int MAX_AMOUNT = 2;
        public const int RANGE = 48;
    }

    partial class BehaviorDb
    {
        private _ MountainsGods = () => Behav()
            .Init("White Demon",
                new State(
                    new TransformOnDeath("Abyss of Demons Portal", probability:.17),
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(10, shoots: 3, shootAngle: 20, aim: 1, coolDown: 500),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(8, ItemType.Weapon, 0.01),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(9, ItemType.Armor, 0.01),
                    new TierLoot(3, ItemType.Ring, 0.015),
                    new TierLoot(4, ItemType.Ring, 0.005)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Attack", 0.07)
                    )
            )

            .Init("Sprite God",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Wander()
                        ),
                    new Shoot(12, index: 0, shoots: 4, shootAngle: 10),
                    new Shoot(10, index: 1, aim: 1),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE),
                    new Reproduce("Sprite Child", 35, 5, 0, 5000)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(8, ItemType.Weapon, 0.01),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(9, ItemType.Armor, 0.01),
                    new TierLoot(4, ItemType.Ring, 0.02),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Attack", 0.07)
                    )
            )

            .Init("Sprite Child",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Protect(4, "Sprite God", protectRange: 1),
                        new Wander()
                        ),
                    new TransformOnDeath("Glowing Portal", probability: .11)
                    )
            )

            .Init("Medusa",
                new State(
                    new TransformOnDeath("Snake Pit Portal", probability: .17),
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, shoots: 5, shootAngle: 10, coolDown: 1000),
                    new Grenade(4, 150, range: 8, coolDown: 3000),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(8, ItemType.Weapon, 0.01),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(9, ItemType.Armor, 0.01),
                    new TierLoot(3, ItemType.Ring, 0.015),
                    new TierLoot(4, ItemType.Ring, 0.005),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Speed", 0.07)
                    )
            )

            .Init("Ent God",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, shoots: 5, shootAngle: 10, aim: 1, coolDown: 1250),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(8, ItemType.Weapon, 0.01),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(9, ItemType.Armor, 0.01),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Defense", 0.07)
                    )
            )

            .Init("Beholder",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, index: 0, shoots: 5, shootAngle: 72, aim: 0.5, coolDown: 750),
                    new Shoot(10, index: 1, aim: 1),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(8, ItemType.Weapon, 0.01),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(9, ItemType.Armor, 0.01),
                    new TierLoot(3, ItemType.Ring, 0.015),
                    new TierLoot(4, ItemType.Ring, 0.005)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Defense", 0.07)
                    )
            )

            .Init("Flying Brain",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, shoots: 5, shootAngle: 72, coolDown: 500),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE),
                    new DropPortalOnDeath("Mad Lab Portal", percent: .17)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(3, ItemType.Ring, 0.015),
                    new TierLoot(4, ItemType.Ring, 0.005),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Attack", 0.07)
                    )
            )

            .Init("Slime God",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, index: 0, shoots: 5, shootAngle: 10, aim: 1, coolDown: 1000),
                    new Shoot(10, index: 1, aim: 1, coolDown: 650),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(8, ItemType.Weapon, 0.01),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(9, ItemType.Armor, 0.01),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Defense", 0.07)
                    )
            )

            .Init("Ghost God",
                new State(
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, shoots: 7, shootAngle: 25, aim: 0.5, coolDown: 900),
                    new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE),
                    new TransformOnDeath("Undead Lair Portal", probability: 0.17)
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(3, ItemType.Ring, 0.015),
                    new TierLoot(4, ItemType.Ring, 0.005),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Speed", 0.07)
                    )
            )

            .Init("Rock Bot",
                new State(
                    new Spawn("Paper Bot", maxChildren: 1, initialSpawn: 1, coolDown: 10000),
                    new Spawn("Steel Bot", maxChildren: 1, initialSpawn: 1, coolDown: 10000),
                    new Swirl(speed: 6, radius: 3, targeted: false),
                    new State("Waiting",
                        new PlayerWithinTransition(15, "Attacking")
                        ),
                    new State("Attacking",
                        new Shoot(8, coolDown: 2000),
                        new HealGroup(8, "Papers", coolDown: 1000),
                        new Taunt(0.5, "We are impervious to non-mystic attacks!"),
                        new TimedTransition(10000, "Waiting")
                        )
                    ),
                new Threshold(.01,
                    new TierLoot(5, ItemType.Weapon, 0.16),
                    new TierLoot(6, ItemType.Weapon, 0.08),
                    new TierLoot(7, ItemType.Weapon, 0.04),
                    new TierLoot(5, ItemType.Armor, 0.16),
                    new TierLoot(6, ItemType.Armor, 0.08),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(3, ItemType.Ring, 0.05),
                    new TierLoot(3, ItemType.Ability, 0.1),
                    new ItemLoot("Purple Drake Egg", 0.01)
                    ),
                new Threshold(0.04,
                    new ItemLoot("Potion of Attack", 0.03)
                    )
            )

            .Init("Paper Bot",
                new State(
                    new DropPortalOnDeath("Puppet Theatre Portal", 0.15),
                    new Prioritize(
                        new Circle(4, 3, target: "Rock Bot"),
                        new Wander(8)
                        ),
                    new State("Idle",
                        new PlayerWithinTransition(15, "Attack")
                        ),
                    new State("Attack",
                        new Shoot(8, shoots: 3, shootAngle: 20, coolDown: 800),
                        new HealGroup(8, "Steels", coolDown: 1000),
                        new NoPlayerWithinTransition(30, "Idle"),
                        new HpLessTransition(0.2, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Decay(0)
                        )
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.01),
                    new ItemLoot("Health Potion", 0.04),
                    new ItemLoot("Magic Potion", 0.01),
                    new ItemLoot("Tincture of Life", 0.01)
                    ),
                new Threshold(0.04,
                    new ItemLoot("Potion of Attack", 0.03)
                    )
            )

            .Init("Steel Bot",
                new State(
                    new Prioritize(
                        new Circle(4, 3, target: "Rock Bot"),
                        new Wander(8)
                        ),
                    new State("Idle",
                        new PlayerWithinTransition(15, "Attack")
                        ),
                    new State("Attack",
                        new Shoot(8, shoots: 3, shootAngle: 20, coolDown: 800),
                        new HealGroup(8, "Rocks", coolDown: 1000),
                        new Taunt(0.5, "Silly squishy. We heal our brothers in a circle."),
                        new NoPlayerWithinTransition(30, "Idle"),
                        new HpLessTransition(0.2, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Decay(0)
                        )
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.01),
                    new ItemLoot("Health Potion", 0.04),
                    new ItemLoot("Magic Potion", 0.01)
                    ),
                new Threshold(0.04,
                    new ItemLoot("Potion of Attack", 0.03)
                    )
            )

            .Init("Djinn",
                new State(
                    new State("Idle",
                        new Prioritize(
                            new StayAbove(10, 200),
                            new Wander(8)
                            ),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok,
                        new Reproduce(max: GodParams.MAX_AMOUNT, range: GodParams.RANGE),
                        new PlayerWithinTransition(8, "Attacking")
                        ),
                    new State("Attacking",
                        new State("Bullet",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 0, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 100, coolDownOffset: 200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 110, coolDownOffset: 400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 120, coolDownOffset: 600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 130, coolDownOffset: 800, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 140, coolDownOffset: 1000, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 150, coolDownOffset: 1200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 160, coolDownOffset: 1400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 170, coolDownOffset: 1600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 180, coolDownOffset: 1800, shootAngle: 90),
                            new Shoot(1, shoots: 8, coolDown: 10000, direction: 180, coolDownOffset: 2000, shootAngle: 45),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 180, coolDownOffset: 0, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 170, coolDownOffset: 200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 160, coolDownOffset: 400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 150, coolDownOffset: 600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 140, coolDownOffset: 800, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 130, coolDownOffset: 1000, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 120, coolDownOffset: 1200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 110, coolDownOffset: 1400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 100, coolDownOffset: 1600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 1800, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 2000, shootAngle: 22.5),
                            new TimedTransition(2000, "Wait")
                            ),
                        new State("Wait",
                            new Chase(7, range: 0.5),
                            new Flashing(0xff00ff00, 0.1, 20),
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2000, "Bullet")
                            ),
                        new NoPlayerWithinTransition(13, "Idle"),
                        new HpLessTransition(0.5, "FlashBeforeExplode")
                        ),
                    new State("FlashBeforeExplode",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff0000, 0.3, 3),
                        new TimedTransition(1000, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Suicide()
                        )
                    ),
                new Threshold(.01,
                    new TierLoot(6, ItemType.Weapon, 0.04),
                    new TierLoot(7, ItemType.Weapon, 0.02),
                    new TierLoot(7, ItemType.Armor, 0.04),
                    new TierLoot(8, ItemType.Armor, 0.02),
                    new TierLoot(3, ItemType.Ring, 0.015),
                    new TierLoot(4, ItemType.Ring, 0.005),
                    new TierLoot(4, ItemType.Ability, 0.02)
                    ),
                new Threshold(0.07,
                    new ItemLoot("Potion of Speed", 0.07)
                    )
            )

            .Init("Leviathan",
                new State(
                    new TransformOnDeath("Ice Cave Portal", probability: .01),
                    new State("Wander",
                        new Swirl(10),
                        new Shoot(10, 2, 10, 1, coolDown: 500),
                        new TimedTransition(5000, "Triangle")
                        ),
                    new State("Triangle",
                        new State("1",
                            new Circle(speed: 10, orbitClockwise: true),
                            new Shoot(1, 3, 120, direction: 34, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 38, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 42, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 46, coolDown: 300),
                            new TimedTransition(1500, "2")
                            ),
                        new State("2",
                            new Circle(speed: 10, orbitClockwise: true),
                            new Shoot(1, 3, 120, direction: 94, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 98, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 102, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 106, coolDown: 300),
                            new TimedTransition(1500, "3")
                            ),
                        new State("3",
                            new Circle(speed: 10, orbitClockwise: true),
                            new Shoot(1, 3, 120, direction: 274, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 278, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 282, coolDown: 300),
                            new Shoot(1, 3, 120, direction: 286, coolDown: 300),
                            new TimedTransition(1500, "Wander"))
                        )
                    ),
                new Threshold(.01,
                    new ItemLoot("Potion of Defense", 0.07),
                    new TierLoot(6, ItemType.Weapon, 0.01),
                    new ItemLoot("Health Potion", 0.04),
                    new ItemLoot("Magic Potion", 0.01)
                    )
            )
        ;
    }
}