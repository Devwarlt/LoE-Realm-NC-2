using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;
using System;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesGhostShipEvent = () => Behav()
            .Init("Ghost Ship",
                new State(
                    new State("Waiting Player",
                        new SetAltTexture(1),
                        new Prioritize(
                            new Circle(2, 2, 10, "Ghost Ship Anchor")
                            ),
                        new HpLessTransition(0.98, "Start")
                        ),
                    new State("Start",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ReturnToSpawn(true, 5),
                        new SetAltTexture(0),
                        new TimedTransition(3000, "PreAttack")
                        ),
                    new State("PreAttack",
                        new ReturnToSpawn(true, 5),
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new SetAltTexture(2),
                        new EntityOrder(100, "Beach Spectre Spawner", "Active"),
                        new TimedTransition(300, "Phase1+2")
                        ),
                    new State("Phase1+2",
                        new SetAltTexture(0),
                        new Reproduce("Vengeful Spirit", 20, 4, coolDown: 4000),
                        new TossObject("Water Mine", coolDown: 5000, range: (new Random()).Next(3, 7), angle: (new Random()).Next(1, 359)),
                        new State("Attack",
                            new Prioritize(
                                new Chase(2, 10, 3),
                                new Wander(3),
                                new StayCloseToSpawn(4, 7)
                                ),
                            new Taunt(0.30, "Fire at will!"),
                            new Shoot(10, shoots: 3, shootAngle: 6, coolDown: 2000),
                            new Shoot(10, shoots: 1, coolDown: 800),
                            new HpLessTransition(0.90, "TransAttack")
                            ),
                        new State("TransAttack",
                            new State("TransAttack1",
                                new AddCond(ConditionEffectIndex.Invulnerable), // ok
                                new Taunt(0.99, "Ready..."),
                                new ReturnToSpawn(false, 3),
                                new TimedTransition(1000, "TransAttack1.1")
                                ),
                            new State("TransAttack1.1",
                                new Wander(3),
                                new Taunt(0.99, "Aim..."),
                                new TimedTransition(1000, "Phase2")
                                ),
                            new State("Phase2",
                                new RemCond(ConditionEffectIndex.Invulnerable), // ok
                                new HpLessTransition(0.7, "Phase3"),
                                new Prioritize(
                                    new Chase(2, 10, 3),
                                    new Wander(3),
                                    new StayCloseToSpawn(4, 7)
                                    ),
                                new State("Attack1.1",
                                    new Taunt(0.99, "FIRE!"),
                                    new Shoot(0, shoots: 3, shootAngle: 20, direction: 270, coolDown: 3000),
                                    new Shoot(0, shoots: 3, shootAngle: 20, direction: 90, coolDown: 3000),
                                    new TimedTransition(1000, "Attack1.2")
                                    ),
                                new State("Attack1.2",
                                    new Shoot(0, shoots: 3, shootAngle: 20, direction: 0, coolDown: 3000),
                                    new Shoot(0, shoots: 3, shootAngle: 20, direction: 180, coolDown: 3000),
                                    new TimedTransition(1000, "Attack1.3")
                                    ),
                                new State("Attack1.3",
                                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                                    new Shoot(0, shoots: 3, shootAngle: 20, direction: 270, coolDown: 3000),
                                    new Shoot(0, shoots: 3, shootAngle: 20, direction: 90, coolDown: 3000),
                                    new TimedTransition(1000, "Attack1.4")
                                    ),
                                new State("Attack1.4",
                                    new RemCond(ConditionEffectIndex.Invulnerable), // ok
                                    new Shoot(0, shoots: 12, direction: 360 / 12, coolDown: 3000),
                                    new TimedTransition(1500, "Attack1.5")
                                    ),
                                new State("Attack1.5",
                                    new Shoot(0, shoots: 1, direction: 0, index: 1, coolDown: 3000),
                                    new Shoot(0, shoots: 1, direction: 90, index: 1, coolDown: 3000),
                                    new Shoot(0, shoots: 1, direction: 180, index: 1, coolDown: 3000),
                                    new Shoot(0, shoots: 1, direction: 270, index: 1, coolDown: 3000),
                                    new TimedTransition(1000, "Attack1.6")
                                    ),
                                new State("Attack1.6",
                                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                                    new Shoot(0, shoots: 1, direction: 45, index: 1, coolDown: 3000),
                                    new Shoot(0, shoots: 1, direction: 135, index: 1, coolDown: 3000),
                                    new Shoot(0, shoots: 1, direction: 225, index: 1, coolDown: 3000),
                                    new Shoot(0, shoots: 1, direction: 315, index: 1, coolDown: 3000),
                                    new TimedTransition(1000, "Attack1.7")
                                    ),
                                new State("Attack1.7",
                                    new RemCond(ConditionEffectIndex.Invulnerable), // ok
                                    new Shoot(0, shoots: 1, direction: 0, index: 1, coolDown: 5000),
                                    new Shoot(0, shoots: 1, direction: 90, index: 1, coolDown: 5000),
                                    new Shoot(0, shoots: 1, direction: 180, index: 1, coolDown: 5000),
                                    new Shoot(0, shoots: 1, direction: 270, index: 1, coolDown: 5000),
                                    new TimedTransition(3000, "TransAttack")
                                    )))
                        ),
                    new State("Phase3",
                        new Shoot(0, 4, direction: 360 / 4, rotateAngle: 35, coolDown: 1000),
                        new Shoot(10, shoots: 1, index: 1, coolDown: 1400, aim: 1),
                        new Reproduce("Vengeful Spirit", 20, 4, coolDown: 4000),
                        new Reproduce("Water Mine", 20, 5, coolDown: 3000),
                        new HpLessTransition(0.5, "Phase4"),
                        new ReturnToSpawn(false, 4),
                        new State("Invul",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2000, "Vul")
                            ),
                        new State("Vul",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(3000, "Invul")
                            )
                        ),
                    new State("Phase4",
                        new Prioritize(
                            new Chase(2, 10, 3),
                            new Wander(),
                            new StayCloseToSpawn(4, 7)
                            ),
                        new TossObject("Water Mine", coolDown: 5000, range: (new Random()).Next(3, 7), angle: (new Random()).Next(1, 359)),
                        new Shoot(20, shoots: 2, shootAngle: 8, coolDown: 800),
                        new Shoot(20, shoots: 3, shootAngle: 8, coolDown: 1300),
                        new Shoot(20, shoots: 2, shootAngle: 8, coolDown: 2000, index: 1),
                        new HpLessTransition(0.4, "PrePhase5"),
                        new State("Invul1",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2000, "Vul1")
                            ),
                        new State("Vul1",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(3000, "Invul1")
                            )
                        ),
                    new State("PrePhase5",
                        new TossObject("Tempest Cloud", 8, 0, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 36, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 72, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 108, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 144, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 180, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 216, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 252, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 288, invisiToss: true, coolDown: 10000),
                        new TossObject("Tempest Cloud", 8, 324, invisiToss: true, coolDown: 10000),
                        new EntityExistsTransition("Tempest Cloud", 10, "Phase5")
                        ),
                    new State("Phase5",
                        new EntityOrder(100, "Beach Spectre Spawner", "Active"),
                        new Shoot(0, 4, direction: 360 / 4, rotateAngle: 35, coolDown: 1000),
                        new Shoot(10, shoots: 1, index: 1, coolDown: 1400, aim: 1),
                        new Reproduce("Vengeful Spirit", 20, 4, coolDown: 4000),
                        new Reproduce("Water Mine", 20, 5, coolDown: 3000),
                        new HpLessTransition(0.2, "PrePhase6"),
                        new ReturnToSpawn(false, 4),
                        new State("Invul2",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2000, "Vul2")
                            ),
                        new State("Vul2",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(3000, "Invul2")
                            )
                        ),
                    new State("PrePhase6",
                        new Taunt(0.99, "Fire at will!!!"),
                        new TimedTransition(100, "Phase6")
                        ),
                    new State("Phase6",
                        new Prioritize(
                            new Chase(2, 10, 3),
                            new Wander(),
                            new StayCloseToSpawn(4, 7)
                            ),
                        new TossObject("Water Mine", coolDown: 5000, range: (new Random()).Next(3, 7), angle: (new Random()).Next(1, 359)),
                        new Shoot(20, shoots: 2, shootAngle: 8, coolDown: 800),
                        new Shoot(20, shoots: 3, shootAngle: 8, coolDown: 1300),
                        new Shoot(20, shoots: 2, shootAngle: 8, coolDown: 2000, index: 1),
                        new State("Invul3",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2000, "Vul3")
                            ),
                        new State("Vul3",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(3000, "Invul3")
                            )
                        )
                    ),
                new Drops(
                    new ItemLoot("Ghost Pirate Rum", 1),
                    new ItemLoot("Ghost Pirate Rum", 1),
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 8),
                        new PurpleBag(ItemType.Weapon, 9),
                        new PurpleBag(ItemType.Armor, 8),
                        new PurpleBag(ItemType.Armor, 9),
                        new PurpleBag(ItemType.Ability, 3),
                        new PurpleBag(ItemType.Ability, 4),
                        new PurpleBag(ItemType.Ring, 4)
                        ),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11),
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Ability, 5),
                        new CyanBag(ItemType.Ring, 5),
                        new CyanBag("Wine Cellar Incantation")
                        ),
                    new CyanBag("Spectral Cloth Armor"),
                    new BlueBag(Potions.POTION_OF_WISDOM, true),
                    new WhiteBag(new string[] { "Spirit Dagger", "Ghostly Prism" })
                    )
            )
            .Init("Ghost Ship Anchor",
                new State(
                    new AddCond(ConditionEffectIndex.Invincible),
                    new State("Waiting",
                        new EntityNotExistsTransition("Ghost Ship", 20, "Davy Or No ?")
                        ),
                    new State("Davy Or No ?",
                        new TimedTransition(10, ((new Random()).NextDouble() > .5 ? "Davy" : "nah"), false)
                        ),
                    new State("nah",
                        new Suicide()
                        ),
                    new State("nah2",
                        //new DropPortalOnDeath("Davy Jones' Locker Portal", 1),
                        new Suicide()
                        ),
                    new State("Davy",
                        new GroundTransform("Ghost Water Beach", relativeX: 1, relativeY: 0, persist: true),
                        new GroundTransform("Ghost Water Beach", relativeX: 0, relativeY: 0, persist: true),
                        new GroundTransform("Ghost Water Beach", relativeX: -1, relativeY: 0, persist: true),
                        new GroundTransform("Ghost Water Beach", relativeX: 0, relativeY: 1, persist: true),
                        new GroundTransform("Ghost Water Beach", relativeX: 0, relativeY: -1, persist: true),
                        new TossObject("Palm Tree", 1, angle: 0, coolDown: 10000, invisiToss: true),
                        new TossObject("Palm Tree", 1, angle: 180, coolDown: 10000, invisiToss: true),
                        new TimedTransition(1000, "nah2")
                        )
                    )
            )
            .Init("Vengeful Spirit",
                new State(
                    new ChangeSize(20, 100),
                    new State("Charge",
                        new Prioritize(
                            new Charge(30),
                            new Wander(1)
                            ),
                        new Shoot(5, shoots: 3, shootAngle: 8, coolDown: 1000),
                        new EntityNotExistsTransition("Ghost Ship", 20, "Die")
                        ),
                    new State("Die",
                        new Suicide()
                        )
                    )
            )

            .Init("Tempest Cloud",
                new State(
                    new AddCond(ConditionEffectIndex.Invincible),
                    new State("Texture",
                        new ChangeSize(10, 140),
                        new State("Texture1",
                            new SetAltTexture(1, 9, cooldown: 200),
                            new TimedTransition(2000, "Attack")
                            )
                        ),
                    new State("Attack",
                        new Prioritize(
                            new Circle(4, 8, 20, "Ghost Ship")
                            ),
                        new Shoot(0, shoots: 10, direction: 360 / 10, coolDown: 700),
                        new EntityNotExistsTransition("Ghost Ship", 30, "Die")
                        ),
                    new State("Die",
                        new State("Tex",
                            new SetAltTexture(9),
                            new TimedTransition(200, "Tex1")
                            ),
                        new State("Tex1",
                            new SetAltTexture(8),
                            new TimedTransition(200, "Tex2")
                            ),
                        new State("Tex2",
                            new SetAltTexture(7),
                            new TimedTransition(200, "Tex3")
                            ),
                        new State("Tex3",
                            new SetAltTexture(6),
                            new TimedTransition(200, "Tex4")
                            ),
                        new State("Tex4",
                            new SetAltTexture(5),
                            new TimedTransition(200, "Tex5")
                            ),
                        new State("Tex5",
                            new SetAltTexture(4),
                            new TimedTransition(200, "Tex6")
                            ),
                        new State("Tex6",
                            new SetAltTexture(3),
                            new TimedTransition(200, "Tex7")
                            ),
                        new State("Tex7",
                            new SetAltTexture(2),
                            new TimedTransition(200, "Tex8")
                            ),
                        new State("Tex8",
                            new SetAltTexture(1),
                            new TimedTransition(200, "Tex9")
                            ),
                        new State("Tex9",
                            new SetAltTexture(0),
                            new TimedTransition(200, "Tex10")
                            ),
                        new State("Tex10",
                            new Suicide()
                            )
                        )
                    )
            )

            .Init("Water Mine",
                new State(
                    new Decay(10000),
                    new State("GRAB IT",
                        new EntityNotExistsTransition("Ghost Ship", 20, "BOOOM"),
                        new Prioritize(
                            new Chase(2, 10, 0)
                            ),
                        new PlayerWithinTransition(3, "BOOOM")
                        ),
                    new State("BOOOM",
                        new Shoot(0, shoots: 10, direction: 360 / 10),
                        new Suicide()
                        )
                    )
            )

            .Init("Beach Spectre Spawner",
                new State(
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("Ghost Ship", 60, "Die"),
                    new State("Waiting Order"),
                    new State("Active",
                        new PlayerWithinTransition(7, "Spawn")
                        ),
                    new State("Spawn",
                        new Reproduce("Beach Spectre", 3, 1, coolDown: 1000),
                        new NoPlayerWithinTransition(8, "Active")
                        ),
                    new State("Die",
                        new Suicide()
                        )
                    )
            )

            .Init("Beach Spectre",
                new State(
                    new ChangeSize(20, 100),
                    new State("Attack",
                        new Prioritize(
                            new Wander(1)
                            ),
                        new Shoot(5, shoots: 3, shootAngle: 12, coolDown: 1300),
                        new NoPlayerWithinTransition(7, "Die")
                        ),
                    new State("Die",
                        new AddCond(ConditionEffectIndex.Invincible),
                        new ChangeSize(20, 0),
                        new Decay(3000)
                        )
                    )
            )
        ;
    }
}