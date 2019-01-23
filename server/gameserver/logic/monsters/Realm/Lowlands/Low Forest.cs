using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ LowlansLowForest = () => Behav()
            .Init("Elf Wizard",
                new State(
                    new DropPortalOnDeath("Forest Maze Portal", 0.2),
                    new State("idle",
                        new Wander(),
                        new PlayerWithinTransition(11, "move1")
                        ),
                    new State("move1",
                        new Shoot(10, shoots: 3, shootAngle: 14, aim: 0.3),
                        new Prioritize(
                            new StayAbove(4, 14),
                            new BackAndForth(8)
                            ),
                        new TimedTransition(2000, "move2")
                        ),
                    new State("move2",
                        new Shoot(10, shoots: 3, shootAngle: 10, aim: 0.5),
                        new Prioritize(
                            new StayAbove(4, 14),
                            new Chase(6, sightRange: 10.5, range: 3),
                            new Wander()
                            ),
                        new TimedTransition(2000, "move3")
                        ),
                    new State("move3",
                        new Prioritize(
                            new StayAbove(4, 14),
                            new Retreat(6, range: 5),
                            new Wander()
                            ),
                        new TimedTransition(2000, "idle")
                        ),
                    new Spawn("Elf Archer", maxChildren: 2, coolDown: 15000),
                    new Spawn("Elf Swordsman", maxChildren: 4, coolDown: 7000),
                    new Spawn("Elf Mage", maxChildren: 1, coolDown: 8000)
                    ),
                new PinkBag( ItemType.Weapon,2),
                new PinkBag(ItemType.Armor,2),
                new PinkBag(ItemType.Ring,1),
                new PinkBag(ItemType.Ability,1),
                new ItemLoot("Health Potion", 0.02),
                new ItemLoot("Magic Potion", 0.02)
            )

            .Init("Elf Archer",
                new State(
                    new Shoot(10, aim: 1),
                    new Prioritize(
                        new Circle(5, 3, speedVariance: 0.1, radiusVariance: 0.5),
                        new Protect(12, "Elf Wizard", sightRange: 30, protectRange: 10, reprotectRange: 1),
                        new Wander()
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Elf Swordsman",
                new State(
                    new Shoot(10, aim: 1),
                    new Prioritize(
                        new Protect(12, "Elf Wizard", sightRange: 15, protectRange: 10, reprotectRange: 5),
                        new Buzz(10, range: 1, coolDown: 2000),
                        new Circle(6, 3, speedVariance: 0.1, radiusVariance: 0.5),
                        new Wander()
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Elf Mage",
                new State(
                    new Shoot(8, coolDown: 300),
                    new Prioritize(
                        new Circle(5, 3),
                        new Protect(12, "Elf Wizard", sightRange: 30, protectRange: 10, reprotectRange: 1),
                        new Wander()
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Goblin Mage",
                new State(
                    new DropPortalOnDeath("Forest Maze Portal", 0.2),
                    new State("unharmed",
                        new Shoot(8, index: 0, aim: 0.35, coolDown: 1000),
                        new Shoot(8, index: 1, aim: 0.35, coolDown: 1300),
                        new Prioritize(
                            new StayAbove(4, 16),
                            new Chase(5, sightRange: 10.5, range: 4),
                            new Wander()
                            ),
                        new HpLessTransition(0.65, "activate_horde")
                        ),
                    new State("activate_horde",
                        new Shoot(8, index: 0, aim: 0.25, coolDown: 1000),
                        new Shoot(8, index: 1, aim: 0.25, coolDown: 1000),
                        new Flashing(0xff484848, 0.6, 5000),
                        new EntityOrder(12, "Goblin Rogue", "help"),
                        new EntityOrder(12, "Goblin Warrior", "help"),
                        new Prioritize(
                            new StayAbove(4, 16),
                            new Retreat(5, range: 6)
                            )
                        ),
                    new Spawn("Goblin Rogue", maxChildren: 7, coolDown: 12000),
                    new Spawn("Goblin Warrior", maxChildren: 7, coolDown: 12000)
                    ),
                new PinkBag(ItemType.Weapon,3),
                new PinkBag(ItemType.Armor,3),
                new PinkBag(ItemType.Ring,1),
                new PinkBag(ItemType.Ability,1),
                new ItemLoot("Health Potion", 0.02),
                new ItemLoot("Magic Potion", 0.02)
            )

            .Init("Goblin Rogue",
                new State(
                    new State("protect",
                        new Protect(8, "Goblin Mage", sightRange: 12, protectRange: 1.5, reprotectRange: 1.5),
                        new TimedTransition(1200, "scatter", randomized: true)
                        ),
                    new State("scatter",
                        new Circle(8, 7, target: "Goblin Mage", radiusVariance: 1),
                        new TimedTransition(2400, "protect")
                        ),
                    new Shoot(3),
                    new State("help",
                        new Protect(8, "Goblin Mage", sightRange: 12, protectRange: 6, reprotectRange: 3),
                        new Chase(8, sightRange: 10.5, range: 1.5),
                        new EntityNotExistsTransition("Goblin Mage", 15, "protect")
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Goblin Warrior",
                new State(
                    new State("protect",
                        new Protect(8, "Goblin Mage", sightRange: 12, protectRange: 1.5, reprotectRange: 1.5),
                        new TimedTransition(1200, "scatter", randomized: true)
                        ),
                    new State("scatter",
                        new Circle(8, 7, target: "Goblin Mage", radiusVariance: 1),
                        new TimedTransition(2400, "protect")
                        ),
                    new Shoot(3),
                    new State("help",
                        new Protect(8, "Goblin Mage", sightRange: 12, protectRange: 6, reprotectRange: 3),
                        new Chase(8, sightRange: 10.5, range: 1.5),
                        new EntityNotExistsTransition("Goblin Mage", 15, "protect")
                        ),
                    new DropPortalOnDeath("Pirate Cave Portal", .01)
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Easily Enraged Bunny",
                new State(
                    new TransformOnDeath(target: "Enraged Bunny"),
                    new Chase(),
                    new Wander()
                )
            )

            .Init("Enraged Bunny",
                new State(
                    new DropPortalOnDeath("Forest Maze Portal", 0.2),
                    new Shoot(9, aim: 0.5, coolDown: 400),
                    new State("red",
                        new Flashing(0xff0000, 1.5, 1),
                        new TimedTransition(1600, "yellow")
                        ),
                    new State("yellow",
                        new Flashing(0xffff33, 1.5, 1),
                        new TimedTransition(1600, "orange")
                        ),
                    new State("orange",
                        new Flashing(0xff9900, 1.5, 1),
                        new TimedTransition(1600, "red")
                        ),
                    new Prioritize(
                        new StayAbove(4, 15),
                        new Chase(8.5, sightRange: 9, range: 2.5),
                        new Wander(8.5)
                        )
                    ),
                new ItemLoot("Health Potion", 0.01),
                new ItemLoot("Magic Potion", 0.02)
            )

            .Init("Forest Nymph",
                new State(
                    new State("circle",
                        new Shoot(4, index: 0, shoots: 1, aim: 0.1, coolDown: 900),
                        new Prioritize(
                            new StayAbove(4, 25),
                            new Chase(9, sightRange: 11, range: 3.5, duration: 1000, coolDown: 5000),
                            new Circle(13, 3.5, sightRange: 12),
                            new Wander(7)
                            ),
                        new TimedTransition(4000, "dart_away")
                        ),
                    new State("dart_away",
                        new Shoot(9, index: 1, shoots: 6, direction: 20, shootAngle: 60, coolDown: 1400),
                        new Wander(),
                        new TimedTransition(3600, "circle")
                        ),
                    new DropPortalOnDeath("Pirate Cave Portal", .01)
                    ),
                new ItemLoot("Health Potion", 0.03),
                new ItemLoot("Magic Potion", 0.02)
            )
        ;
    }
}