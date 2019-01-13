using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
	partial class BehaviorDb
	{
		private _ DynoBot = () => Behav()
			.Init("Dyno Bot",
				new State(
					new State("default",
						new PlayerWithinTransition(8, "taunt")
						),
					new State("taunt",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Taunt("What's this? I see a heroic adventurer trying to face me..."),
						new TimedTransition(4500, "taunt1")
						),
					new State("taunt1",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Taunt("I am Dyno Bot, the master of all bots."),
						new TimedTransition(4500, "taunt2")
						),
					new State("taunt2",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Taunt("Prepare yourself challenger!"),
						new TimedTransition(4500, "7")
						),
			   new State("7",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Flashing(0x0083FF, 1, 2),
						new Taunt("Prepare to face my wrath!"),
						new TimedTransition(4500, "Fight1")
						),

						 new State("Fight1",
							 new RemCond(ConditionEffectIndex.Invulnerable),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 160, coolDownOffset: 1400, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 170, coolDownOffset: 1600, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 180, coolDownOffset: 1800, shootAngle: 90),
							new Shoot(1, 8, index: 1, coolDown: 4575, angleOffset: 180, coolDownOffset: 2000, shootAngle: 45),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 180, coolDownOffset: 0, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 170, coolDownOffset: 200, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 160, coolDownOffset: 400, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 150, coolDownOffset: 600, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 140, coolDownOffset: 800, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 130, coolDownOffset: 1000, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 120, coolDownOffset: 1200, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 110, coolDownOffset: 1400, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 4575, angleOffset: 100, coolDownOffset: 1600, shootAngle: 90),
						 new HpLessTransition(.5, "Fight2")
						),
					new State("Fight2",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Taunt("You have done well! But can you survive this?"),
						new Flashing(0x005db5, 1, 2),
						new RemCond(ConditionEffectIndex.Invulnerable),
						new AddCond(ConditionEffectIndex.Armored),
						new Shoot(25, index: 0, shoots: 4, shootAngle: 10, aim: 0.7, coolDown: 800,
							coolDownOffset: 800),
						new Shoot(25, index: 1, shoots: 2, shootAngle: 10, aim: 0.7, coolDown: 500,
							coolDownOffset: 500),
						new HpLessTransition(.3, "LastStand Taunt")
											),
				   new State("LastStand Taunt",
					   	new AddCond(ConditionEffectIndex.Invulnerable),
						new Flashing(0xFF0000, 2, 2),
						new Taunt("Get ready for the last phase!"),
						new TimedTransition(4500, "LastStand")
						),
						 new State("LastStand",
							 new RemCond(ConditionEffectIndex.Invulnerable),
						new AddCond(ConditionEffectIndex.Armored),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 160, coolDownOffset: 1400, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 170, coolDownOffset: 1600, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 180, coolDownOffset: 1800, shootAngle: 90),
							new Shoot(1, 8, index: 1, coolDown: 2575, angleOffset: 180, coolDownOffset: 2000, shootAngle: 45),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 180, coolDownOffset: 0, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 170, coolDownOffset: 200, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 160, coolDownOffset: 400, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 150, coolDownOffset: 600, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 140, coolDownOffset: 800, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 130, coolDownOffset: 1000, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 120, coolDownOffset: 1200, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 110, coolDownOffset: 1400, shootAngle: 90),
							new Shoot(1, 4, index: 1, coolDown: 2575, angleOffset: 100, coolDownOffset: 1600, shootAngle: 90),
						 new HpLessTransition(.1, "Dying")
						),
					new State("Dying",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Taunt(1.00, "You have done well to defeat me challenger. Well done..."),
						new TimedTransition(3250, "Dead")
						),
				   new State("Dead",
						new AddCond(ConditionEffectIndex.Invulnerable),
						new Shoot(7, shoots: 8, aim: 1, coolDown: 5000),
						new Suicide()
						)
					),
				new MostDamagers(5,
					new ItemLoot("Potion of Life", 0.5)
					  ),
				new Threshold(0.025,
					new WhiteBag(new[] { "Doom Spoon","The Mad Jester's Garment" }),
					new TierLoot(11, ItemType.Weapon, 0.5),
					new TierLoot(12, ItemType.Weapon, 0.3),
					new TierLoot(6, ItemType.Ability, 0.3),
					new TierLoot(12, ItemType.Armor, 0.5),
					new TierLoot(13, ItemType.Armor, 0.3),
					new TierLoot(6, ItemType.Ring, 0.35)
				)
			)
		;
	}
}