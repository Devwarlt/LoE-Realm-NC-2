using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.transitions;
using LoESoft.GameServer.logic.loot;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ SpriteWorldEnemies = () => Behav()
		  .Init("Limon the Sprite God",
			 new State(
				 new RealmPortalDrop(),
				 new State("idle",
					 new Flashing(0x66FF00, 0.6, 9),
					 new Wander(0.7),
					 new PlayerWithinTransition(12, "nothing")
				 ),
				 new State("nothing",
					 new Wander(.7),
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new TimedTransition(2000, "e.e")
					 ),
				 new State("e.e",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Prioritize(
					 new Charge(1, range: 4, coolDown: 1500),
					 new Wander(10),
					 new StayAbove(0.45, 4)
					 ),
					 new Shoot(10, 2, 20, angleOffset: 0 / 2, index: 0, coolDown: 300),
					 new Shoot(10, 1, 0, defaultAngle: 180, angleOffset: 180, index: 0, aim: 1,
					 coolDown: 300, coolDownOffset: 0),
					 new TimedTransition(3000, ":c")
					 ),
				 new State(":c",
					 new Prioritize(
						 new Charge(1, range: 4, coolDown: 4000),
						 new Wander(5),
						 new StayAbove(0.45, 4)
					 ),
					 new Shoot(10, 2, 20, angleOffset: 0 / 2, index: 0, coolDown: 300),
					 new Shoot(10, 1, 0, defaultAngle: 180, angleOffset: 180, index: 0, aim: 1,
					 coolDown: 300, coolDownOffset: 0),
					 new TimedTransition(7000, ":P")
					 ),
				 new State(":P",
					 new ReturnToSpawn(speed: 1.4),
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new TimedTransition(1000, "rage")
					 ),
				 new State("rage",
					 new TossObject("Limon Element 1", 6, 45, 100000),
					 new TossObject("Limon Element 2", 6, 135, 100000),
					 new TossObject("Limon Element 3", 6, 225, 100000),
					 new TossObject("Limon Element 4", 6, 315, 100000),
					 new TossObject("Limon Element 1", 10, 45, 100000),
					 new TossObject("Limon Element 2", 10, 135, 100000),
					 new TossObject("Limon Element 3", 10, 225, 100000),
					 new TossObject("Limon Element 4", 10, 315, 100000),
					 new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(2000, "rage2")
					 ),
				 new State("rage2",
					 new RemCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(3000, "rage3")
					 ),
				 new State("rage3",
					  new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(3000, "rage4")
					 ),
				 new State("rage4",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new EntityOrder(100, "Limon Element 1", "X Shape"),
					 new EntityOrder(100, "Limon Element 2", "X Shape"),
					 new EntityOrder(100, "Limon Element 3", "X Shape"),
					 new EntityOrder(100, "Limon Element 4", "X Shape"),
					 new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(3000, "rage5")
					 ),
				 new State("rage5",
					 new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(3000, "rage6")
					 ),
				 new State("rage6",
					 new RemCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(3000, "rage7")
					 ),
				 new State("rage7",
					 new Shoot(10, 1, 0, defaultAngle: 0, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 120, angleOffset: 120, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new Shoot(10, 1, 0, defaultAngle: 240, angleOffset: 240, index: 0, aim: 1,
					 coolDown: 800, coolDownOffset: 0),
					 new TimedTransition(3000, "brown shield")
					 ),
				 new State("brown shield",
					 new Wander(0.07),
					 new AddCond(ConditionEffectIndex.Armored),
					 new Spawn("Native Magic Sprite", maxChildren: 2, initialSpawn: 0.5),
					 new Spawn("Native Ice Sprite", maxChildren: 2, initialSpawn: 0.5),
					 new EntityOrder(100, "Limon Element 1", "suicide"),
					 new EntityOrder(100, "Limon Element 2", "suicide"),
					 new EntityOrder(100, "Limon Element 3", "suicide"),
					 new EntityOrder(100, "Limon Element 4", "suicide"),
					 new Shoot(10, 2, 20, angleOffset: 0 / 2, index: 0, coolDown: 300),
					 new Shoot(10, 1, 0, defaultAngle: 180, angleOffset: 0, index: 0, aim: 1,
					 coolDown: 300, coolDownOffset: 0),
					 new TimedTransition(2000, "nothing")
					 )
				 ),
			 new MostDamagers(3,
				 new ItemLoot("Potion of Dexterity", 1.00)
				 ),
				new Threshold(0.1,
				new ItemLoot("Potion of Defense", 0.3),
				new ItemLoot("Sprite Wand", 0.3),
				new ItemLoot("Cloak of the Planewalker", 0.005),
				new ItemLoot("Staff of Extreme Prejudice", 0.005),
				new ItemLoot("Wine Cellar Incantation", 0.005),
					new TierLoot(3, ItemType.Ring, 0.2),

					new TierLoot(6, ItemType.Armor, 0.2),

					new TierLoot(3, ItemType.Ability, 0.2),
					new TierLoot(4, ItemType.Ability, 0.15),
					new TierLoot(5, ItemType.Ability, 0.1)
			)
			)
		.Init("Limon Element 1",
			 new State(
				 new EntityNotExistsTransition("Limon the Sprite God", 1000, "suicide"),
				 new State("Shoot",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 270, angleOffset: 270, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 180, angleOffset: 180, coolDown: 200)
					 ),
				 new State("X Shape",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 270, angleOffset: 270, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 180, angleOffset: 180, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 225, angleOffset: 225, coolDown: 200)
					 ),
				 new State("suicide",
					 new Suicide()
			   )
				 )
			)
	   .Init("Limon Element 2",
			 new State(
				 new EntityNotExistsTransition("Limon the Sprite God", 1000, "suicide"),
				 new State("Shoot",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 360, angleOffset: 360, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 270, angleOffset: 270, coolDown: 200)
					 ),
				 new State("X Shape",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 270, angleOffset: 270, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 360, angleOffset: 360, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 315, angleOffset: 315, coolDown: 200)
					 ),
				 new State("suicide",
					 new Suicide()
			   )
				 )
			)
		.Init("Limon Element 3",
			 new State(
				 new EntityNotExistsTransition("Limon the Sprite God", 1000, "suicide"),
				 new State("Shoot",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 90, angleOffset: 90, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 360, angleOffset: 360, coolDown: 200)
					 ),
				 new State("X Shape",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 90, angleOffset: 90, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 360, angleOffset: 360, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 45, angleOffset: 45, coolDown: 200)
					 ),
				 new State("suicide",
					 new Suicide()
			   )
				 )
			)
				.Init("Limon Element 4",
			 new State(
				 new EntityNotExistsTransition("Limon the Sprite God", 1000, "suicide"),
				 new State("Shoot",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 90, angleOffset: 90, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 180, angleOffset: 180, coolDown: 200)
					 ),
				 new State("X Shape",
					 new AddCond(ConditionEffectIndex.Invulnerable),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 90, angleOffset: 90, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 180, angleOffset: 180, coolDown: 200),
					 new Shoot(0, index: 0, shoots: 1, shootAngle: 135, angleOffset: 135, coolDown: 200)
					 ),
				 new State("suicide",
					 new Suicide()
			   )
				 )
			)
			.Init("Native Fire Sprite",
                new State(
                    new StayAbove(speed: 5, altitude: 95),
                    new Shoot(range: 10, shoots: 2, shootAngle: 7, index: 0, coolDown: 300),
                    new Wander(speed: 14)
                )
            )

            .Init("Native Ice Sprite",
                new State(
                    new StayAbove(speed: 5, altitude: 105),
                    new Shoot(range: 10, shoots: 3, shootAngle: 7, index: 0, coolDown: 1000),
                    new Wander(speed: 14)
                )
            )

            .Init("Native Magic Sprite",
                new State(
                    new StayAbove(speed: 5, altitude: 115),
                    new Shoot(range: 10, shoots: 4, shootAngle: 7, index: 0, coolDown: 1000),
                    new Wander(speed: 14)
                )
            )

            .Init("Native Nature Sprite",
                new State(
                    new Shoot(range: 10, shoots: 5, shootAngle: 7, index: 0, coolDown: 1000),
                    new Wander(speed: 16)
                )
            )

            .Init("Native Darkness Sprite",
                new State(
                    new Shoot(range: 10, shoots: 5, shootAngle: 7, index: 0, coolDown: 1000),
                    new Wander(speed: 16)
                )
            )

            .Init("Native Sprite God",
                new State(
                    new StayAbove(speed: 5, altitude: 200),
                    new Shoot(range: 12, shoots: 4, shootAngle: 10, index: 0, coolDown: 1000),
                    new Shoot(range: 10, index: 1, aim: 1, coolDown: 1000),
                    new Wander(speed: 4)
                )
            )
        ;
    }
}