using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
	partial class BehaviorDb
	{
		private _ HighlandsHighForest = () => Behav()
			.Init("Lizard God",
				new State(
					new Spawn("Night Elf Archer", maxChildren: 4),
					new Spawn("Night Elf Warrior", maxChildren: 3),
					new Spawn("Night Elf Mage", maxChildren: 2),
					new Spawn("Night Elf Veteran", maxChildren: 2),
					new Spawn("Night Elf King", maxChildren: 1),
					new Prioritize(
						new StayAbove(3, 160),
						new Chase(10, range: 7),
						new Wander()
						),
					new State("idle",
						new PlayerWithinTransition(10.2, "normal_attack")
						),
					new State("normal_attack",
						new Shoot(10, shoots: 3, shootAngle: 3, aim: 0.5),
						new TimedTransition(4000, "if_cloaked")
						),
					new State("if_cloaked",
						new Shoot(10, shoots: 8, shootAngle: 45, direction: 20, coolDown: 1600, coolDownOffset: 400),
						new Shoot(10, shoots: 8, shootAngle: 45, direction: 42, coolDown: 1600, coolDownOffset: 1200),
						new PlayerWithinTransition(10, "normal_attack")
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

			.Init("Night Elf Archer",
				new State(
					new Shoot(10, aim: 1),
					new Prioritize(
						new StayAbove(4, 160),
						new Chase(15, range: 7),
						new Wander()
						)
					),
				new ItemLoot("Health Potion", 0.03)
			)

			.Init("Night Elf Warrior",
				new State(
					new Shoot(3, aim: 1),
					new Prioritize(
						new StayAbove(4, 160),
						new Chase(15, range: 1),
						new Wander()
						)
					),
				new ItemLoot("Health Potion", 0.03)
			)

			.Init("Night Elf Mage",
				new State(
					new Shoot(10, aim: 1),
					new Prioritize(
						new StayAbove(4, 160),
						new Chase(15, range: 7),
						new Wander()
						)
					),
				new ItemLoot("Health Potion", 0.03)
			)

			.Init("Night Elf Veteran",
				new State(
					new Shoot(10, aim: 1),
					new Prioritize(
						new StayAbove(4, 160),
						new Chase(15, range: 7),
						new Wander()
						)
					),
				new ItemLoot("Health Potion", 0.03)
			)

			.Init("Night Elf King",
				new State(
					new Shoot(10, aim: 1),
					new Prioritize(
						new StayAbove(4, 160),
						new Chase(15, range: 7),
						new Wander()
						)
					),
				new ItemLoot("Health Potion", 0.03)
			)

			.Init("Ogre King",
				new State(
					new Spawn("Ogre Warrior", maxChildren: 4, coolDown: 12000),
					new Spawn("Ogre Mage", maxChildren: 2, coolDown: 16000),
					new Spawn("Ogre Wizard", maxChildren: 2, coolDown: 20000),
					new State("idle",
						new Prioritize(
							new StayAbove(3, 160),
							new Wander(3)
							),
						new PlayerWithinTransition(10, "grenade_blade_combo")
						),
					new State("grenade_blade_combo",
						new State("grenade1",
							new Grenade(3, 60, coolDown: 100000),
							new Prioritize(
								new StayAbove(3, 160),
								new Wander(3)
								),
							new TimedTransition(2000, "grenade2")
							),
						new State("grenade2",
							new Grenade(3, 60, coolDown: 100000),
							new Prioritize(
								new StayAbove(5, 160),
								new Wander(5)
								),
							new TimedTransition(3000, "slow_follow")
							),
						new State("slow_follow",
							new Shoot(13, coolDown: 1000),
							new Prioritize(
								new StayAbove(4, 160),
								new Chase(4, sightRange: 9, range: 3.5, duration: 4),
								new Wander()
								),
							new TimedTransition(4000, "grenade1")
							),
						new HpLessTransition(0.45, "furious")
						),
					new State("furious",
						new Grenade(2.4, 55, range: 9, coolDown: 1500),
						new Prioritize(
							new StayAbove(6, 160),
							new Wander(6)
							),
						new TimedTransition(12000, "idle")
						)
					),
				new Drops(
					new OnlyOne(
						new PinkBag(ItemType.Weapon, 4),
						new PinkBag(ItemType.Weapon, 5)
						),
					new OnlyOne(
						new PinkBag(ItemType.Armor, 4),
						new PinkBag(ItemType.Armor, 5),
						new PinkBag(ItemType.Armor, 6)
						),
						new PinkBag(ItemType.Ability, 2),
						new PurpleBag(ItemType.Ring, 2)

					)
			)

			.Init("Ogre Warrior",
				new State(
					new Shoot(3, aim: 0.5),
					new Prioritize(
						new StayAbove(12, 160),
						new Protect(12, "Ogre King", sightRange: 15, protectRange: 10, reprotectRange: 5),
						new Chase(14, sightRange: 10.5, range: 1.6, duration: 2600, coolDown: 2200),
						new Circle(6, 6),
						new Wander()
						)
					),
				new ItemLoot("Magic Potion", 0.03)
			)

			.Init("Ogre Mage",
				new State(
					new Shoot(10, aim: 0.3),
					new Prioritize(
						new StayAbove(12, 160),
						new Protect(12, "Ogre King", sightRange: 30, protectRange: 10, reprotectRange: 1),
						new Circle(5, 6),
						new Wander()
						)
					)
			)

			.Init("Ogre Wizard",
				new State(
					new Shoot(10, coolDown: 300),
					new Prioritize(
						new StayAbove(12, 160),
						new Protect(12, "Ogre King", sightRange: 30, protectRange: 10, reprotectRange: 1),
						new Circle(5, 6),
						new Wander()
						)
					),
				new ItemLoot("Magic Potion", 0.03)
			)

			.Init("Dragon Egg",
				new State(
					new TransformOnDeath("White Dragon Whelp", probability: 0.3),
					new TransformOnDeath("Juvenile White Dragon", probability: 0.2),
					new TransformOnDeath("Adult White Dragon", probability: 0.1)
					)
			)

			.Init("White Dragon Whelp",
				new State(
					new Shoot(10, shoots: 2, shootAngle: 20, aim: 0.3, coolDown: 750),
					new Prioritize(
						new StayAbove(10, 150),
						new Chase(20, range: 2.5, sightRange: 10.5, duration: 2200, coolDown: 3200),
						new Wander(9)
						)
					)
			)

			.Init("Juvenile White Dragon",
				new State(
					new Shoot(10, shoots: 2, shootAngle: 20, aim: 0.3, coolDown: 750),
					new Prioritize(
						new StayAbove(90, 150),
						new Chase(18, range: 2.2, sightRange: 10.5, duration: 3000, coolDown: 3000),
						new Wander(7.5)
						)
					),
			   new Drops(
					new PurpleBag(ItemType.Weapon, 7),
					new PurpleBag(ItemType.Armor, 7),
					new PinkBag(ItemType.Armor, 6)
					)
			)

			.Init("Adult White Dragon",
				new State(
					new Shoot(10, shoots: 3, shootAngle: 15, aim: 0.3, coolDown: 750),
					new Prioritize(
						new StayAbove(90, 150),
						new Chase(14, range: 1.8, sightRange: 10.5, duration: 4000, coolDown: 2000),
						new Wander(7.5)
						)
					),
				new ItemLoot("Health Potion", 0.03),
				new ItemLoot("Magic Potion", 0.03),
				new Drops(
					new PurpleBag(ItemType.Armor, 7),
					// new ItemLoot("Seal of the Divine", 0.015),
					new ItemLoot("White Drake Egg", 0.004)
					// new ItemLoot("Eternal Rest", 0.01),
					// new ItemLoot("Lightweight Leather Armor", 0.01),
					// new ItemLoot("Helm of the Barbarian Leader", 0.01),
					// new ItemLoot("Heart Dragon", 0.01)
					)
			)

			.Init("Beer God",
				new State(
					new State("Waiting Player",
						new AddCond(ConditionEffectIndex.Invincible),
						new PlayerWithinTransition(8, "yay i am good")
						),
					new State("yay i am good",
						new AddCond(ConditionEffectIndex.Invincible),
						new SetAltTexture(1),
						new ChangeSize(20, 100),
						new TimedTransition(2000, "Attack")
						),
					new State("Attack",
						new SetAltTexture(0),
						new Chase(12, 15, 0),
						new Shoot(20, shoots: 1, index: 0, coolDown: 1000),
						new PlayerWithinTransition(1, "BEER")
						),
					new State("BEER",
						new Shoot(20, shoots: 10, index: 1, shootAngle: 36, coolDown: 700),
						new Shoot(20, shoots: 1, index: 0, coolDown: 1000),
						new NoPlayerWithinTransition(4, "Attack")
						)
					),
				new Drops(
					new BlueBag(Potions.POTION_OF_DEFENSE),
					new OnlyOne(
					new ItemLoot("Potion of Attack", 0.1),
					new ItemLoot("Realm-wheat Hefeweizen", 0.2),
					new ItemLoot("Mad God Ale", 0.2),
					new ItemLoot("Oryx Stout", 0.2),
					new ItemLoot("Moloch's bammer weed", 0.02)
						)
                    )
            )
        ;
    }
}