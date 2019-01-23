using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ HighlandsHighPlains = () => Behav()
            .Init("Undead Dwarf God",
                new State(
                    new Spawn("Undead Dwarf Warrior", maxChildren: 3),
                    new Spawn("Undead Dwarf Axebearer", maxChildren: 3),
                    new Spawn("Undead Dwarf Mage", maxChildren: 3),
                    new Spawn("Undead Dwarf King", maxChildren: 2),
                    new Spawn("Soulless Dwarf", maxChildren: 1),
                    new Prioritize(
                        new StayAbove(3, 160),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(10, index: 0, shoots: 3, shootAngle: 15),
                    new Shoot(10, index: 1, aim: 0.5, coolDown: 1200)
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

            .Init("Undead Dwarf Warrior",
                new State(
                    new Shoot(3),
                    new Prioritize(
                        new StayAbove(10, 160),
                        new Chase(10, range: 1),
                        new Wander()
                        )
                    )
            )

            .Init("Undead Dwarf Axebearer",
                new State(
                    new Shoot(3),
                    new Prioritize(
                        new StayAbove(10, 160),
                        new Chase(10, range: 1),
                        new Wander()
                        )
                    )
            )

            .Init("Undead Dwarf Mage",
                new State(
                    new State("circle_player",
                        new Shoot(8, aim: 0.3, coolDown: 1000, coolDownOffset: 500),
                        new Prioritize(
                            new StayAbove(7, 160),
                            new Protect(7, "Undead Dwarf King", sightRange: 11, protectRange: 10, reprotectRange: 3),
                            new Circle(7, 3.5, sightRange: 11),
                            new Wander(7)
                            ),
                        new TimedTransition(3500, "circle_king")
                        ),
                    new State("circle_king",
                        new Shoot(8, shoots: 5, shootAngle: 72, defaultAngle: 20, aim: 0.3, coolDown: 1600, coolDownOffset: 500),
                        new Shoot(8, shoots: 5, shootAngle: 72, defaultAngle: 33, aim: 0.3, coolDown: 1600, coolDownOffset: 1300),
                        new Prioritize(
                            new StayAbove(7, 160),
                            new Circle(12, 2.5, target: "Undead Dwarf King", sightRange: 12, radiusVariance: 0.1, speedVariance: 0.1),
                            new Wander(7)
                            ),
                        new TimedTransition(3500, "circle_player")
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Undead Dwarf King",
                new State(
                    new Shoot(3),
                    new Prioritize(
                        new StayAbove(10, 160),
                        new Chase(8, range: 1.4),
                        new Wander()
                        )
                    ),
                new ItemLoot("Health Potion", 0.03)
            )

            .Init("Soulless Dwarf",
                new State(
                    new Shoot(10),
                    new State("idle",
                        new PlayerWithinTransition(10.5, "run1")
                        ),
                    new State("run1",
                        new Prioritize(
                            new StayAbove(4, 160),
                            new Protect(11, "Undead Dwarf God", sightRange: 16, protectRange: 10, reprotectRange: 1),
                            new Wander()
                            ),
                        new TimedTransition(2000, "run2")
                        ),
                    new State("run2",
                        new Prioritize(
                            new StayAbove(4, 160),
                            new Retreat(8, range: 4),
                            new Wander()
                            ),
                        new TimedTransition(1400, "run3")
                        ),
                    new State("run3",
                        new Prioritize(
                            new StayAbove(4, 160),
                            new Protect(10, "Undead Dwarf King", sightRange: 16, protectRange: 2, reprotectRange: 2),
                            new Protect(10, "Undead Dwarf Axebearer", sightRange: 16, protectRange: 2, reprotectRange: 2),
                            new Protect(10, "Undead Dwarf Warrior", sightRange: 16, protectRange: 2, reprotectRange: 2),
                            new Wander()
                            ),
                        new TimedTransition(2000, "idle")
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Shield Orc Shield",
                new State(
                    new Prioritize(
                        new Circle(10, 3, target: "Shield Orc Flooder"),
                        new Wander(1)
                        ),
                    new State("Attacking",
                        new State("Attack",
                            new Flashing(0xff000000, 10, 100),
                            new Shoot(10, coolDown: 500),
                            new HpLessTransition(0.5, "Heal"),
                            new EntityNotExistsTransition("Shield Orc Key", 7, "Idling")
                            ),
                        new State("Heal",
                            new HealGroup(7, "Shield Orcs", coolDown: 500),
                            new TimedTransition(500, "Attack"),
                            new EntityNotExistsTransition("Shield Orc Key", 7, "Idling")
                            )
                        ),
                    new State("Flash",
                        new Flashing(0xff0000, 1, 1),
                        new TimedTransition(300, "Idling")
                        ),
                    new State("Idling")
                    ),
                new ItemLoot("Health Potion", 0.04),
                new ItemLoot("Magic Potion", 0.01),
                new PurpleBag( ItemType.Ring, 2)
            )

            .Init("Shield Orc Flooder",
                new State(
                    new Prioritize(
                        new Wander(1)
                        ),
                    new State("Attacking",
                        new State("Attack",
                            new Flashing(0xff000000, 10, 100),
                            new Shoot(10, coolDown: 500),
                            new HpLessTransition(0.5, "Heal"),
                            new EntityNotExistsTransition("Shield Orc Key", 7, "Idling")
                            ),
                        new State("Heal",
                            new HealGroup(7, "Shield Orcs", coolDown: 500),
                            new TimedTransition(500, "Attack"),
                            new EntityNotExistsTransition("Shield Orc Key", 7, "Idling")
                            )
                        ),
                    new State("Flash",
                        new Flashing(0xff0000, 1, 1),
                        new TimedTransition(300, "Idling")
                        ),
                    new State("Idling")
                    ),
                new ItemLoot("Health Potion", 0.04),
                new ItemLoot("Magic Potion", 0.01),
                new PurpleBag(ItemType.Ability, 4)
            )

            .Init("Shield Orc Key",
                new State(
                    new Spawn("Shield Orc Flooder", maxChildren: 1, initialSpawn: 1, coolDown: 10000),
                    new Spawn("Shield Orc Shield", maxChildren: 1, initialSpawn: 1, coolDown: 10000),
                    new Spawn("Shield Orc Shield", maxChildren: 1, initialSpawn: 1, coolDown: 10000),
                    new State("Start",
                        new TimedTransition(500, "Attacking")
                        ),
                    new State("Attacking",
                        new Circle(10, 3, target: "Shield Orc Flooder"),
                        new EntityOrder(7, "Shield Orc Flooder", "Attacking"),
                        new EntityOrder(7, "Shield Orc Shield", "Attacking"),
                        new HpLessTransition(0.5, "FlashBeforeExplode")
                        ),
                    new State("FlashBeforeExplode",
                        new EntityOrder(7, "Shield Orc Flooder", "Flash"),
                        new EntityOrder(7, "Shield Orc Shield", "Flash"),
                        new Flashing(0xff0000, 1, 1),
                        new TimedTransition(300, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Decay(0)
                        )
                    ),
                new ItemLoot("Health Potion", 0.04),
                new ItemLoot("Magic Potion", 0.01),
                new PinkBag(ItemType.Armor, 4)
            )

            .Init("Left Horizontal Trap",
                new State(
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new State("weak_effect",
                        new Shoot(1, direction: 0, index: 0, coolDown: 200),
                        new TimedTransition(2000, "blind_effect")
                        ),
                    new State("blind_effect",
                        new Shoot(1, direction: 0, index: 1, coolDown: 200),
                        new TimedTransition(2000, "pierce_effect")
                        ),
                    new State("pierce_effect",
                        new Shoot(1, direction: 0, index: 2, coolDown: 200),
                        new TimedTransition(2000, "weak_effect")
                        ),
                    new Decay(6000)
                    )
            )

            .Init("Top Vertical Trap",
                new State(
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new State("weak_effect",
                        new Shoot(1, direction: 90, index: 0, coolDown: 200),
                        new TimedTransition(2000, "blind_effect")
                        ),
                    new State("blind_effect",
                        new Shoot(1, direction: 90, index: 1, coolDown: 200),
                        new TimedTransition(2000, "pierce_effect")
                        ),
                    new State("pierce_effect",
                        new Shoot(1, direction: 90, index: 2, coolDown: 200),
                        new TimedTransition(2000, "weak_effect")
                        ),
                    new Decay(6000)
                    )
            )

            .Init("45-225 Diagonal Trap",
                new State(
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new State("weak_effect",
                        new Shoot(1, direction: 45, index: 0, coolDown: 200),
                        new TimedTransition(2000, "blind_effect")
                        ),
                    new State("blind_effect",
                        new Shoot(1, direction: 45, index: 1, coolDown: 200),
                        new TimedTransition(2000, "pierce_effect")
                        ),
                    new State("pierce_effect",
                        new Shoot(1, direction: 45, index: 2, coolDown: 200),
                        new TimedTransition(2000, "weak_effect")
                        ),
                    new Decay(6000)
                    )
            )

            .Init("135-315 Diagonal Trap",
                new State(
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new State("weak_effect",
                        new Shoot(1, direction: 135, index: 0, coolDown: 200),
                        new TimedTransition(2000, "blind_effect")
                        ),
                    new State("blind_effect",
                        new Shoot(1, direction: 135, index: 1, coolDown: 200),
                        new TimedTransition(2000, "pierce_effect")
                        ),
                    new State("pierce_effect",
                        new Shoot(1, direction: 135, index: 2, coolDown: 200),
                        new TimedTransition(2000, "weak_effect")
                        ),
                    new Decay(6000)
                    )
            )

            .Init("Urgle",
                new State(
                    new DropPortalOnDeath("Spider Den Portal", 0.1),
                    new Prioritize(
                        new StayCloseToSpawn(8, 3),
                        new Wander(5)
                        ),
                    new Shoot(8, aim: 0.3),
                    new State("idle",
                        new PlayerWithinTransition(10.5, "toss_horizontal_traps")
                        ),
                    new State("toss_horizontal_traps",
                        new TossObject("Left Horizontal Trap", range: 9, angle: 230, coolDown: 100000),
                        new TossObject("Left Horizontal Trap", range: 10, angle: 180, coolDown: 100000),
                        new TossObject("Left Horizontal Trap", range: 9, angle: 140, coolDown: 100000),
                        new TimedTransition(1000, "toss_vertical_traps")
                        ),
                    new State("toss_vertical_traps",
                        new TossObject("Top Vertical Trap", range: 8, angle: 200, coolDown: 100000),
                        new TossObject("Top Vertical Trap", range: 10, angle: 240, coolDown: 100000),
                        new TossObject("Top Vertical Trap", range: 10, angle: 280, coolDown: 100000),
                        new TossObject("Top Vertical Trap", range: 8, angle: 320, coolDown: 100000),
                        new TimedTransition(1000, "toss_diagonal_traps")
                        ),
                    new State("toss_diagonal_traps",
                        new TossObject("45-225 Diagonal Trap", range: 2, angle: 45, coolDown: 100000),
                        new TossObject("45-225 Diagonal Trap", range: 7, angle: 45, coolDown: 100000),
                        new TossObject("45-225 Diagonal Trap", range: 11, angle: 225, coolDown: 100000),
                        new TossObject("45-225 Diagonal Trap", range: 6, angle: 225, coolDown: 100000),
                        new TossObject("135-315 Diagonal Trap", range: 2, angle: 135, coolDown: 100000),
                        new TossObject("135-315 Diagonal Trap", range: 7, angle: 135, coolDown: 100000),
                        new TossObject("135-315 Diagonal Trap", range: 11, angle: 315, coolDown: 100000),
                        new TossObject("135-315 Diagonal Trap", range: 6, angle: 315, coolDown: 100000),
                        new TimedTransition(1000, "wait")
                        ),
                    new State("wait",
                        new TimedTransition(2400, "idle")
                        )
                    ),
                new Drops(
					new OnlyOne(
                        new PinkBag(ItemType.Weapon, 5),
					    new PinkBag(ItemType.Weapon, 6),
						new PinkBag(ItemType.Weapon, 7)
					),
					new OnlyOne(
					    new PinkBag(ItemType.Armor, 5),
						new PinkBag(ItemType.Armor, 6),
						new PinkBag(ItemType.Armor, 7)
					),
                    new PurpleBag(ItemType.Ring, 3),
                    new PurpleBag(ItemType.Ability,3)
                    )
            )
        ;
    }
}