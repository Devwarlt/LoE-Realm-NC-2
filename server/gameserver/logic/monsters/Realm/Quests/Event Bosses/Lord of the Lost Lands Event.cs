using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;
using System;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesLordoftheLostLandsEvent = () => Behav()
            .Init("Lord of the Lost Lands",
                new State(
                    //new DropPortalOnDeath("Ice Cave Portal", 0.5),
                    new State("Waiting",
                        new HpLessTransition(0.99, "Start1.0")
                        ),
                    new State("Start1.0",
                        new RemCond(ConditionEffectIndex.Invulnerable),
                        new HpLessTransition(0.1, "Dead"),
                        new State("Start",
                            new SetAltTexture(0),
                            new Prioritize(
                                new Wander(8)
                                ),
                            new Shoot(0, shoots: 7, shootAngle: 10, direction: 180, coolDown: 2000),
                            new Shoot(0, shoots: 7, shootAngle: 10, direction: 0, coolDown: 2000),
                            new TossObject("Guardian of the Lost Lands", 5, coolDown: 1000),
                            new TimedTransition(100, "Spawning Guardian")
                            ),
                        new State("Spawning Guardian",
                            new TossObject("Guardian of the Lost Lands", 5, coolDown: 1000),
                            new TimedTransition(3100, "Attack")
                            ),
                        new State("Attack",
                            new SetAltTexture(0),
                            new Wander(8),
                            new PlayerWithinTransition(1, "Follow"),
                            new TimedTransition(10000, "Gathering"),
                            new State("Attack1.0",
                                new TimedTransition(10, ((new Random()).NextDouble() > .5 ? "Attack1.1" : "Attack1.2"), false),
                                new State("Attack1.1",
                                    new Shoot(12, shoots: 7, shootAngle: 10, coolDown: 2000),
                                    new Shoot(12, shoots: 7, shootAngle: 190, coolDown: 2000),
                                    new TimedTransition(2000, "Attack1.0")
                                    ),
                                new State("Attack1.2",
                                    new Shoot(0, shoots: 7, shootAngle: 10, direction: 180, coolDown: 3000),
                                    new Shoot(0, shoots: 7, shootAngle: 10, direction: 0, coolDown: 3000),
                                    new TimedTransition(2000, "Attack1.0")
                                    )
                                )
                            ),
                        new State("Follow",
                            new Prioritize(
                                new Chase(10, 20, 3),
                                new Wander()
                                ),
                            new Shoot(20, shoots: 7, shootAngle: 10, coolDown: 1300),
                            new TimedTransition(5000, "Gathering")
                            ),
                        new State("Gathering",
                            new Taunt(0.99, "Gathering power!"),
                            new SetAltTexture(3),
                            new TimedTransition(2000, "Gathering1.0")
                            ),
                        new State("Gathering1.0",
                            new TimedTransition(5000, "Protection"),
                            new State("Gathering1.1",
                                new Shoot(30, 4, direction: 90, index: 1, coolDown: 2000),
                                new TimedTransition(1500, "Gathering1.2")
                                ),
                            new State("Gathering1.2",
                                new Shoot(30, 4, direction: 45, index: 1, coolDown: 2000),
                                new TimedTransition(1500, "Gathering1.1")
                                )
                            ),
                        new State("Protection",
                            new AddCond(ConditionEffectIndex.StunImmune),
                            new SetAltTexture(0),
                            new TossObject("Protection Crystal", 4, angle: 0, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 45, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 90, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 135, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 180, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 225, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 270, coolDown: 5000),
                            new TossObject("Protection Crystal", 4, angle: 315, coolDown: 5000),
                            new EntityExistsTransition("Protection Crystal", 10, "Waiting")
                            )
                        ),
                    new State("Waiting",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new RemCond(ConditionEffectIndex.StunImmune),
                        new SetAltTexture(1),
                        new EntityNotExistsTransition("Protection Crystal", 10, "Start1.0")
                        ),
                    new State("Dead",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new SetAltTexture(3),
                        new Taunt(0.99, "NOOOO!!!!!!"),
                        new Flashing(0xFF0000, .1, 1000),
                        new TimedTransition(2000, "Suicide")
                        ),
                    new State("Suicide",
                        new AddCond(ConditionEffectIndex.StunImmune),
                        new Shoot(0, 8, direction: 360 / 8, index: 1),
                        new Suicide()
                        )
                    ),
                new Drops(
                    new MostDamagers(3, new BlueBag(Potions.POTION_OF_MANA, true)),
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 8),
                        new PurpleBag(ItemType.Weapon, 9)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11),
                        new CyanBag(ItemType.Weapon, 12)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12),
                        new CyanBag(ItemType.Armor, 13)
                        ),
                    new CyanBag(ItemType.Ability, 5),
                    new CyanBag(ItemType.Ring, 5),
                    new OnlyOne(
                        new PurpleBag(ItemType.Armor, 8),
                        new PurpleBag(ItemType.Armor, 9)
                        ),
                    new PurpleBag(ItemType.Ability, 4),
                    new OnlyOne(
                        new CyanBag(ItemType.Ability, 5),
                        new CyanBag(ItemType.Ability, 6)
                        ),
                    new OnlyOne(
                        new PurpleBag(ItemType.Ring, 3),
                        new PurpleBag(ItemType.Ring, 4)
                        ),
                    new CyanBag(ItemType.Ring, 5),
                    new EggBasket(new[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4, EggType.TIER_5, EggType.TIER_6, EggType.TIER_7 }),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK, true),
                        new BlueBag(Potions.POTION_OF_DEFENSE, true),
                        new BlueBag(Potions.POTION_OF_SPEED, true),
                        new BlueBag(Potions.POTION_OF_DEXTERITY, true),
                        new BlueBag(Potions.POTION_OF_VITALITY, true),
                        new BlueBag(Potions.POTION_OF_WISDOM, true)
                        ),
                    new WhiteBag(new[] { "Shield of Ogmur", "Staff of Esben", "Skullish Remains of Esben" })
                    )
            )

            .Init("Protection Crystal",
                new State(
                    new Prioritize(
                        new Circle(3, 4, 10, "Lord of the Lost Lands")
                        ),
                    new Shoot(8, shoots: 4, shootAngle: 7, coolDown: 500)
                    )
            )

            .Init("Guardian of the Lost Lands",
                new State(
                    new State("Full",
                        new Spawn("Knight of the Lost Lands", 2, 1, coolDown: 4000),
                        new Prioritize(
                            new Chase(6, 20, 6),
                            new Wander(2)
                            ),
                        new Shoot(10, shoots: 8, direction: 360 / 8, coolDown: 3000, index: 1),
                        new Shoot(10, shoots: 5, shootAngle: 10, coolDown: 1500),
                        new HpLessTransition(0.25, "Low")
                        ),
                    new State("Low",
                        new Prioritize(
                            new Retreat(6, 5),
                            new Wander(2)
                            ),
                        new Shoot(10, shoots: 8, direction: 360 / 8, coolDown: 3000, index: 1),
                        new Shoot(10, shoots: 5, shootAngle: 10, coolDown: 1500)
                        )
                    ),
                new ItemLoot("Health Potion", 0.1),
                new ItemLoot("Magic Potion", 0.1)
            )

            .Init("Knight of the Lost Lands",
                new State(
                    new Prioritize(
                        new Chase(10, 20, 4),
                        new Retreat(5, 2),
                        new Wander(3)
                        ),
                    new Shoot(13, 1, coolDown: 700)
                    ),
                new ItemLoot("Health Potion", 0.1),
                new ItemLoot("Magic Potion", 0.1)
            )
        ;
    }
}