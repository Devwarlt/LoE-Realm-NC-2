using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesCubeGodEvent = () => Behav()
            .Init("Cube God",
                new State(
                    new State("initial",
                        new Wander(3),
                        new Shoot(30, 9, 10, 0, aim: .5, coolDown: 750),
                        new Shoot(30, 4, 10, 1, aim: .5, coolDown: 1500),
                        new Reproduce("Cube Overseer", 30, 10, coolDown: 1500),
                        new HpLessTransition(0.35, "flashing")
                        ),
                    new State("flashing",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xFF0000, 0.5, (int)(10 / 0.5)),
                        new TimedTransition(5000, "final")
                        ),
                    new State("final",
                        new AddCond(ConditionEffectIndex.StunImmune),
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Wander(3),
                        new Shoot(30, 9, 10, 0, aim: .15, coolDown: 500),
                        new Shoot(30, 4, 10, 1, aim: .15, coolDown: 750),
                        new Reproduce("Cube Overseer", 30, 10, coolDown: 1500),
                        new Flashing(0xFF0000, 0.5, int.MaxValue / 2)
                        )
                    ),
                new Drops(
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 8),
                        new PurpleBag(ItemType.Weapon, 9),
                        new PurpleBag(ItemType.Ability, 4),
                        new PurpleBag(ItemType.Armor, 8),
                        new PurpleBag(ItemType.Armor, 9),
                        new PurpleBag(ItemType.Ring, 3),
                        new PurpleBag(ItemType.Ring, 4)
                        ),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11),
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12),
                        new CyanBag(ItemType.Ability, 5),
                        new CyanBag(ItemType.Ring, 5)
                        ),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK, true),
                        new BlueBag(Potions.POTION_OF_DEFENSE, true),
                        new BlueBag(Potions.POTION_OF_SPEED, true),
                        new BlueBag(Potions.POTION_OF_DEXTERITY, true),
                        new BlueBag(Potions.POTION_OF_VITALITY, true),
                        new BlueBag(Potions.POTION_OF_WISDOM, true)
                        ),
                    new WhiteBag("Dirk of Cronus")
                    )
            )

            .Init("Cube Overseer",
                new State(
                    new Prioritize(
                        new Circle(3.75, 10, 30, "Cube God", .075, 5),
                        new Wander(3.75)
                        ),
                    new Reproduce("Cube Defender", 12, 5, coolDown: 1000),
                    new Reproduce("Cube Blaster", 30, 5, coolDown: 1000),
                    new Shoot(10, 4, 10, 0, coolDown: 750),
                    new Shoot(10, index: 1, coolDown: 1500)
                    ),
                new Threshold(.01,
                    new ItemLoot("Fire Sword", .05)
                    )
            )

            .Init("Cube Defender",
                new State(
                    new Prioritize(
                        new Circle(10.5, 5, 15, "Cube Overseer", .15, 3),
                        new Wander(10.5)
                        ),
                    new Shoot(10, coolDown: 500)
                    )
            )

            .Init("Cube Blaster",
                new State(
                    new State("Orbit",
                        new Prioritize(
                            new Circle(10.5, 7.5, 40, "Cube Overseer", .15, 3),
                            new Wander(10.5)
                            ),
                        new EntityNotExistsTransition("Cube Overseer", 10, "Follow")
                        ),
                    new State("Follow",
                        new Prioritize(
                            new Chase(7.5, 10, 1, 5000),
                            new Wander(10.5)
                            ),
                        new EntityNotExistsTransition("Cube Defender", 10, "Orbit"),
                        new TimedTransition(5000, "Orbit")
                        ),
                    new Shoot(10, 2, 10, 1, aim: 1, coolDown: 500),
                    new Shoot(10, aim: 1, coolDown: 1500)
                    )
            )
        ;
    }
}