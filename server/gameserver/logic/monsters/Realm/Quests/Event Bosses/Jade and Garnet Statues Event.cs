using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesJadeandGarnetStatuesEvent = () => Behav()
            .Init("Garnet Statue",
                    new State(
                       new State("Wait",
                       new AddCond(ConditionEffectIndex.Invincible)
                    ),
                    new State("Activate",
                       new TimedTransition(20, "RemINVINC")
                    ),
                    new State("RemINVINC",
                       new Flashing(0xffffff, 2, 100),
                       new RemCond(ConditionEffectIndex.Invincible),
                       new TimedTransition(2000, "Shotgun")
                    ),
                    new State("FlashingRING",
                       new Flashing(0xd40000, 2, 100),
                       new TimedTransition(2000, "RingCharge"),
                       new EntitiesNotExistsTransition(50, "JadeDied", "Jade Statue")
                    ),
                    new State("RingCharge",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(18, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 5),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 220),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 420), //smonk :3
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 620),
                       new TimedTransition(800, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "JadeDied", "Jade Statue")
                    ),
                    new State("Shotgun",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 5, aim: 1, shootAngle: 5, coolDown: 500, index: 0),
                       //new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 1000, index: 4),
                       new TimedTransition(3700, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "JadeDied", "Jade Statue")
                    ),
                    new State("Singular",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 1, aim: 1, coolDown: 90, index: 0),
                       new TimedTransition(1800, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "JadeDied", "Jade Statue")
                    ),
                    new State("PetRing",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 400, index: 4),
                       new TimedTransition(120, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "JadeDied", "Jade Statue")
                    ),
                    new State("Spawn",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new TossObject("Corrupted Spirit", 5, coolDown: 8000, randomToss: false),
                       new TossObject("Corrupted Spirit", 5, coolDown: 8000, randomToss: false),
                       new TimedTransition(700, "ChooseRandom")
                    ),
                    new State("ChooseRandom",
                       new Flashing(0xffffff, 2, 100),
                       new AddCond(ConditionEffectIndex.Invulnerable), // ok
                       new TimedTransition(1200, "Singular", true),
                       new TimedTransition(1200, "FlashingRING", true),
                       new TimedTransition(1200, "Shotgun", true),
                       new TimedTransition(1200, "PetRing", true),
                       new TimedTransition(1200, "Spawn", true),
                       new EntitiesNotExistsTransition(50, "JadeDied", "Jade Statue")
                    ),
                    new State("JadeDied",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                       new SpecificHeal(range: 10, amount: 56250, group: "Self", coolDown: 99999),
                       new ChangeSize(20, 130),
                       new Flashing(0xffffff, 2, 100),
                       new TimedTransition(1500, "ChooseRandomV2")
                    ),
                    new State("ChooseRandomV2",
                       new Flashing(0xffffff, 2, 100),
                       new AddCond(ConditionEffectIndex.Invulnerable), // ok
                       new TimedTransition(1200, "SingularV2", true),
                       new TimedTransition(1200, "FlashingRINGV2", true),
                       new TimedTransition(1200, "ShotgunV2", true),
                       new TimedTransition(1200, "PetRingV2", true),
                       new TimedTransition(1200, "ShotgunWIDER", true),
                       new TimedTransition(1200, "Swirl", true),
                       new TimedTransition(1200, "SpawnV2", true)
                    ),
                    new State("SpawnV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new TossObject("Corrupted Spirit", 5, coolDown: 8000, randomToss: false),
                       new TossObject("Corrupted Spirit", 5, coolDown: 8000, randomToss: false),
                       new TimedTransition(700, "ChooseRandomV2")
                    ),
                    new State("FlashingRINGV2",
                       new Flashing(0xd40000, 2, 100),
                       new TimedTransition(2000, "RingChargeV2")
                    ),
                    new State("RingChargeV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(18, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 5),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 220),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 420), //smonk :3
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 620),
                       new TimedTransition(800, "ChooseRandomV2")
                    ),
                    new State("ShotgunV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 5, aim: 1, shootAngle: 5, coolDown: 500, index: 0),
                       //new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 580, index: 3),
                       new TimedTransition(2300, "ChooseRandomV2")
                    ),
                    new State("ShotgunWIDER",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 5, aim: 1, shootAngle: 5, coolDown: 99999, index: 0, coolDownOffset: 50),
                       new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 99999, index: 3, coolDownOffset: 50),
                       //
                       new Shoot(10, shoots: 10, aim: 1, shootAngle: 5, coolDown: 99999, index: 0, coolDownOffset: 500),
                       new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 99999, index: 3, coolDownOffset: 500),
                       //
                       new Shoot(10, shoots: 15, aim: 1, shootAngle: 5, coolDown: 99999, index: 0, coolDownOffset: 1000),
                       new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 99999, index: 3, coolDownOffset: 1000),
                       new TimedTransition(1200, "ChooseRandomV2")
                    ),
                    new State("SingularV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 1, aim: 1, coolDown: 90, index: 0),
                       new TimedTransition(2500, "ChooseRandomV2")
                    ),
                    new State("PetRingV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 20, shootAngle: 18, defaultAngle: 0, coolDown: 9400, index: 4),
                       new Shoot(10, shoots: 20, shootAngle: 18, defaultAngle: 10, coolDown: 9400, index: 4, coolDownOffset: 100),
                       new Shoot(10, shoots: 20, shootAngle: 18, defaultAngle: 20, coolDown: 9400, index: 4, coolDownOffset: 300),
                       new TimedTransition(120, "ChooseRandomV2")
                    ),
                    new State("Swirl",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 0, index: 4, coolDownOffset: 50),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 90, index: 4, coolDownOffset: 200),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 180, index: 4, coolDownOffset: 400),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 270, index: 4, coolDownOffset: 600),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 45, index: 4, coolDownOffset: 800),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 135, index: 4, coolDownOffset: 1000),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 225, index: 4, coolDownOffset: 1200),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 315, index: 4, coolDownOffset: 1400),
                       new TimedTransition(1500, "ChooseRandomV2")
                    )
                ),
                new Drops(
                    new MostDamagers(5, LootTemplates.StatIncreasePotionsLoot()),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12)
                        ),
                    new CyanBag(ItemType.Ability, 5),
                    new CyanBag(ItemType.Ring, 5),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK, true),
                        new BlueBag(Potions.POTION_OF_DEFENSE, true),
                        new BlueBag(Potions.POTION_OF_SPEED, true),
                        new BlueBag(Potions.POTION_OF_DEXTERITY, true),
                        new BlueBag(Potions.POTION_OF_VITALITY, true),
                        new BlueBag(Potions.POTION_OF_WISDOM, true)
                        ),
                    new WhiteBag(new[] { "Wand of the Fallen", "Kageboshi" })
                    )
            )

            .Init("Jade Statue",
                    new State(
                       new State("Wait",
                       new AddCond(ConditionEffectIndex.Invincible)
                    ),
                    new State("Activate",
                       new TimedTransition(20, "RemINVINC")
                    ),
                    new State("RemINVINC",
                       new Flashing(0xffffff, 2, 100),
                       new RemCond(ConditionEffectIndex.Invincible), // ok
                       new TimedTransition(2000, "Shotgun")
                    ),
                    new State("FlashingRING",
                       new Flashing(0xd40000, 2, 100),
                       new TimedTransition(2000, "RingCharge"),
                       new EntitiesNotExistsTransition(50, "GarnetDied", "Garnet Statue")
                    ),
                    new State("RingCharge",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(1.8, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 5),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 220),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 420), //smonk :3
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 620),
                       new TimedTransition(800, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "GarnetDied", "Garnet Statue")
                    ),
                    new State("Shotgun",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 5, aim: 1, shootAngle: 5, coolDown: 500, index: 0),
                       //new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 1000, index: 4),
                       new TimedTransition(3700, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "GarnetDied", "Garnet Statue")
                    ),
                    new State("Singular",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 1, aim: 1, coolDown: 90, index: 0),
                       new TimedTransition(1800, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "GarnetDied", "Garnet Statue")
                    ),
                    new State("PetRing",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 400, index: 4),
                       new TimedTransition(120, "ChooseRandom"),
                       new EntitiesNotExistsTransition(50, "GarnetDied", "Garnet Statue")
                    ),
                    new State("Spawn",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new TossObject("Corrupted Sprite", 5, coolDown: 8000, randomToss: false),
                       new TossObject("Corrupted Sprite", 5, coolDown: 8000, randomToss: false),
                       new TimedTransition(700, "ChooseRandom")
                    ),
                    new State("ChooseRandom",
                       new Flashing(0xffffff, 2, 100),
                       new AddCond(ConditionEffectIndex.Invulnerable), // ok
                       new TimedTransition(1200, "Singular", true),
                       new TimedTransition(1200, "FlashingRING", true),
                       new TimedTransition(1200, "Shotgun", true),
                       new TimedTransition(1200, "PetRing", true),
                       new TimedTransition(1200, "Spawn", true),
                       new EntitiesNotExistsTransition(50, "GarnetDied", "Garnet Statue")
                    ),
                    new State("GarnetDied",
                       new AddCond(ConditionEffectIndex.Invulnerable), // ok
                       new SpecificHeal(range: 10, amount: 56250, group: "Self", coolDown: 99999),
                       new ChangeSize(20, 130),
                       new Flashing(0xffffff, 2, 100),
                       new TimedTransition(1500, "ChooseRandomV2")
                    ),
                    new State("ChooseRandomV2",
                       new Flashing(0xffffff, 2, 100),
                       new AddCond(ConditionEffectIndex.Invulnerable), // ok
                       new TimedTransition(1200, "SingularV2", true),
                       new TimedTransition(1200, "FlashingRINGV2", true),
                       new TimedTransition(1200, "ShotgunV2", true),
                       new TimedTransition(1200, "PetRingV2", true),
                       new TimedTransition(1200, "ShotgunWIDER", true),
                       new TimedTransition(1200, "Swirl", true),
                       new TimedTransition(1200, "SpawnV2", true)
                    ),
                    new State("SpawnV2",
                       new TossObject("Corrupted Sprite", 5, coolDown: 8000, randomToss: false),
                       new TossObject("Corrupted Sprite", 5, coolDown: 8000, randomToss: false),
                       new TimedTransition(700, "ChooseRandomV2")
                    ),
                    new State("FlashingRINGV2",
                       new Flashing(0xd40000, 2, 100),
                       new TimedTransition(2000, "RingChargeV2")
                    ),
                    new State("RingChargeV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(18, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 5),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 220),
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 420), //smonk :3
                       new Shoot(10, shoots: 20, shootAngle: 18, coolDown: 99999, index: 1, coolDownOffset: 620),
                       new TimedTransition(800, "ChooseRandomV2")
                    ),
                    new State("ShotgunV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 5, aim: 1, shootAngle: 5, coolDown: 500, index: 0),
                       //new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 580, index: 3),
                       new TimedTransition(2300, "ChooseRandomV2")
                    ),
                    new State("ShotgunWIDER",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 5, aim: 1, shootAngle: 5, coolDown: 99999, index: 0, coolDownOffset: 50),
                       new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 99999, index: 3, coolDownOffset: 50),
                       //
                       new Shoot(10, shoots: 10, aim: 1, shootAngle: 5, coolDown: 99999, index: 0, coolDownOffset: 500),
                       new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 99999, index: 3, coolDownOffset: 500),
                       //
                       new Shoot(10, shoots: 15, aim: 1, shootAngle: 5, coolDown: 99999, index: 0, coolDownOffset: 1000),
                       new Shoot(10, shoots: 1, aim: 1, shootAngle: 5, coolDown: 99999, index: 3, coolDownOffset: 1000),
                       new TimedTransition(1200, "ChooseRandomV2")
                    ),
                    new State("SingularV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Chase(7, range: 1, duration: 5000, coolDown: 0),
                       new Shoot(10, shoots: 1, aim: 1, coolDown: 90, index: 0),
                       new TimedTransition(2500, "ChooseRandomV2")
                    ),
                    new State("PetRingV2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 20, shootAngle: 18, defaultAngle: 0, coolDown: 9400, index: 4),
                       new Shoot(10, shoots: 20, shootAngle: 18, defaultAngle: 10, coolDown: 9400, index: 4, coolDownOffset: 100),
                       new Shoot(10, shoots: 20, shootAngle: 18, defaultAngle: 20, coolDown: 9400, index: 4, coolDownOffset: 300),
                       new TimedTransition(120, "ChooseRandomV2")
                    ),
                    new State("Swirl",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 0, index: 4, coolDownOffset: 50),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 90, index: 4, coolDownOffset: 200),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 180, index: 4, coolDownOffset: 400),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 270, index: 4, coolDownOffset: 600),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 45, index: 4, coolDownOffset: 800),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 135, index: 4, coolDownOffset: 1000),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 225, index: 4, coolDownOffset: 1200),
                       new Shoot(10, shoots: 2, shootAngle: 20, coolDown: 99999, defaultAngle: 315, index: 4, coolDownOffset: 1400),
                       new TimedTransition(1500, "ChooseRandomV2")
                    )
                ),
                new Drops(
                    new MostDamagers(5, LootTemplates.StatIncreasePotionsLoot()),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12)
                        ),
                    new CyanBag(ItemType.Ability, 5),
                    new CyanBag(ItemType.Ring, 5),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK, true),
                        new BlueBag(Potions.POTION_OF_DEFENSE, true),
                        new BlueBag(Potions.POTION_OF_SPEED, true),
                        new BlueBag(Potions.POTION_OF_DEXTERITY, true),
                        new BlueBag(Potions.POTION_OF_VITALITY, true),
                        new BlueBag(Potions.POTION_OF_WISDOM, true)
                        ),
                    new WhiteBag(new[] { "Wand of the Fallen", "Kageboshi" })
                    )
            )

            .Init("Corrupted Sprite",
                new State(
                    new State("One",
                        new Chase(4, range: 1, duration: 5000, coolDown: 0),
                        // 1
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999),
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999, coolDownOffset: 400),
                        // 2
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 1200),
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 1600),
                        // 8
                        new Shoot(10, shoots: 8, aim: 1, shootAngle: 30, index: 0, coolDown: 99999, coolDownOffset: 2300),
                        new Shoot(10, shoots: 8, aim: 1, shootAngle: 30, index: 0, coolDown: 99999, coolDownOffset: 2800),
                        // 2
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 3200),
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 3500),
                        // 1
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999, coolDownOffset: 4000),
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999, coolDownOffset: 4400),
                        new TimedTransition(10000, "KYS")
                        ),
                    new State("KYS",
                        new Suicide()
                        )
                )
            )

            .Init("Corrupted Spirit",
                new State(
                    new State("One",
                        new Chase(4, range: 1, duration: 5000, coolDown: 0),
                        // 1
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999),
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999, coolDownOffset: 400),
                        // 2
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 1200),
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 1600),
                        // 8
                        new Shoot(10, shoots: 8, aim: 1, shootAngle: 30, index: 0, coolDown: 99999, coolDownOffset: 2300),
                        new Shoot(10, shoots: 8, aim: 1, shootAngle: 30, index: 0, coolDown: 99999, coolDownOffset: 2800),
                        // 2
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 3200),
                        new Shoot(10, shoots: 2, aim: 1, shootAngle: 25, index: 0, coolDown: 99999, coolDownOffset: 3500),
                        // 1
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999, coolDownOffset: 4000),
                        new Shoot(10, shoots: 1, aim: 1, index: 0, coolDown: 99999, coolDownOffset: 4400),
                        new TimedTransition(10000, "KYS")
                        ),
                    new State("KYS",
                        new Suicide()
                        )
                )
            )

            .Init("Encounter Altar",
                new State(
                    //new DropPortalOnDeath("Mountain Temple Portal", 0.4),
                    new State("Wait",
                        new AddCond(ConditionEffectIndex.Invincible)
                        ),
                    new State("toss",
                        new TossObject("Garnet Statue", 5, 0, int.MaxValue - 1, 0, false, true),
                        new TossObject("Jade Statue", 5, 180, int.MaxValue - 1, 0, false, true),
                        new PlayerWithinTransition(24, "ActivateG&J")
                        ),
                    new State("ActivateG&J",
                        new EntityOrder(100, "Garnet Statue", "Activate"),
                        new EntityOrder(100, "Jade Statue", "Activate"),
                        new TimedTransition(170, "IsG&JDead?")
                        ),
                    new State("IsG&JDead?",
                        new EntitiesNotExistsTransition(50, "DropPortal", "Garnet Statue", "Jade Statue")
                        ),
                    new State("DropPortal",
                        new Suicide()
                        )
                  )
                )
        ;
    }
}