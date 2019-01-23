using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ Frog = () => Behav()
        .Init("Frog King",
            new State(
                new State("Pause",
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new PlayerWithinTransition(12, "Start")
                    ),
                new State("Start",
                    new Taunt("Who dares come and challenge the Frog King?"),
                    new TimedTransition(2000, "Fight")
                    ),
                new State("Fight",
                    new Taunt("If Filisha cannot defeat me, you nibbas definitely cannot beat me."),
                    new RemCond(ConditionEffectIndex.Invulnerable), // ok
                    new ManaDrainBomb(6, 50, 6, coolDown: 2000),
                    new HpLessTransition(0.97, "Fight2")
                    ),
                new State("Fight2",
                    new Prioritize(
                        new Chase(speed: 4),
                        new Wander(speed: 1)
                        ),
                    new Taunt("REEEEEEEEE!"),
                    new Heal(range: 0, amount: 2000, coolDown: 7500),
                    new TossObject("Frog Minion", 5, coolDown: 1000),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 0, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 100, coolDownOffset: 200, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 110, coolDownOffset: 400, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 120, coolDownOffset: 600, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 130, coolDownOffset: 800, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 140, coolDownOffset: 1000, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 150, coolDownOffset: 1200, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 160, coolDownOffset: 1400, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 170, coolDownOffset: 1600, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 180, coolDownOffset: 1800, shootAngle: 90),
                    new Shoot(1, shoots: 8, coolDown: 10000, direction: 180, coolDownOffset: 2000, shootAngle: 45),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 180, coolDownOffset: 0, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 170, coolDownOffset: 200, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 160, coolDownOffset: 400, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 150, coolDownOffset: 600, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 140, coolDownOffset: 800, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 130, coolDownOffset: 1000, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 120, coolDownOffset: 1200, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 110, coolDownOffset: 1400, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 100, coolDownOffset: 1600, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 1800, shootAngle: 90),
                    new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 2000, shootAngle: 22.5),
                    new TimedTransition(2000, "Wait")
                    ),
                new State("Wait",
                    new Taunt("Begone Thot"),
                    new Chase(7, range: 0.5),
                    new Shoot(10, shoots: 3, shootAngle: 20, aim: 1, coolDown: 500),
                    new Shoot(20, shoots: 4, shootAngle: 25, aim: 0.4, coolDown: 900),
                    new Flashing(0xff00ff00, 0.1, 20),
                    new HpLessTransition(0.7, "Nightmare")
                    ),
                new State("Nightmare",
                    new Taunt("I am STRONGER than ORYX you dumbass,you cannot beat me!"),
                    new Flashing(0xff00ff00, 0.2, 30),
                    new TossObject("Frog Minion", 5, coolDown: 1000),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 0, angleOffset: 0, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 1, angleOffset: 5 * 1, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 2, angleOffset: 5 * 2, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 3, angleOffset: 5 * 3, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 4, angleOffset: 5 * 4, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 5, angleOffset: 5 * 5, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 6, angleOffset: 5 * 6, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 7, angleOffset: 5 * 7, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 8, angleOffset: 5 * 8, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 9, angleOffset: 5 * 9, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 10, angleOffset: 5 * 10, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 11, angleOffset: 5 * 11, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 12, angleOffset: 5 * 12, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 13, angleOffset: 5 * 13, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 14, angleOffset: 5 * 14, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 15, angleOffset: 5 * 15, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 16, angleOffset: 5 * 16, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 17, angleOffset: 5 * 17, shootAngle: 60),
                    new Shoot(50, index: 1, shoots: 6, coolDown: 200 * 20, coolDownOffset: 200 * 18, angleOffset: 5 * 18, shootAngle: 60),
                    new HpLessTransition(0.5, "Nightmare2")
                    ),
                new State("Nightmare2",
                    new Taunt("I RULE LoE!"),
                    new TossObject("Frog Minion", 5, coolDown: 1000),
                    new Shoot(30, index: 1, shoots: 1, coolDown: 1000),
                    new Shoot(50, index: 3, shoots: 5, coolDown: 4000, coolDownOffset: 0, angleOffset: 0, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 7, coolDown: 4000, coolDownOffset: 400, angleOffset: 0, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 800, angleOffset: 0, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 1200, angleOffset: 0, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 1600, angleOffset: 0, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 5, coolDown: 4000, coolDownOffset: 0, angleOffset: 180, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 7, coolDown: 4000, coolDownOffset: 400, angleOffset: 180, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 800, angleOffset: 180, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 1200, angleOffset: 180, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 1600, angleOffset: 180, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 5, coolDown: 4000, coolDownOffset: 2000, angleOffset: 90, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 7, coolDown: 4000, coolDownOffset: 2400, angleOffset: 90, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 2800, angleOffset: 90, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 3200, angleOffset: 90, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 3600, angleOffset: 90, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 5, coolDown: 4000, coolDownOffset: 2000, angleOffset: 270, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 7, coolDown: 4000, coolDownOffset: 2400, angleOffset: 270, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 2800, angleOffset: 270, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 3200, angleOffset: 270, shootAngle: 9),
                    new Shoot(50, index: 3, shoots: 9, coolDown: 4000, coolDownOffset: 3600, angleOffset: 270, shootAngle: 9),
                    new HpLessTransition(0.4, "Waiting")
                    ),
                new State("Waiting",
                    new Flashing(0xFF0000, .1, 1000),
                    new RemCond(ConditionEffectIndex.Invulnerable), // ok
                    new Shoot(10, shoots: 3, shootAngle: 20, aim: 1, coolDown: 500),
                    new HpLessTransition(0.1, "Dead")
                    ),
                new State("Dead",
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new AddCond(ConditionEffectIndex.StunImmune),
                    new Taunt(0.99, "Oh looky I died.Oh well."),
                    new Flashing(0xFF0000, .1, 1000),
                    new TimedTransition(2000, "Suicide")
                    ),
                new State("Suicide",
                    new Shoot(0, 8, direction: 360 / 8, index: 1),
                    new Suicide()
                    )
                ),
            new WhiteBag("Frog King Skin"),
            new Drops(
                new OnlyOne(
				    new BlueBag(Potions.POTION_OF_LIFE),
					new BlueBag(Potions.POTION_OF_MANA),
					new BlueBag(Potions.POTION_OF_VITALITY),
					new BlueBag(Potions.POTION_OF_DEFENSE)
					),
                new OnlyOne(
                    new CyanBag(ItemType.Weapon, 10),
					new CyanBag(ItemType.Weapon, 11),
					new CyanBag(ItemType.Weapon, 12)
					),
                new OnlyOne(
				  new CyanBag(ItemType.Armor, 10),
				  new CyanBag(ItemType.Armor, 11), 
				  new CyanBag(ItemType.Armor, 12)

					),
                new OnlyOne(
					 new CyanBag(ItemType.Ability, 5),
					 new CyanBag(ItemType.Ability, 6)
					),
                new OnlyOne(
				     new CyanBag(ItemType.Ring, 5),
					 new CyanBag(ItemType.Ring, 6)
					)
                )
            )

        .Init("Frog Minion",
            new State(
                new State("Shooting1",
                    new Wander(2),
                    new AddCond(ConditionEffectIndex.Armored),
                    new EntityNotExistsTransition("Frog King", 50, "Shooting4"),
                    new Shoot(30, index: 0, shoots: 5, coolDown: 3000, coolDownOffset: 0, shootAngle: 72),
                    new Shoot(30, index: 1, shoots: 5, coolDown: 3000, coolDownOffset: 1500, shootAngle: 72),
                    new TimedTransition(5000, "Shooting2")
                    ),
                new State("Shooting2",
                    new Wander(2),
                    new Shoot(30, index: 1, shoots: 8, coolDown: 2000, coolDownOffset: 0, angleOffset: 0, shootAngle: 45),
                    new Shoot(30, index: 1, shoots: 8, coolDown: 2000, coolDownOffset: 1000, angleOffset: 22.5, shootAngle: 45),
                    new TimedTransition(5000, "Shooting3")
                    ),
                new State("Shooting3",
                    new Wander(2),
                    new Shoot(30, index: 0, shoots: 1, coolDown: 1000),
                    new TimedTransition(5000, "Shooting4")
                    ),
                new State("Shooting4",
                    new Wander(2),
                    new TossObject("Frog Bomb", 5, randomToss: true, ignoreStun: true, invisiToss: true),
                    new AddCond(ConditionEffectIndex.StunImmune),
                    new Shoot(0, 8, direction: 360 / 8, index: 1),
                    new Suicide()
                    )
                )
            )

        .Init("Frog Bomb",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new Flashing(0xff0000, 2, 1),
                    new ChangeSize(100, 80),
                    new TimedTransition(800, "Kaboom2")
                    ),
                new State("Kaboom2",
                    new Shoot(30, index: 0, shoots: 8, angleOffset: 0, shootAngle: 45),
                    new Decay(0)
                    )
                )
            )
        ;
    }
}