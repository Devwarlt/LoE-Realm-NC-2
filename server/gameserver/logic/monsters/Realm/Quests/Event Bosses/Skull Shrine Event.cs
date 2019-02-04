using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesSkullShrineEvent = () => Behav()
            .Init("Skull Shrine",
                new State(
                    new State("initial",
                        new Shoot(30, 9, 10, coolDown: 750, aim: 1),
                        new Reproduce("Red Flaming Skull", 40, 20, coolDown: 500),
                        new Reproduce("Blue Flaming Skull", 40, 20, coolDown: 500),
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
                        new Shoot(30, 9, 10, coolDown: 750, aim: 1),
                        new Reproduce("Red Flaming Skull", 40, 20, coolDown: 5000),
                        new Reproduce("Blue Flaming Skull", 40, 20, coolDown: 5000),
                        new Flashing(0xFF0000, 0.5, int.MaxValue / 2)
                        )
                    ),
                new Drops(
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 8),
                        new PurpleBag(ItemType.Weapon, 9)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11)
                        ),
                    new PurpleBag(ItemType.Ability, 4),
                    new CyanBag(ItemType.Ability, 5),
                    new OnlyOne(
                        new PurpleBag(ItemType.Armor, 7),
                        new PurpleBag(ItemType.Armor, 8),
                        new PurpleBag(ItemType.Armor, 9)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12)
                        ),
                    new CyanBag(ItemType.Ring, 5),
                    new OnlyOne(
                        new PurpleBag(ItemType.Ring, 3),
                        new PurpleBag(ItemType.Ring, 4)
                        ),
                    new EggBasket(new[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK, true),
                        new BlueBag(Potions.POTION_OF_DEFENSE, true),
                        new BlueBag(Potions.POTION_OF_SPEED, true),
                        new BlueBag(Potions.POTION_OF_DEXTERITY, true),
                        new BlueBag(Potions.POTION_OF_VITALITY, true),
                        new BlueBag(Potions.POTION_OF_WISDOM, true)
                        ),
                    new WhiteBag("Orb of Conflict")
                    )
            )

            .Init("Red Flaming Skull",
                new State(
                    new State("Orbit Skull Shrine",
                        new Prioritize(
                            new Protect(3, "Skull Shrine", 30, 15, 15),
                            new Wander(3)
                            ),
                        new EntityNotExistsTransition("Skull Shrine", 40, "Wander")
                        ),
                    new State("Wander",
                        new Wander(3)
                        ),
                        new Shoot(12, 2, 10, coolDown: 750)
                    )
            )

            .Init("Blue Flaming Skull",
                new State(
                    new State("Orbit Skull Shrine",
                        new Circle(15, 15, 40, "Skull Shrine", .6, 10, orbitClockwise: null),
                        new EntityNotExistsTransition("Skull Shrine", 40, "Wander")
                        ),
                    new State("Wander",
                        new Wander(15)
                        ),
                    new Shoot(12, 2, 10, coolDown: 750)
                    )
            )
        ;
    }
}