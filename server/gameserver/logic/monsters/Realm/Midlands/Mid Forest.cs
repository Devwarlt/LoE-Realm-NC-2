using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ MidlandMidForest = () => Behav()
            .Init("Dwarf King",
                new State(
                    new DropPortalOnDeath("Forest Maze Portal", 0.20),
                    new SpawnGroup("Dwarves", maxChildren: 10, coolDown: 8000),
                    new Shoot(4, coolDown: 2000),
                    new State("Circling",
                        new Prioritize(
                            new Circle(4, 2.7, sightRange: 11),
                            new Wander()
                            ),
                        new TimedTransition(3400, "Engaging")
                        ),
                    new State("Engaging",
                        new Taunt(0.2, "You'll taste my axe!"),
                        new Prioritize(
                            new Chase(10, sightRange: 15, range: 1),
                            new Wander()
                            ),
                        new TimedTransition(2600, "Circling")
                        )
                    ),
				new OnlyOne(
					new PinkBag(ItemType.Weapon, 3),
					new PinkBag(ItemType.Weapon, 4)
				),
				new OnlyOne(
					new PinkBag(ItemType.Armor, 3),
					new PinkBag(ItemType.Armor, 4),
					new PinkBag(ItemType.Armor, 5)
				),
					new PinkBag(ItemType.Ring, 1),
					new PinkBag(ItemType.Ability, 1),
					new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Dwarf Veteran",
                new State(
                    new Shoot(4),
                    new State("Default",
                        new Prioritize(
                            new Chase(10, sightRange: 9, range: 2, duration: 3000, coolDown: 1000),
                            new Wander()
                            )
                        ),
                    new State("Circling",
                        new Prioritize(
                            new Circle(4, 2.7, sightRange: 11),
                            new Protect(12, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander()
                            ),
                        new TimedTransition(3300, "Default"),
                        new EntityNotExistsTransition("Dwarf King", 8, "Default")
                        ),
                    new State("Engaging",
                        new Prioritize(
                            new Chase(10, sightRange: 15, range: 1),
                            new Protect(12, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander()
                            ),
                        new TimedTransition(2500, "Circling"),
                        new EntityNotExistsTransition("Dwarf King", 8, "Default")
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Dwarf Mage",
                new State(
                    new State("Default",
                        new Prioritize(
                            new Protect(12, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander(6)
                            ),
                        new State("fire1_def",
                            new Shoot(10, aim: 0.2, coolDown: 100000),
                            new TimedTransition(1500, "fire2_def")
                            ),
                        new State("fire2_def",
                            new Shoot(5, aim: 0.2, coolDown: 100000),
                            new TimedTransition(1500, "fire1_def")
                            )
                        ),
                    new State("Circling",
                        new Prioritize(
                            new Circle(4, 2.7, sightRange: 11),
                            new Protect(12, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander(6)
                            ),
                        new State("fire1_cir",
                            new Shoot(10, aim: 0.2, coolDown: 100000),
                            new TimedTransition(1500, "fire2_cir")
                            ),
                        new State("fire2_cir",
                            new Shoot(5, aim: 0.2, coolDown: 100000),
                            new TimedTransition(1500, "fire1_cir")
                            ),
                        new TimedTransition(3300, "Default"),
                        new EntityNotExistsTransition("Dwarf King", 8, "Default")
                        ),
                    new State("Engaging",
                        new Prioritize(
                            new Chase(10, sightRange: 15, range: 1),
                            new Protect(12, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander()
                            ),
                        new State("fire1_eng",
                            new Shoot(10, aim: 0.2, coolDown: 100000),
                            new TimedTransition(1500, "fire2_eng")
                            ),
                        new State("fire2_eng",
                            new Shoot(5, aim: 0.2, coolDown: 100000),
                            new TimedTransition(1500, "fire1_eng")
                            ),
                        new TimedTransition(2500, "Circling"),
                        new EntityNotExistsTransition("Dwarf King", 8, "Default")
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Dwarf Axebearer",
                new State(
                    new Shoot(3.4),
                    new State("Default",
                        new Wander()
                        ),
                    new State("Circling",
                        new Prioritize(
                            new Circle(4, 2.7, sightRange: 11),
                            new Protect(1.2, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander()
                            ),
                        new TimedTransition(3300, "Default"),
                        new EntityNotExistsTransition("Dwarf King", 8, "Default")
                        ),
                    new State("Engaging",
                        new Prioritize(
                            new Chase(10, sightRange: 15, range: 1),
                            new Protect(12, "Dwarf King", sightRange: 15, protectRange: 6, reprotectRange: 3),
                            new Wander()
                            ),
                        new TimedTransition(2500, "Circling"),
                        new EntityNotExistsTransition("Dwarf King", 8, "Default")
                        )
                    ),
                new ItemLoot("Health Potion", 0.04)
            )

            .Init("Werelion",
                new State(
                    new DropPortalOnDeath("Spider Den Portal", 0.1),
                    new Spawn("Weretiger", maxChildren: 1, coolDown: 23000),
                    new Spawn("Wereleopard", maxChildren: 2, coolDown: 9000),
                    new Spawn("Werepanther", maxChildren: 3, coolDown: 15000),
                    new Shoot(4, coolDown: 2000),
                    new State("idle",
                        new Prioritize(
                            new StayAbove(6, 60),
                            new Wander(6)
                            ),
                        new PlayerWithinTransition(11, "player_nearby")
                        ),
                    new State("player_nearby",
                        new State("normal_attack",
                            new Shoot(10, shoots: 3, shootAngle: 15, aim: 1, coolDown: 10000),
                            new TimedTransition(900, "if_cloaked")
                            ),
                        new State("if_cloaked",
                            new Shoot(10, shoots: 8, shootAngle: 45, defaultAngle: 20, coolDown: 1600, coolDownOffset: 400),
                            new Shoot(10, shoots: 8, shootAngle: 45, defaultAngle: 42, coolDown: 1600, coolDownOffset: 1200),
                            new PlayerWithinTransition(10, "normal_attack")
                            ),
                        new Prioritize(
                            new StayAbove(6, 60),
                            new Chase(4, sightRange: 7, range: 3),
                            new Wander(6)
                            ),
                        new TimedTransition(30000, "idle")
                        )
                    ),
				  new OnlyOne(
					new PinkBag(ItemType.Weapon, 4),
					new PinkBag(ItemType.Weapon, 5)
				),
				new OnlyOne(
					new PinkBag(ItemType.Armor, 5),
					new PinkBag(ItemType.Armor, 6)
				),
					new PurpleBag(ItemType.Ring, 2),
					new PurpleBag(ItemType.Ability, 2),
				new ItemLoot("Health Potion", 0.04),
                new ItemLoot("Magic Potion", 0.05)
            )

            .Init("Weretiger",
                new State(
                    new Shoot(8, aim: 0.3, coolDown: 1000),
                    new Prioritize(
                        new StayAbove(6, 60),
                        new Protect(11, "Werelion", sightRange: 12, protectRange: 10, reprotectRange: 5),
                        new Chase(8, range: 6.3),
                        new Wander(6)
                        )
                    ),
                new ItemLoot("Health Potion", 0.03)
            )

            .Init("Wereleopard",
                new State(
                    new Shoot(4.5, aim: 0.4, coolDown: 900),
                    new Prioritize(
                        new Protect(11, "Werelion", sightRange: 12, protectRange: 10, reprotectRange: 5),
                        new Chase(11, range: 3),
                        new Wander(10)
                        )
                    ),
                new ItemLoot("Health Potion", 0.03)
            )

            .Init("Werepanther",
                new State(
                    new State("idle",
                        new Protect(6.5, "Werelion", sightRange: 11, protectRange: 7.5, reprotectRange: 7.4),
                        new PlayerWithinTransition(9.5, "wander")
                        ),
                    new State("wander",
                        new Prioritize(
                            new Protect(6.5, "Werelion", sightRange: 11, protectRange: 7.5, reprotectRange: 7.4),
                            new Chase(6.5, range: 5, sightRange: 10),
                            new Wander(6.5)
                            ),
                        new PlayerWithinTransition(4, "jump")
                        ),
                    new State("jump",
                        new Prioritize(
                            new Protect(6.5, "Werelion", sightRange: 11, protectRange: 7.5, reprotectRange: 7.4),
                            new Chase(7, range: 1, sightRange: 6),
                            new Wander(5.5)
                            ),
                        new TimedTransition(200, "attack")
                        ),
                    new State("attack",
                        new Prioritize(
                            new Protect(6.5, "Werelion", sightRange: 11, protectRange: 7.5, reprotectRange: 7.4),
                            new Chase(range: 1, sightRange: 6),
                            new Wander(5)
                            ),
                        new Shoot(4, aim: 0.5, coolDown: 800, coolDownOffset: 300),
                        new TimedTransition(4000, "idle")
                        )
                    ),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Metal Golem",
                new State(
                    new State("idle",
                        new PlayerWithinTransition(11, "player_nearby")
                        ),
                    new State("player_nearby",
                        new Prioritize(
                            new StayAbove(14, 65),
                            new Chase(10, range: 3, duration: 3000, coolDown: 3000),
                            new Wander()
                            ),
                        new Spawn("Red Satellite", maxChildren: 1, coolDown: 200),
                        new Spawn("Gray Satellite 1", maxChildren: 1, coolDown: 200),
                        new State("slowshot",
                            new Shoot(10, aim: 0.5, coolDown: 300, coolDownOffset: 600),
                            new TimedTransition(5000, "megashot")
                            ),
                        new State("megashot",
                            new Flashing(0xffffffff, 0.2, 5),
                            new Shoot(10, aim: 0.2, coolDown: 90, coolDownOffset: 1000),
                            new TimedTransition(1200, "slowshot")
                            )
                        ),
                    new Reproduce(max: 1)
                    ),
                new TierLoot(5, ItemType.Weapon, 0.02),
                new ItemLoot("Health Potion", 0.03)
            )

            .Init("Clockwork Golem",
                new State(
                    new State("idle",
                        new PlayerWithinTransition(11, "player_nearby")
                        ),
                    new State("player_nearby",
                        new Spawn("Blue Satellite", maxChildren: 1, coolDown: 200),
                        new Spawn("Gray Satellite 2", maxChildren: 1, coolDown: 200),
                        new Shoot(10, aim: 0.5, coolDown: 700),
                        new Prioritize(
                            new StayAbove(14, 65),
                            new Chase(10, range: 3, duration: 3000, coolDown: 3000),
                            new Wander()
                            ),
                        new TimedTransition(12000, "idle")
                        ),
                    new Reproduce(max: 1)
                    ),
				new OnlyOne(
                new PinkBag(ItemType.Armor, 4),
                new PinkBag(ItemType.Armor, 6)
				),
                new ItemLoot("Health Potion", 0.03)
            )

            .Init("Horned Drake",
                new State(
                    new Spawn("Drake Baby", maxChildren: 1, initialSpawn: 1, coolDown: 50000),
                    new State("idle",
                        new StayAbove(8, 60),
                        new PlayerWithinTransition(10, "get_player")
                        ),
                    new State("get_player",
                        new Prioritize(
                            new StayAbove(8, 60),
                            new Chase(8, range: 2.7, sightRange: 10, duration: 5000, coolDown: 1800),
                            new Wander(8)
                            ),
                        new State("one_shot",
                            new Shoot(8, aim: 0.1, coolDown: 800),
                            new TimedTransition(900, "three_shot")
                            ),
                        new State("three_shot",
                            new Shoot(8, shoots: 3, shootAngle: 40, aim: 0.1, coolDown: 100000, coolDownOffset: 800),
                            new TimedTransition(2000, "one_shot")
                            )
                        ),
                    new State("protect_me",
                        new Protect(8, "Drake Baby", sightRange: 12, protectRange: 2.5, reprotectRange: 1.5),
                        new State("one_shot",
                            new Shoot(8, aim: 0.1, coolDown: 700),
                            new TimedTransition(800, "three_shot")
                            ),
                        new State("three_shot",
                            new Shoot(8, shoots: 3, shootAngle: 40, aim: 0.1, coolDown: 100000, coolDownOffset: 700),
                            new TimedTransition(1800, "one_shot")
                            ),
                        new EntityNotExistsTransition("Drake Baby", 8, "idle")
                        )
                    ),
				new OnlyOne(
					new PinkBag(ItemType.Weapon, 5),
					new PinkBag(ItemType.Weapon, 6)
					),
				new OnlyOne(
					new PinkBag(ItemType.Armor, 5),
					new PinkBag(ItemType.Armor, 6)
					),
				new OnlyOne(
					new PurpleBag(ItemType.Ring, 2),
					new PurpleBag(ItemType.Ring, 3)
					),
				new OnlyOne(
					new PurpleBag(ItemType.Ability, 2),
					new PurpleBag(ItemType.Ability, 3)
					),
				new ItemLoot("Health Potion", 0.09),
                new ItemLoot("Magic Potion", 0.12)
            )

            .Init("Drake Baby",
                new State(
                    new DropPortalOnDeath("Forest Maze Portal", 0.20),
                    new State("unharmed",
                        new Shoot(8, coolDown: 1500),
                        new State("wander",
                            new Prioritize(
                                new StayAbove(8, 60),
                                new Wander(8)
                                ),
                            new TimedTransition(2000, "find_mama")
                            ),
                        new State("find_mama",
                            new Prioritize(
                                new StayAbove(8, 60),
                                new Protect(14, "Horned Drake", sightRange: 15, protectRange: 4, reprotectRange: 4)
                                ),
                            new TimedTransition(2000, "wander")
                            ),
                        new HpLessTransition(0.65, "call_mama")
                        ),
                    new State("call_mama",
                        new Flashing(0xff484848, 0.6, 5000),
                        new State("get_close_to_mama",
                            new Taunt("Awwwk! Awwwk!"),
                            new Protect(14, "Horned Drake", sightRange: 15, protectRange: 1, reprotectRange: 1),
                            new TimedTransition(1500, "cry_for_mama")
                            ),
                        new State("cry_for_mama",
                            new Retreat(6.5, 8),
                            new EntityOrder(8, "Horned Drake", "protect_me")
                            )
                        )
                    )
            )

            .Init("Red Spider",
                new State(
                    new Wander(8),
                    new Shoot(9),
                    new Reproduce(max: 3, radius: 15, coolDown: 45000)
                    ),
                new ItemLoot("Health Potion", 0.03),
                new ItemLoot("Magic Potion", 0.03)
            )

            .Init("Black Bat",
                new State(
                    new Prioritize(
                        new Charge(),
                        new Wander()
                        ),
                    new Shoot(1),
                    new Reproduce(max: 5, radius: 20, coolDown: 20000)
                    ),
                new ItemLoot("Health Potion", 0.01),
                new ItemLoot("Magic Potion", 0.01),
                new PinkBag(ItemType.Armor, 2)
            )
        ;
    }
}