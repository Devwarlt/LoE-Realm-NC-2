using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ LowlansLowSand = () => Behav()
            .Init("Sandsman King",
                new State(
                    new Shoot(10, coolDown: 10000),
                    new Prioritize(
                        new StayAbove(4, 15),
                        new Chase(6, range: 4),
                        new Wander()
                        ),
                    new Spawn("Sandsman Archer", maxChildren: 2, coolDown: 10000),
                    new Spawn("Sandsman Sorcerer", maxChildren: 3, coolDown: 8000)
                    ),
				 new PinkBag(ItemType.Weapon, 3),
				 new PinkBag(ItemType.Armor, 3),
				 new PinkBag(ItemType.Ability, 1),
				 new PinkBag(ItemType.Ring, 1),
				new ItemLoot("Health Potion", 0.04)
            )

            .Init("Sandsman Archer",
                new State(
                    new Shoot(10, aim: 0.5),
                    new Prioritize(
                        new Circle(8, 3.25, sightRange: 15, target: "Sandsman King", radiusVariance: 0.5),
                        new Wander()
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Sandsman Sorcerer",
                new State(
                    new Shoot(10, index: 0, coolDown: 5000),
                    new Shoot(5, index: 1, coolDown: 400),
                    new Prioritize(
                        new Protect(12, "Sandsman King", sightRange: 15, protectRange: 6, reprotectRange: 5),
                        new Wander()
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Giant Crab",
                new State(
                    new State("idle",
                        new Prioritize(
                            new StayAbove(6, 13),
                            new Wander(6)
                            ),
                        new PlayerWithinTransition(11, "scuttle")
                        ),
                    new State("scuttle",
                        new Shoot(9, index: 0, coolDown: 1000),
                        new Shoot(9, index: 1, coolDown: 1000),
                        new Shoot(9, index: 2, coolDown: 1000),
                        new Shoot(9, index: 3, coolDown: 1000),
                        new State("move",
                            new Prioritize(
                                new Chase(10, sightRange: 10.6, range: 2),
                                new StayAbove(10, 25),
                                new Wander(6)
                                ),
                            new TimedTransition(400, "pause")
                            ),
                        new State("pause",
                            new TimedTransition(200, "move")
                            ),
                        new TimedTransition(4700, "tri-spit")
                        ),
                    new State("tri-spit",
                        new Shoot(9, index: 4, aim: 0.5, coolDownOffset: 1200, coolDown: 90000),
                        new Shoot(9, index: 4, aim: 0.5, coolDownOffset: 1800, coolDown: 90000),
                        new Shoot(9, index: 4, aim: 0.5, coolDownOffset: 2400, coolDown: 90000),
                        new State("move",
                            new Prioritize(
                                new Chase(10, sightRange: 10.6, range: 2),
                                new StayAbove(10, 25),
                                new Wander(6)
                                ),
                            new TimedTransition(400, "pause")
                            ),
                        new State("pause",
                            new TimedTransition(200, "move")
                            ),
                        new TimedTransition(3200, "idle")
                        ),
                    new DropPortalOnDeath("Pirate Cave Portal", 0.01,1)
                    ),
				 new PinkBag(ItemType.Weapon, 2),
				 new PinkBag(ItemType.Armor, 2),
				 new PinkBag(ItemType.Ability, 1),
				 new PinkBag(ItemType.Ring, 1),
				new ItemLoot("Health Potion", 0.02),
                new ItemLoot("Magic Potion", 0.02)
            )

            .Init("Sand Devil",
                new State(
                    new State("wander",
                        new Shoot(8, aim: 0.3, coolDown: 700),
                        new Prioritize(
                            new StayAbove(7, 10),
                            new Chase(7, sightRange: 10, range: 2.2),
                            new Wander(7)
                            ),
                        new TimedTransition(3000, "circle")
                        ),
                    new State("circle",
                        new Shoot(8, aim: 0.3, coolDownOffset: 1000, coolDown: 1000),
                        new Circle(7, 2, sightRange: 9),
                        new TimedTransition(3100, "wander")
                        ),
                    new DropPortalOnDeath("Pirate Cave Portal", 0.01, 1)
                    )
            )
        ;
    }
}