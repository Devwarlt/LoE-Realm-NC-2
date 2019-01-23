using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ LowlansLowPlains = () => Behav()
            .Init("Hobbit Mage",
                new State(
                    new DropPortalOnDeath("Forest Maze Portal", 0.2),
                    new State("idle",
                        new PlayerWithinTransition(12, "ring1")
                        ),
                    new State("ring1",
                        new Shoot(1, direction: 0, shoots: 15, shootAngle: 24, coolDown: 1200, index: 0),
                        new TimedTransition(400, "ring2")
                        ),
                    new State("ring2",
                        new Shoot(1, direction: 8, shoots: 15, shootAngle: 24, coolDown: 1200, index: 1),
                        new TimedTransition(400, "ring3")
                        ),
                    new State("ring3",
                        new Shoot(1, direction: 16, shoots: 15, shootAngle: 24, coolDown: 1200, index: 2),
                        new TimedTransition(400, "idle")
                        ),
                    new Prioritize(
                        new StayAbove(4, 9),
                        new Chase(7.5, range: 6),
                        new Wander()
                        ),
                    new Spawn("Hobbit Archer", maxChildren: 4, coolDown: 12000),
                    new Spawn("Hobbit Rogue", maxChildren: 3, coolDown: 6000)
                    ),
			     new PinkBag(ItemType.Weapon, 2),
			     new PinkBag(ItemType.Armor, 2),
				 new PinkBag(ItemType.Ability, 1),
				 new PinkBag(ItemType.Ring, 1),
				new ItemLoot("Health Potion", 0.02),
                new ItemLoot("Magic Potion", 0.02)
            )

            .Init("Hobbit Rogue",
                new State(
                    new Shoot(3),
                    new Prioritize(
                        new Protect(12, "Hobbit Mage", sightRange: 15, protectRange: 9, reprotectRange: 2.5),
                        new Chase(8.5, range: 1),
                        new Wander()
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Hobbit Archer",
                new State(
                    new Shoot(10),
                    new State("run1",
                        new Prioritize(
                            new Protect(11, "Hobbit Mage", sightRange: 12, protectRange: 10, reprotectRange: 1),
                            new Wander()
                            ),
                        new TimedTransition(400, "run2")
                        ),
                    new State("run2",
                        new Prioritize(
                            new Retreat(8, 4),
                            new Wander()
                            ),
                        new TimedTransition(600, "run3")
                        ),
                    new State("run3",
                        new Prioritize(
                            new Protect(10, "Hobbit Archer", sightRange: 16, protectRange: 2, reprotectRange: 2),
                            new Wander()
                            ),
                        new TimedTransition(400, "run1")
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Undead Hobbit Mage",
                new State(
                    new Shoot(10, index: 3),
                    new State("idle",
                        new PlayerWithinTransition(12, "ring1")
                        ),
                    new State("ring1",
                        new Shoot(1, direction: 0, shoots: 15, shootAngle: 24, coolDown: 1200, index: 0),
                        new TimedTransition(400, "ring2")
                        ),
                    new State("ring2",
                        new Shoot(1, direction: 8, shoots: 15, shootAngle: 24, coolDown: 1200, index: 1),
                        new TimedTransition(400, "ring3")
                        ),
                    new State("ring3",
                        new Shoot(1, direction: 16, shoots: 15, shootAngle: 24, coolDown: 1200, index: 2),
                        new TimedTransition(400, "idle")
                        ),
                    new Prioritize(
                        new StayAbove(4, 20),
                        new Chase(7.5, range: 6),
                        new Wander()
                        ),
                    new Spawn("Undead Hobbit Archer", maxChildren: 4, coolDown: 12000),
                    new Spawn("Undead Hobbit Rogue", maxChildren: 3, coolDown: 6000)
                    ),
			     new PinkBag(ItemType.Weapon, 3),
				 new PinkBag(ItemType.Armor, 3),
				 new PinkBag(ItemType.Ability, 1),
				 new PinkBag(ItemType.Ring, 1),
				new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Undead Hobbit Rogue",
                new State(
                    new Shoot(3),
                    new Prioritize(
                        new Protect(12, "Undead Hobbit Mage", sightRange: 15, protectRange: 9, reprotectRange: 2.5),
                        new Chase(8.5, range: 1),
                        new Wander()
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Undead Hobbit Archer",
                new State(
                    new Shoot(10),
                    new State("run1",
                        new Prioritize(
                            new Protect(11, "Undead Hobbit Mage", sightRange: 12, protectRange: 10, reprotectRange: 1),
                            new Wander()
                            ),
                        new TimedTransition(400, "run2")
                        ),
                    new State("run2",
                        new Prioritize(
                            new Retreat(8, 4),
                            new Wander()
                            ),
                        new TimedTransition(600, "run3")
                        ),
                    new State("run3",
                        new Prioritize(
                            new Protect(10, "Undead Hobbit Archer", sightRange: 16, protectRange: 2,
                                reprotectRange: 2),
                            new Wander()
                            ),
                        new TimedTransition(400, "run1")
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Sumo Master",
                new State(
                    new State("sleeping1",
                        new SetAltTexture(0),
                        new TimedTransition(1000, "sleeping2"),
                        new HpLessTransition(0.99, "hurt")
                        ),
                    new State("sleeping2",
                        new SetAltTexture(3),
                        new TimedTransition(1000, "sleeping1"),
                        new HpLessTransition(0.99, "hurt")
                        ),
                    new State("hurt",
                        new SetAltTexture(2),
                        new Spawn("Lil Sumo", coolDown: 200),
                        new TimedTransition(1000, "awake")
                        ),
                    new State("awake",
                        new SetAltTexture(1),
                        new Shoot(3, coolDown: 250),
                        new Prioritize(
                            new Chase(5, range: 1),
                            new Wander(5)
                            ),
                        new HpLessTransition(0.5, "rage")
                        ),
                    new State("rage",
                        new SetAltTexture(4),
                        new Taunt("Engaging Super-Mode!!!"),
                        new Prioritize(
                            new Chase(6, range: 1),
                            new Wander(6)
                            ),
                        new State("shoot",
                            new Shoot(8, index: 1, coolDown: 150),
                            new TimedTransition(700, "rest")
                            ),
                        new State("rest",
                            new TimedTransition(400, "shoot")
                            )
                        )
                    ),
                new ItemLoot("Health Potion", 0.05),
                new ItemLoot("Magic Potion", 0.05)
            )

            .Init("Lil Sumo",
                new State(
                    new Shoot(8),
                    new Prioritize(
                        new Circle(4, 2, target: "Sumo Master"),
                        new Wander()
                        )
                    ),
                new ItemLoot("Health Potion", 0.02),
                new ItemLoot("Magic Potion", 0.02)
            )
        ;
    }
}