using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ HighlandsHighSand = () => Behav()
            .Init("Flayer God",
                new State(
                    new Spawn("Flayer", maxChildren: 2),
                    new Spawn("Flayer Veteran", maxChildren: 3),
                    new Reproduce("Flayer God", max: 2),
                    new Prioritize(
                        new StayAbove(4, 155),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(10, index: 0, aim: 0.5, coolDown: 400),
                    new Shoot(10, index: 1, aim: 1)
                    ),
                new Drops(
					new OnlyOne(
                    new PinkBag(ItemType.Weapon, 5),
                    new PinkBag(ItemType.Weapon, 6),
                    new PurpleBag(ItemType.Weapon, 7)
					),
					new OnlyOne(
                    new PinkBag(ItemType.Armor, 5),
                    new PinkBag(ItemType.Armor, 6),
                    new PurpleBag(ItemType.Armor, 7)
					),
                    new PurpleBag(ItemType.Ring, 3),
                    new PurpleBag(ItemType.Ability, 3),
                    new ItemLoot("Purple Drake Egg", 0.005)
                    )
            )

            .Init("Flayer",
                new State(
                    new Shoot(10, aim: 0.5),
                    new Prioritize(
                        new StayAbove(10, 155),
                        new Chase(12, range: 7),
                        new Wander()
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Flayer Veteran",
                new State(
                    new Shoot(10, aim: 0.5),
                    new Prioritize(
                        new StayAbove(10, 155),
                        new Chase(12, range: 7),
                        new Wander()
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Flamer King",
                new State(
                    new Spawn("Flamer", maxChildren: 5, coolDown: 10000),
                    new State("Attacking",
                        new State("Charge",
                            new Chase(7, range: 0.1),
                            new PlayerWithinTransition(2, "Bullet")
                            ),
                        new State("Bullet",
                            new Flashing(0xffffaa00, 0.2, 20),
                            new ChangeSize(20, 140),
                            new Shoot(8, coolDown: 200),
                            new TimedTransition(4000, "Wait")
                            ),
                        new State("Wait",
                            new ChangeSize(-20, 80),
                            new TimedTransition(500, "Charge")
                            ),
                        new HpLessTransition(0.2, "FlashBeforeExplode")
                        ),
                    new State("FlashBeforeExplode",
                        new Flashing(0xffff0000, 1, 1),
                        new TimedTransition(300, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Decay(0)
                        )
                    ),
                new ItemLoot("Health Potion", 0.01),
                new ItemLoot("Magic Potion", 0.01),
                new PurpleBag(ItemType.Ring, 2)
            )

            .Init("Flamer",
                new State(
                    new State("Attacking",
                        new State("Charge",
                            new Prioritize(
                                new Protect(7, "Flamer King"),
                                new Chase(7, range: 0.1)
                                ),
                            new PlayerWithinTransition(2, "Bullet")
                            ),
                        new State("Bullet",
                            new Flashing(0xffffaa00, 0.2, 20),
                            new ChangeSize(20, 130),
                            new Shoot(8, coolDown: 200),
                            new TimedTransition(4000, "Wait")
                            ),
                        new State("Wait",
                            new ChangeSize(-20, 70),
                            new TimedTransition(600, "Charge")
                            ),
                        new HpLessTransition(0.2, "FlashBeforeExplode")
                        ),
                    new State("FlashBeforeExplode",
                        new Flashing(0xffff0000, 1, 1),
                        new TimedTransition(300, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Decay(0)
                        )
                    ),
                new ItemLoot("Magic Potion", 0.2),
                new PinkBag(ItemType.Weapon, 5)
            )

            .Init("Minotaur",
                new State(
                    new State("idle",
                        new StayAbove(6, 160),
                        new PlayerWithinTransition(10, "charge")
                        ),
                    new State("charge",
                        new Prioritize(
                            new StayAbove(6, 160),
                            new Chase(6, sightRange: 11, range: 1.6)
                            ),
                        new TimedTransition(200, "spam_blades")
                        ),
                    new State("spam_blades",
                        new Shoot(8, index: 0, shoots: 1, coolDown: 100000, coolDownOffset: 1000),
                        new Shoot(8, index: 0, shoots: 2, shootAngle: 16, coolDown: 100000, coolDownOffset: 1200),
                        new Shoot(8, index: 0, shoots: 3, aim: 0.2, coolDown: 100000, coolDownOffset: 1600),
                        new Shoot(8, index: 0, shoots: 1, shootAngle: 24, coolDown: 100000, coolDownOffset: 2200),
                        new Shoot(8, index: 0, shoots: 2, aim: 0.2, coolDown: 100000, coolDownOffset: 2800),
                        new Shoot(8, index: 0, shoots: 3, shootAngle: 16, coolDown: 100000, coolDownOffset: 3200),
                        new Prioritize(
                            new StayAbove(6, 160),
                            new Wander(6)
                            ),
                        new TimedTransition(4400, "blade_ring")
                        ),
                    new State("blade_ring",
                        new Shoot(7, direction: 0, shoots: 12, shootAngle: 30, coolDown: 800, index: 1, coolDownOffset: 600),
                        new Shoot(7, direction: 15, shoots: 6, shootAngle: 60, coolDown: 800, index: 2, coolDownOffset: 1000),
                        new Prioritize(
                            new StayAbove(6, 160),
                            new Chase(6, sightRange: 10, range: 1),
                            new Wander(6)
                            ),
                        new TimedTransition(3500, "pause")
                        ),
                    new State("pause",
                        new Prioritize(
                            new StayAbove(6, 160),
                            new Wander(6)
                            ),
                        new TimedTransition(1000, "idle")
                        )
                    ),
                new Drops(
					new OnlyOne(
                    new PinkBag(ItemType.Weapon, 5),
                    new PinkBag(ItemType.Weapon, 6),
                    new PurpleBag(ItemType.Weapon, 7)
					),
					new OnlyOne(
                    new PinkBag(ItemType.Armor, 5),
                    new PinkBag(ItemType.Armor, 6),
                    new PurpleBag(ItemType.Armor, 7)
					),
                    new PurpleBag(ItemType.Ring, 3),
                    new PurpleBag(ItemType.Ability, 3),
                    new ItemLoot("Purple Drake Egg", 0.005)
                    )
            )
        ;
    }
}