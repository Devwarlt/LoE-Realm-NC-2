using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ HeroesofOryxGhostKingHero = () => Behav()
            .Init("Ghost King",
                new State(
                    new State("Idle",
                        new BackAndForth(3, 3),
                        new HpLessTransition(0.99999, "EvaluationStart1")
                        ),
                    new State("EvaluationStart1",
                        new Taunt("No corporeal creature can kill my sorrow"),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Prioritize(
                            new StayCloseToSpawn(4, range: 3),
                            new Wander()
                            ),
                        new TimedTransition(2500, "EvaluationStart2")
                        ),
                    new State("EvaluationStart2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x0000ff, 0.1, 60),
                        new ChangeSize(20, 140),
                        new Shoot(10, shoots: 4, shootAngle: 30, defaultAngle: 0, coolDown: 1000),
                        new Prioritize(
                            new StayCloseToSpawn(4, range: 3),
                            new Wander()
                            ),
                        new HpLessTransition(0.87, "EvaluationEnd"),
                        new TimedTransition(6000, "EvaluationEnd")
                        ),
                    new State("EvaluationEnd",
                        new Taunt(0.5, "Aye, let's be miserable together"),
                        new HpLessTransition(0.875, "HugeMob"),
                        new HpLessTransition(0.952, "Mob"),
                        new HpLessTransition(0.985, "SmallGroup"),
                        new HpLessTransition(0.99999, "Solo")
                        ),
                    new State("HugeMob",
                        new Taunt("What a HUGE MOB!"),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x00ff00, 0.2, 300),
                        new TossObject("Small Ghost", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 60, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 120, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 240, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 300, coolDown: 100000),
                        new TimedTransition(30000, "HugeMob2")
                        ),
                    new State("HugeMob2",
                        new Taunt("I feel almost manic!"),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x00ff00, 0.2, 300),
                        new TossObject("Small Ghost", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 60, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 120, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 240, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 300, coolDown: 100000),
                        new TimedTransition(30000, "Company")
                        ),
                    new State("Mob",
                        new Taunt("There's a MOB of you."),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x00ff00, 0.2, 300),
                        new TossObject("Small Ghost", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 60, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 120, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 240, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 300, coolDown: 100000),
                        new TimedTransition(30000, "Company")
                        ),
                    new State("Company",
                        new Taunt("Misery loves company!"),
                        new TossObject("Ghost Master", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 60, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 120, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 240, coolDown: 100000),
                        new TossObject("Large Ghost", range: 4, angle: 300, coolDown: 100000),
                        new TimedTransition(2000, "Wait")
                        ),
                    new State("SmallGroup",
                        new Taunt("Such a small party."),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x00ff00, 0.2, 300),
                        new TossObject("Small Ghost", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 60, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 120, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 240, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 300, coolDown: 100000),
                        new TimedTransition(30000, "SmallGroup2")
                        ),
                    new State("SmallGroup2",
                        new Taunt("Misery loves company!"),
                        new TossObject("Ghost Master", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 60, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 120, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 240, coolDown: 100000),
                        new TossObject("Medium Ghost", range: 4, angle: 300, coolDown: 100000),
                        new TimedTransition(2000, "Wait")
                        ),
                    new State("Solo",
                        new Taunt("Just you?  I guess you don't have any friends to play with."),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x00ff00, 0.2, 10),
                        new TossObject("Ghost Master", range: 4, angle: 0, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 70, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 140, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 210, coolDown: 100000),
                        new TossObject("Small Ghost", range: 4, angle: 280, coolDown: 100000),
                        new TimedTransition(1000, "Wait")
                        ),
                    new State("Wait",
                        new Flashing(0x00ff00, 0.2, 10000),
                        new Prioritize(
                            new StayCloseToSpawn(10, range: 8),
                            new Chase(6, range: 2, duration: 2000, coolDown: 2000)
                            ),
                        new Shoot(10, coolDown: 1000),
                        new State("Speak",
                            new Taunt("I cannot be defeated while my loyal subjects sustain me!"),
                            new TimedTransition(1000, "Quiet")
                            ),
                        new State("Quiet",
                            new TimedTransition(22000, "Speak")
                            ),
                        new TimedTransition(40000, "Overly_long_combat")
                        ),
                    new State("Overly_long_combat",
                        new Taunt("You have sapped my energy. A curse on you!"),
                        new Prioritize(
                            new StayCloseToSpawn(10, range: 8),
                            new Chase(6, range: 2, duration: 2000, coolDown: 2000)
                            ),
                        new Shoot(10, coolDown: 1000),
                        new EntityOrder(30, "Ghost Master", "Decay"),
                        new EntityOrder(30, "Small Ghost", "Decay"),
                        new EntityOrder(30, "Medium Ghost", "Decay"),
                        new EntityOrder(30, "Large Ghost", "Decay"),
                        new Transform("Actual Ghost King")
                        ),
                    new State("Killed",
                        new Taunt("I feel my flesh again! For the first time in a 1000 years I LIVE!"),
                        new Taunt(0.5, "Will you release me?"),
                        new Transform("Actual Ghost King")
                        )
                    )
            )

            .Init("Ghost Master",
                new State(
                    new State("Attack1",
                        new State("NewLocation1",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Flashing(0xff00ff00, 0.2, 10),
                            new Prioritize(
                                new StayCloseToSpawn(20, range: 7),
                                new Wander(20)
                                ),
                            new TimedTransition(1000, "Att1")
                            ),
                        new State("Att1",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(10, shoots: 4, shootAngle: 90, direction: 0, coolDown: 400),
                            new TimedTransition(9000, "NewLocation1")
                            ),
                        new HpLessTransition(0.99, "Attack2")
                        ),
                    new State("Attack2",
                        new State("Intro",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Flashing(0xff00ff00, 0.2, 10),
                            new ChangeSize(20, 140),
                            new TimedTransition(1000, "NewLocation2")
                            ),
                        new State("NewLocation2",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Flashing(0xff00ff00, 0.2, 10),
                            new Prioritize(
                                new StayCloseToSpawn(20, range: 7),
                                new Wander(20)
                                ),
                            new TimedTransition(1000, "Att2")
                            ),
                        new State("Att2",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(10, shoots: 4, shootAngle: 90, direction: 45, coolDown: 400),
                            new TimedTransition(6000, "NewLocation2")
                            ),
                        new HpLessTransition(0.98, "Attack3")
                        ),
                    new State("Attack3",
                        new State("Intro",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Flashing(0xff00ff00, 0.2, 10),
                            new ChangeSize(20, 180),
                            new TimedTransition(1000, "NewLocation3")
                            ),
                        new State("NewLocation3",
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new Flashing(0xff00ff00, 0.2, 10),
                            new Prioritize(
                                new StayCloseToSpawn(20, range: 7),
                                new Wander(20)
                                ),
                            new TimedTransition(1000, "Att3")
                            ),
                        new State("Att3",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(10, shoots: 4, shootAngle: 90, direction: 22.5, coolDown: 400),
                            new TimedTransition(3000, "NewLocation3")
                            ),
                        new HpLessTransition(0.94, "KillKing")
                        ),
                    new State("KillKing",
                        new Taunt("Your secret soul master is dying, Your Majesty"),
                        new EntityOrder(30, "Ghost King", "Killed"),
                        new TimedTransition(3000, "Suicide")
                        ),
                    new State("Suicide",
                        new Taunt("I cannot live with my betrayal..."),
                        new Shoot(0, shoots: 8, shootAngle: 45, direction: 22.5),
                        new Decay(0)
                        ),
                    new State("Decay",
                        new Decay(0)
                        )
                    ),
                new ItemLoot("Purple Drake Egg", 0.03),
                new ItemLoot("White Drake Egg", 0.001),
                new ItemLoot("Tincture of Dexterity", 0.02)
            )

            .Init("Actual Ghost King",
                new State(
                    new Taunt(0.9, "I am still so very alone"),
                    new ChangeSize(-20, 95),
                    new Flashing(0xff000000, 0.4, 100),
                    new BackAndForth(5, distance: 3)
                    ),
                 new PinkBag(ItemType.Weapon, 6),
                 new PinkBag(ItemType.Armor, 5),
                 new PinkBag(ItemType.Armor, 6),
                 new PinkBag(ItemType.Ability, 2),
                 new Drops(
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 7),
                        new PurpleBag(ItemType.Weapon, 8),
                        new PurpleBag(ItemType.Armor, 7),
                        new PurpleBag(ItemType.Ability, 3),
                        new PurpleBag(ItemType.Ability, 4),
                        new PurpleBag(ItemType.Ring, 2),
                        new PurpleBag(ItemType.Ring, 3)
                        )
                    )
            )

            .Init("Small Ghost",
                new State(
                    new TransformOnDeath("Medium Ghost"),
                    new State("NewLocation",
                        new Taunt(0.1, "Switch!"),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 10),
                        new Prioritize(
                            new StayCloseToSpawn(20, range: 7),
                            new Wander(20)
                            ),
                        new TimedTransition(1000, "Attack")
                        ),
                    new State("Attack",
                        new Taunt(0.1, "Save the King's Soul!"),
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Shoot(10, shoots: 4, shootAngle: 90, direction: 0, coolDown: 400),
                        new TimedTransition(9000, "NewLocation")
                        ),
                    new State("Decay",
                        new Decay(0)
                        ),
                    new Decay(160000)
                    ),
                new ItemLoot("Magic Potion", 0.02),
                new ItemLoot("Ring of Magic", 0.02),
                new ItemLoot("Ring of Attack", 0.02)
            )

            .Init("Medium Ghost",
                new State(
                    new TransformOnDeath("Large Ghost"),
                    new State("Intro",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 10),
                        new ChangeSize(20, 140),
                        new TimedTransition(1000, "NewLocation")
                        ),
                    new State("NewLocation",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 10),
                        new Prioritize(
                            new StayCloseToSpawn(20, range: 7),
                            new Wander(20)
                            ),
                        new TimedTransition(1000, "Attack")
                        ),
                    new State("Attack",
                        new Taunt(0.02, "I come back more powerful than you could ever imagine"),
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Shoot(10, shoots: 4, shootAngle: 90, direction: 45, coolDown: 800),
                        new TimedTransition(6000, "NewLocation")
                        ),
                    new State("Decay",
                        new Decay(0)
                        ),
                    new Decay(160000)
                    ),
                new ItemLoot("Magic Potion", 0.02),
                new ItemLoot("Ring of Speed", 0.02),
                new ItemLoot("Ring of Attack", 0.02),
                new ItemLoot("Iron Quiver", 0.02)
            )

            .Init("Large Ghost",
                new State(
                    new State("Intro",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 10),
                        new ChangeSize(20, 180),
                        new TimedTransition(1000, "NewLocation")
                        ),
                    new State("NewLocation",
                        new Taunt(0.01,
                            "The Ghost King protects this sacred ground",
                            "The Ghost King gave his heart to the Ghost Master.  He cannot die.",
                            "Only the Secret Ghost Master can kill the King."),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff00ff00, 0.2, 10),
                        new Prioritize(
                            new StayCloseToSpawn(20, range: 7),
                            new Wander(20)
                            ),
                        new TimedTransition(1000, "Attack")
                        ),
                    new State("Attack",
                        new Taunt(0.01, "The King's wife died here.  For her memory."),
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Shoot(10, shoots: 8, shootAngle: 45, direction: 22.5, coolDown: 800),
                        new TimedTransition(3000, "NewLocation"),
                        new EntityNotExistsTransition("Ghost King", 30, "AttackKingGone")
                        ),
                    new State("AttackKingGone",
                        new Taunt(0.01, "The King's wife died here.  For her memory."),
                        new Shoot(10, shoots: 8, shootAngle: 45, direction: 22.5, coolDown: 800, coolDownOffset: 800),
                        new TransformOnDeath("Imp", min: 2, max: 3),
                        new TimedTransition(3000, "NewLocation")
                        ),
                    new State("Decay",
                        new Decay(0)
                        ),
                    new Decay(160000)
                    ),
                new ItemLoot("Magic Potion", 0.02),
                new ItemLoot("Tincture of Defense", 0.02),
                new ItemLoot("Blue Drake Egg", 0.02),
                new ItemLoot("Yellow Drake Egg", 0.02)
            )
        ;
    }
}