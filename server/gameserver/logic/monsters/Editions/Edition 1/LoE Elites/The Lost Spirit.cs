using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
	partial class BehaviorDb
	{
		private _ LostSpirit = () => Behav()
			.Init("The Lost Spirit",
				new State(
					new ScaleHP(500),
					new TransformOnDeath("OM Loot Chest", 1, 1, 1),
					new HpLessTransition(0.12, "DyingPhase"),
					new State("idle",
						 new HpLessTransition(0.99, "Awakening")
						),
					new State("Awakening",
						new AddCond(ConditionEffectIndex.Invincible),
						new Flashing(0xFFFFFF, 2, 3),
						new TimedTransition(4768, "backandforth")
						),
				  new State("backandforth",
					  new Shoot(10, shoots: 8, shootAngle: 28, index: 6, coolDown: 3451),
					  new Shoot(10, shoots: 5, shootAngle: 28, index: 5, coolDown: 2600),
					  new Shoot(10, shoots: 3, shootAngle: 28, index: 4, coolDown: 2982),
					  new TimedTransition(8875, "spawnshades"),
					   new State("2",
						new Taunt(1.00, "There is no mercy here.", "Prepare to meet your doom!"),
					    new RemCond(ConditionEffectIndex.Invincible),
						new Prioritize(
							 new Chase(0.34, 8, 1),
							 new StayAbove(0.3, 5)
							),
						new Shoot(10, shoots: 12, index: 0, coolDown: 3000),
						new Shoot(10, shoots: 6, shootAngle: 60, index: 2, coolDown: 2000),
						new Shoot(10, shoots: 2, index: 1, coolDown: 1500),
						new TimedTransition(3250, "1")
						),
					 new State("1",
						new Grenade(5, 100, range: 6, coolDown: 2000),
						new ReturnToSpawn(once: false, speed: 0.4),
						new Shoot(10, shoots: 8, shootAngle: 28, index: 6, coolDown: 2600),
						new Shoot(10, shoots: 2, shootAngle: 36, index: 0, coolDown: 3000),
						new Shoot(10, shoots: 8, shootAngle: 60, index: 2, coolDown: 2000),
						new Shoot(10, shoots: 2, index: 1, coolDown: 1500),
						new TimedTransition(3250, "2")
						 )
						),
					new State("spawnshades",
						new TimedTransition(500, "shadesandrek")
						),
				  new State("shadesandrek",
					  new Shoot(10, shoots: 8, shootAngle: 28, index: 6, coolDown: 3451),
					  new Shoot(10, shoots: 5, shootAngle: 28, index: 5, coolDown: 2600),
					  new Shoot(10, shoots: 3, shootAngle: 28, index: 4, coolDown: 2982),
					  new TimedTransition(12875, "returntospawn"),
					new State("1",
						new SetAltTexture(0),
						new Taunt(0.80, "I shall rip you apart.", "Your life has come to a end."),
						new Prioritize(
							 new Chase(0.34, 8, 1),
							 new StayAbove(0.3, 5)
							),
						new Shoot(10, shoots: 12, index: 0, coolDown: 3000),
						new Shoot(10, shoots: 6, shootAngle: 60, index: 2, coolDown: 2000),
						new Shoot(10, shoots: 2, index: 1, coolDown: 1000),
						new Shoot(10, shoots: 2, index: 3, aim: 2, coolDown: 2500),
						new TimedTransition(3250, "2")
						),
					 new State("2",
						new ReturnToSpawn(once: false, speed: 0.4),
						new Shoot(10, shoots: 2, shootAngle: 36, index: 0, coolDown: 3000),
						new Shoot(10, shoots: 8, shootAngle: 60, index: 2, coolDown: 2000),
						new Shoot(10, shoots: 2, index: 1, coolDown: 1500),
						new Shoot(10, shoots: 2, index: 3, aim: 2, coolDown: 1700),
						new Grenade(5, 100, range: 6, coolDown: 2000),
						new TimedTransition(3250, "1")
						 )
						),
					new State("returntospawn",
						new ReturnToSpawn(once: true, speed: 0.5),
						new Shoot(10, shoots: 4, shootAngle: 36, index: 3, coolDown: 3000),
						new TimedTransition(2000, "invisibleattack")
						),
					  new State("invisibleattack",
					   new Shoot(10, shoots: 8, shootAngle: 28, index: 4, coolDown: 2600),
					   new Wander(0.1),
					   new AddCond(ConditionEffectIndex.Invincible),
					   new Shoot(10, shoots: 2, shootAngle: 36, index: 0, coolDown: 3000),
						new Shoot(10, shoots: 8, shootAngle: 60, index: 2, coolDown: 2000),
						new Shoot(10, shoots: 2, index: 1, coolDown: 1500),
						new Shoot(10, shoots: 2, index: 3, aim: 2, coolDown: 1700),
						new TimedTransition(7000, "backandforth")
						),
				   new State(
					  new AddCond(ConditionEffectIndex.Invincible),
					  new State("DyingPhase",
						new ReturnToSpawn(once: true, speed: 0.3),
						new Flashing(0xFFFFFF, 2, 3),
						new Taunt(1.00, "What a wonderful hero you are!", "I will see you soon, mortal."),
						new TimedTransition(4750, "restindark")
						),
					   new State("restindark",
						new Suicide()
						)
					  )
					)
			)
			.Init("SPIRIT Loot Chest",
			new State(
		  new State("timed",
		  new AddCond(ConditionEffectIndex.Invulnerable),
		  new Taunt("Loot Chest will be available in 5 seconds."),
		  new TimedTransition(5000, "loot"),
		  new RemCond(ConditionEffectIndex.Invincible)
			  ),
		  new State("loot"
			  )),
				 new Threshold(0.025,
					new ItemLoot("Potion of Defense", 1),
					new ItemLoot("Potion of Defense", 0.5),
					//LT
					new ItemLoot("Crossbow of the Frozen North", 0.01),
					new ItemLoot("Phantom Light-X", 0.01),
					//SRT
					new ItemLoot("Galatic Axe", 0.0094)
					 ),
				 new MostDamagers(10,
					LootTemplates.StatIncreasePotionsLoot()
					 ),

					 new MostDamagers(10,
					LootTemplates.StatIncreasePotionsLoot()


			)

		)
			;
	}
}