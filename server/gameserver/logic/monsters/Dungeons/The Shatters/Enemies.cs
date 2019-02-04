using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.monsters.Dungeons.The_Shatters.Regular_Enemies;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        /// <summary>
        /// The Shatters
        /// Author: Mike
        /// Co-Author: Filisha and Devwarlt
        /// </summary>
        private _ TheShattersEnemies = () => Behav()

        #region "Beta Behaviors"

        .Init("shtrs Stone Paladin", new StonePaladin().Behavior())
        .Init("shtrs Stone Knight", new StoneKnight().Behavior())
        .Init("shtrs Lava Souls", new LavaSouls().Behavior())
        .Init("shtrs Glassier Archmage", new GlassierArchmage().Behavior())
        .Init("shtrs Ice Adept", new IceAdept().Behavior())
        .Init("shtrs Fire Adept", new FireAdept().Behavior())

        #endregion "Beta Behaviors"

        #region "Beta Boss Behaviors"

         .Init("shtrs Bridge Sentinel", new BridgeSentinel().Behavior())
         .Init("shtrs Twilight Archmage", new TwilightMage().Behavior())
         .Init("shtrs The Forgotten King", new ForgottenKing().Behavior())

        #endregion "Beta Boss Behaviors"

        #region Birds

        .Init("shtrs Inferno", new Inferno().Behavior())
        .Init("shtrs Blizzard", new Blizzard().Behavior())

        #endregion Birds

        .Init("shtrs MagiGenerators",
            new State(
                new State("Main",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new Shoot(15, 10, coolDown: 1000),
                    new Shoot(15, 1, index: 1, coolDown: 2500),
                    new EntitiesNotExistsTransition(30, "Hide", "Shtrs Twilight Archmage", "shtrs Inferno", "shtrs Blizzard")
                ),
                new State("Hide",
                    new SetAltTexture(1),
                    new AddCond(ConditionEffectIndex.Invulnerable)
                    ),
                new State("Despawn",
                    new Decay()
                    )
                  ))

        #region portals

            .Init("shtrs Ice Portal",
                new State(
                    new State("Idle",
                        new TimedTransition(1000, "Spin")
                    ),
                    new State("Spin",
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 0, coolDown: 1200),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 15, coolDown: 1200, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 30, coolDown: 1200, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 45, coolDown: 1200, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 60, coolDown: 1200, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 75, coolDown: 1200, coolDownOffset: 1000),
                            new TimedTransition(1200, "Pause")
                    ),
                    new State("Pause",
                       new TimedTransition(5000, "Idle")
                    )
                )
            )
            .Init("shtrs Fire Portal",
                new State(
                    new State("Idle",
                        new TimedTransition(1000, "Spin")
                    ),
                    new State("Spin",
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 0, coolDown: 1200),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 15, coolDown: 1200, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 30, coolDown: 1200, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 45, coolDown: 1200, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 60, coolDown: 1200, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 6, shootAngle: 60, angleOffset: 75, coolDown: 1200, coolDownOffset: 1000),
                            new TimedTransition(1200, "Pause")
                    ),
                    new State("Pause",
                       new TimedTransition(5000, "Idle")
                    )
                )
            )
            .Init("shtrs Ice Shield",
                new State(
                    new HpLessTransition(.2, "Death"),
                    new State(
                        new Charge(6, 7, coolDown: 5000),
                        new Shoot(3, 6, 60, index: 0, angleOffset: 0, coolDown: 1200),
                        new Shoot(3, 6, 60, index: 0, angleOffset: 10, coolDown: 1200, coolDownOffset: 200),
                        new Shoot(3, 6, 60, index: 0, angleOffset: 20, coolDown: 1200, coolDownOffset: 400),
                        new Shoot(3, 6, 60, index: 0, angleOffset: 30, coolDown: 1200, coolDownOffset: 600),
                        new Shoot(3, 6, 60, index: 0, angleOffset: 40, coolDown: 1200, coolDownOffset: 800),
                        new Shoot(3, 6, 60, index: 0, angleOffset: 50, coolDown: 1200, coolDownOffset: 1000),
                        new TimedTransition(20000, "Death")
                    ),
                    new State("Death",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new Shoot(13, 45, 8, index: 1, angleOffset: 1, coolDown: 10000),
                        new Timed(1000, new Suicide())
                    )
                )
            )
            .Init("shtrs Ice Shield 2",
            new State(
                new HpLessTransition(0.3, "Death"),
                new State(
                    new Circle(5, 5, 1, "shtrs Twilight Archmage"),
                    new Charge(1, 6, coolDown: 10000),
                new Shoot(13, 10, 8, index: 0, coolDown: 1000, angleOffset: 1)
                ),
            new State("Death",
                new AddCond(ConditionEffectIndex.Invincible),
                new Shoot(13, 45, index: 1, coolDown: 10000),
                new Timed(1000, new Suicide())
                )
                )
            )

        #endregion portals

        #region 1stbosschest

            .Init("shtrs Loot Balloon Bridge",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(5000, "Bridge")
                    ),
                    new State("Bridge")
                ),
                new Threshold(0.1,
                    new TierLoot(11, ItemType.Weapon, 1),
                    new TierLoot(12, ItemType.Weapon, 1),
                    new TierLoot(6, ItemType.Ability, 1),
                    new TierLoot(12, ItemType.Armor, 1),
                    new TierLoot(13, ItemType.Armor, 1),
                    new TierLoot(6, ItemType.Ring, 1),
                new Threshold(0.32,
                    new ItemLoot("Potion of Attack", 1),
                    new ItemLoot("Potion of Defense", 1),
                    new ItemLoot("Bracer of the Guardian", 0.3)
                    )
                )
            )

        #endregion 1stbosschest

        #region 2ndbosschest

            .Init("shtrs Loot Balloon Mage",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(5000, "Mage")
                    ),
                    new State("Mage")
                ),
                new Threshold(0.1,
                    new TierLoot(11, ItemType.Weapon, 1),
                    new TierLoot(12, ItemType.Weapon, 1),
                    new TierLoot(6, ItemType.Ability, 1),
                    new TierLoot(12, ItemType.Armor, 1),
                    new TierLoot(13, ItemType.Armor, 1),
                    new TierLoot(6, ItemType.Ring, 1),
                new Threshold(0.32,
                    new ItemLoot("Potion of Mana", 1),
                    new ItemLoot("The Twilight Gemstone", 0.30)
                    )
                )
            )

        #endregion 2ndbosschest

        #region BridgeStatues

            .Init("shtrs Bridge Obelisk A",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new EntityNotExistsTransition("Shtrs Bridge Closer4", 100, "TALK")
                        ),
                    new State("TALK",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                        new TimedTransition(2000, "AFK")
                        ),
                    new State("AFK",
                            new AddCond(ConditionEffectIndex.Invulnerable),
                            new Flashing(0x0000FF0C, 0.5, 4),
                            new TimedTransition(2500, "Shoot")
                        ),
                    new State("Shoot",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 10000),
                            new TimedTransition(10000, "Pause")
                        ),
                    new State("Pause",
                        new TimedTransition(7000, "Shoot")
                        )
                    )
            )
            .Init("shtrs Bridge Obelisk B",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new EntityNotExistsTransition("Shtrs Bridge Closer4", 100, "TALK")
                        ),
                    new State("TALK",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                        new TimedTransition(2000, "AFK")
                        ),
                    new State("AFK",
                            new AddCond(ConditionEffectIndex.Invulnerable),
                            new Flashing(0x0000FF0C, 0.5, 4),
                            new TimedTransition(2500, "Shoot")
                        ),
                    new State("Shoot",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                                new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 10000),
                            new TimedTransition(10000, "Pause")
                        ),
                    new State("Pause",
                        new TimedTransition(7000, "Shoot")
                        )
                    )
            )
            .Init("shtrs Bridge Obelisk D",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new EntityNotExistsTransition("Shtrs Bridge Closer4", 100, "TALK")
                        ),
                    new State("TALK",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                        new TimedTransition(2000, "AFK")
                        ),
                    new State("AFK",
                            new AddCond(ConditionEffectIndex.Invulnerable),
                            new Flashing(0x0000FF0C, 0.5, 4),
                            new TimedTransition(2500, "Shoot")
                        ),
                    new State("Shoot",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 10000),
                            new TimedTransition(10000, "Pause")
                        ),
                    new State("Pause",
                        new TimedTransition(7000, "Shoot")
                        )
                    )
            )
            .Init("shtrs Bridge Obelisk E",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new EntityNotExistsTransition("Shtrs Bridge Closer4", 100, "TALK")
                        ),
                    new State("TALK",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                        new TimedTransition(2000, "AFK")
                        ),
                    new State("AFK",
                            new AddCond(ConditionEffectIndex.Invulnerable),
                            new Flashing(0x0000FF0C, 0.5, 4),
                            new TimedTransition(2500, "Shoot")
                        ),
                    new State("Shoot",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                                new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 10000),
                            new TimedTransition(10000, "Pause")
                        ),
                    new State("Pause",
                        new TimedTransition(7000, "Shoot")
                        )
                    )
            )
            .Init("shtrs Bridge Obelisk C",                                                     //YELLOW TOWERS!
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new AddCond(ConditionEffectIndex.Armored),
                        new EntityNotExistsTransition("Shtrs Bridge Closer4", 100, "JustKillMe")
                        ),
                    new State("JustKillMe",
                        new AddCond(ConditionEffectIndex.Armored),
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(2000, "AFK")
                        ),
                    new State("AFK",
                        new AddCond(ConditionEffectIndex.Armored),
                            new AddCond(ConditionEffectIndex.Invulnerable),
                            new Flashing(0x0000FF0C, 0.5, 4),
                            new TimedTransition(2500, "Shoot")
                        ),
                    new State("Shoot",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                                new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 10000),
                            new TimedTransition(10000, "Pause")
                        ),
                    new State("Pause",
                        new AddCond(ConditionEffectIndex.Armored),
                        new TimedTransition(7000, "Shoot")
                        )
                    )
            )
            .Init("shtrs Bridge Obelisk F",                                                     //YELLOW TOWERS!
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new AddCond(ConditionEffectIndex.Armored),
                        new EntityNotExistsTransition("Shtrs Bridge Closer4", 100, "JustKillMe")
                        ),
                    new State("JustKillMe",
                        new AddCond(ConditionEffectIndex.Armored),
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(2000, "AFK")
                        ),
                    new State("AFK",
                            new AddCond(ConditionEffectIndex.Invulnerable),
                            new AddCond(ConditionEffectIndex.Armored),
                            new Flashing(0x0000FF0C, 0.5, 4),
                            new TimedTransition(2500, "Shoot")
                        ),
                    new State("Shoot",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 1800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 2800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 3800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 4800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 5800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 6800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 7800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 8800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9000),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9200),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9400),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9600),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 9800),
                            new Shoot(0, index: 0, shoots: 4, shootAngle: 90, angleOffset: 45, coolDown: 10000, coolDownOffset: 10000),
                            new TimedTransition(10000, "Pause")
                        ),
                    new State("Pause",
                        new AddCond(ConditionEffectIndex.Armored),
                        new TimedTransition(7000, "Shoot")
                        )
                    )
            )

        #endregion BridgeStatues

        #region SomeMobs

            .Init("shtrs Titanum",
                new State(
                    new State("Wait",
                        new PlayerWithinTransition(5, "spawn")
                            ),
                    new State("spawn",
                        new Spawn("shtrs Stone Knight", maxChildren: 1, initialSpawn: 1, coolDown: 5000),
                        new Spawn("shtrs Stone Mage", maxChildren: 1, initialSpawn: 1, coolDown: 7500)
                        )
                    )
            )
            .Init("shtrs Paladin Obelisk",
                new State(
                    new State("Wait",
                        new PlayerWithinTransition(5, "spawn")
                            ),
                    new State("spawn",
                        new Spawn("shtrs Stone Paladin", maxChildren: 1, initialSpawn: 1, coolDown: 7500)
                        )
                    )
            )
            .Init("shtrs Ice Mage",
                new State(
                    new State("Wait",
                        new PlayerWithinTransition(5, "fire")
                            ),
                    new State("fire",
                        new Chase(5, range: 1),
                        new Shoot(10, 5, 10, index: 0, coolDown: 1500),
                        new TimedTransition(15000, "Spawn")
                        ),
                    new State("Spawn",
                        new Spawn("shtrs Ice Shield", maxChildren: 1, initialSpawn: 1, coolDown: 30000),
                        new TimedTransition(2500, "fire")
                        )
                    )
            )
            .Init("shtrs Archmage of Flame",
            new State(
                new State("wait",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new PlayerWithinTransition(7, "Follow")
                    ),
                new State("Follow",
                    new Chase(10, range: 1),
                    new TimedTransition(5000, "Throw")
                    ),
                new State("Throw",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new TossObject("shtrs Firebomb", 1, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 2, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 3, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 4, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 5, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 6, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 6, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 5, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 4, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 3, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 2, randomToss: true, coolDown: 5000),
                    new TossObject("shtrs Firebomb", 1, randomToss: true, coolDown: 5000),
                    new TimedTransition(4000, "Fire")
                    ),
                new State("Fire",
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 45, angleOffset: 45, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 90, angleOffset: 90, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 135, angleOffset: 135, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 180, angleOffset: 180, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 225, angleOffset: 225, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 270, angleOffset: 270, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 315, angleOffset: 315, coolDown: 1, coolDownOffset: 0),
                    new Shoot(0, index: 0, shoots: 1, shootAngle: 360, angleOffset: 360, coolDown: 1, coolDownOffset: 0),
                    new TimedTransition(5000, "wait")
                    )
                )
            )

            .Init("shtrs Firebomb",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invincible),
                        new TimedTransition(2000, "Explode")
                        ),
                    new State("Explode",
                        new RemCond(ConditionEffectIndex.Invincible),
                        new Shoot(100, index: 0, shoots: 8),
                        new Suicide()
                        )
                    )
            )

            .Init("shtrs Fire Mage",
                new State(
                    new State("Wait",
                        new PlayerWithinTransition(5, "fire")
                            ),
                    new State("fire",
                        new Chase(5, range: 1),
                        new Shoot(10, 5, 10, index: 0, coolDown: 1500),
                        new TimedTransition(10000, "nothing")
                            ),
                    new State("nothing",
                        new TimedTransition(1000, "fire")
                        )
                    )
            )
            .Init("shtrs Stone Mage",
                new State(
                    new State("Wait",
                        new PlayerWithinTransition(5, "fire")
                            ),
                    new State("fire",
                        new RemCond(ConditionEffectIndex.Invulnerable),
                        new Chase(5, range: 1),
                        new Shoot(10, 2, 10, index: 1, coolDown: 200),
                        new TimedTransition(10000, "invulnerable")
                            ),
                    new State("invulnerable",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new Shoot(10, 2, 10, index: 0, coolDown: 200),
                        new TimedTransition(3000, "fire")
                        )
                    )
            )

        #endregion SomeMobs

        #region WOODENGATESSWITCHESBRIDGES

            .Init("shtrs Wooden Gate 3",
                new State(
                    new State("Despawn",
                        new Decay(0)
                        )
                    )
            )
            //.Init("OBJECTHERE",
            //new State(
            //      new EntityNotExistTransition("shtrs Abandoned Switch 1", 10, "OPENGATE")
            //        ),
            //      new State("OPENGATE",
            //            new OpenGate("shtrs Wooden Gate", 10)
            //              )
            //        )
            //      )
            .Init("shtrs Wooden Gate",
                new State(
                    new State("Idle",
                        new EntityNotExistsTransition("shtrs Abandoned Switch 1", 10, "Despawn")
                        ),
                    new State("Despawn",
                        new Decay(0)
                        )
                    )
            )
            .Init("shtrs Wooden Gate 2",
                new State(
                    new State("Idle",
                        new EntityNotExistsTransition("shtrs Abandoned Switch 2", 60, "Despawn")
                        ),
                    new State("Despawn",
                        new Decay(0)
                        )
                    )
            )
        .Init("shtrs Bridge Closer",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible)
                    ),
                new State("Closer",
                    new ChangeGroundOnDeath(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()
                    ),
                new State("TwilightClose",
                    new ChangeGroundOnDeath(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()

                    )
                )
            )
        .Init("shtrs Bridge Closer2",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible)
                    ),
                new State("Closer",
                    new ChangeGroundOnDeath(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()
                    ),
                new State("TwilightClose",
                    new ChangeGroundOnDeath(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()

                    )
                )
            )
        .Init("shtrs Bridge Closer3",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible)
                    ),
                new State("Closer",
                    new ChangeGroundOnDeath(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()
                    ),
                new State("TwilightClose",
                    new ChangeGroundOnDeath(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()

                    )
                )
            )
        .Init("shtrs Bridge Closer4",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible)
                    ),
                new State("Closer",
                    new ChangeGroundOnDeath(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()
                    ),
                new State("TwilightClose",
                    new ChangeGroundOnDeath(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()

                    )
                )
            )
        .Init("shtrs Spawn Bridge",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible)
                    ),
                new State("Open",
                    new ChangeGroundOnDeath(new[] { "shtrs Pure Evil" }, new[] { "shtrs Bridge" },
                        1),
                    new Suicide()
                    )
                )
            )
        .Init("shtrs Spawn Bridge 2",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("shtrs Abandoned Switch 3", 500, "Open")
                    ),
                new State("Open",
                    new ChangeGroundOnDeath(new[] { "shtrs Pure Evil" }, new[] { "shtrs Shattered Floor" },
                        1),
                    new Suicide()
                    ),
                new State("CloseBridge2",
                    new ChangeGroundOnDeath(new[] { "shtrs Shattered Floor" }, new[] { "shtrs Pure Evil" },
                        1),
                    new Suicide()
                    )
                )
            )
        .Init("shtrs Spawn Bridge 3",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("shtrs Twilight Archmage", 500, "Open")
                    ),
                new State("Open",
                    new ChangeGroundOnDeath(new[] { "shtrs Pure Evil" }, new[] { "shtrs Shattered Floor" },
                        1),
                    new Suicide()
                    )
                )
            )
        .Init("shtrs Spawn Bridge 5",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("shtrs Royal Guardian L", 100, "Open")
                    ),
                new State("Open",
                    new ChangeGroundOnDeath(new[] { "Dark Cobblestone" }, new[] { "Hot Lava" },
                        1),
                    new Suicide()
                    )
                )
            )

        #endregion WOODENGATESSWITCHESBRIDGES

        #region 3rdboss

            .Init("shtrs Royal Guardian J",
                new State(
                    new State("shoot",
                        new Circle(1.0, 2, 5, "shtrs The Forgotten King"),
                        new Shoot(15, 8, index: 0)
                        )
                    )
            )
            .Init("shtrs Royal Guardian L",
                new State(
                    new State("1st",
                        new Chase(10, 8, 5),
                        new Shoot(15, 20, index: 0),
                        new TimedTransition(1000, "2nd")
                        ),
                    new State("2nd",
                        new Chase(10, 8, 5),
                        new Shoot(10, index: 1),
                        new TimedTransition(1000, "3rd")
                        ),
                    new State("3rd",
                        new Chase(10, 8, 5),
                        new Shoot(10, index: 1),
                        new TimedTransition(1000, "1st")
                        )
                    )
            )
            .Init("shtrs Green Crystal",
                new State(
                    new Heal(5, 1000, "idkanymore", coolDown: 10000),
                    new State("orbit",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new TimedTransition(8000, "dafuq")
                        ),
                    new State("dafuq",
                        new Circle(10, 2, 5, "shtrs The Forgotten King")
                        )
                    )
            )
            .Init("shtrs Yellow Crystal",
                new State(
                    new State("orbit",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new TimedTransition(25, "shoot")
                        ),
                    new State("shoot",
                        new Shoot(5, 4, 4, index: 0),
                        new TimedTransition(1, "dafuq")
                        ),
                    new State("dafuq",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new Shoot(5, 4, 4, index: 0)
                        )
                    )
            )
            .Init("shtrs Red Crystal",
                new State(
                    new State("orbit",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new TimedTransition(8000, "dafuq")
                        ),
                    new State("dafuq",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new TimedTransition(15000, "ThrowPortal")
                        ),
                    new State("ThrowPortal",
                        new TossObject("shtrs Fire Portal", 5, coolDown: 8000, coolDownOffset: 7000, randomToss: false),
                        new TimedTransition(25, "orbit")
                        )
                    )
            )
            .Init("shtrs Blue Crystal",
                new State(
                    new State("orbit",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new TimedTransition(8000, "dafuq")
                        ),
                    new State("dafuq",
                        new Circle(10, 2, 5, "shtrs The Forgotten King"),
                        new TimedTransition(15000, "ThrowPortal")
                        ),
                    new State("ThrowPortal",
                        new TossObject("shtrs Ice Portal", 5, coolDown: 8000, coolDownOffset: 7000, randomToss: false),
                        new TimedTransition(25, "orbit")
                        )
                    )
            )
        .Init("shtrs The Cursed Crown",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("shtrs Royal Guardian L", 100, "Open")
                    ),
                new State("Open",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new MoveTo(0, -15, 0.5),
                    new TimedTransition(3000, "WADAFAK")
                    ),
                new State("WADAFAK",
                    new TransformOnDeath("shtrs The Forgotten King"),
                    new Suicide()
                    )
                )
            )

        #endregion 3rdboss

        #region 3rdbosschest

            .Init("shtrs Loot Balloon King",
                new State(
                    new State("Idle",
                        new AddCond(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(5000, "Crown")
                    ),
                    new State("Crown")
                ),
                new Threshold(0.1,
                    new TierLoot(11, ItemType.Weapon, 1),
                    new TierLoot(12, ItemType.Weapon, 1),
                    new TierLoot(6, ItemType.Ability, 1),
                    new TierLoot(12, ItemType.Armor, 1),
                    new TierLoot(13, ItemType.Armor, 1),
                    new TierLoot(6, ItemType.Ring, 1),
                new Threshold(0.32,
                    new ItemLoot("Potion of Life", 1),
                    new ItemLoot("The Forgotten Crown", 1)
                    )
                )
            )

        #endregion 3rdbosschest

        // Use this for other stuff.

        #region NotInUse

        //      .Init("shtrs Spawn Bridge 6",
        //          new State(
        //              new State("Idle",
        //                  new ConditionalEffect(ConditionEffectIndex.Invincible, true),
        //                  new EntityNotExistsTransition("shtrs Royal Guardian L", 100, "Open")
        //                  ),
        //              new State("Open",
        //                  new ChangeGroundOnDeath(new[] { "Green BigSmall Squared" }, new[] { "Hot Lava" },
        //                      1),
        //                  new Suicide()
        //                  )
        //              )
        //          )
        //      .Init("shtrs Spawn Bridge 7",
        //          new State(
        //              new State("Idle",
        //                  new ConditionalEffect(ConditionEffectIndex.Invincible, true),
        //                  new EntityNotExistsTransition("shtrs Royal Guardian L", 100, "Open")
        //                  ),
        //              new State("Open",
        //                  new ChangeGroundOnDeath(new[] { "Gold Tile" }, new[] { "Hot Lava" },
        //                      1),
        //                  new Suicide()
        //                  )
        //              )
        //          )
        //      .Init("shtrs Spawn Bridge 8",
        //          new State(
        //              new State("Idle",
        //                  new ConditionalEffect(ConditionEffectIndex.Invincible, true),
        //                  new EntityNotExistsTransition("shtrs Royal Guardian L", 100, "Open")
        //                  ),
        //              new State("Open",
        //                  new ChangeGroundOnDeath(new[] { "Shattered Floor" }, new[] { "Hot Lava" },
        //                      1),
        //                  new Suicide()
        //                  )
        //              )
        //          )

        #endregion NotInUse

        #region MISC

        .Init("shtrs Chest Spawner 1",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("shtrs Bridge Sentinel", 500, "Open")
                    ),
                new State("Open",
                    new TransformOnDeath("shtrs Loot Balloon Bridge"),
                    new Suicide()
                    )
                )
            )
        .Init("shtrs Chest Spawner 2",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntityNotExistsTransition("shtrs Twilight Archmage", 500, "Open")
                    ),
                new State("Open",
                    new TransformOnDeath("shtrs Loot Balloon Mage"),
                    new Suicide()
                    )
                )
            )
        .Init("shtrs Chest Spawner 3",
            new State(
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invincible),
                    new EntitiesNotExistsTransition(30, "Open", "shtrs The Cursed Crown", "shtrs The Forgotten King")
                    ),
                new State("Open",
                    new TransformOnDeath("shtrs Loot Balloon King"),
                    new Suicide()
                    )
                )
            )

        #endregion MISC

            ;
    }
}