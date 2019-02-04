using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ HeroesofOryxEntAncientHero = () => Behav()
            .Init("Ent Ancient",
                new State(
                    new State("Idle",
                        new StayCloseToSpawn(1, range: 6),
                        new Wander(1),
                        new HpLessTransition(0.99999, "EvaluationStart1")
                        ),
                    new State("EvaluationStart1",
                        new Taunt("Uhh. So... sleepy..."),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Prioritize(
                            new StayCloseToSpawn(2, range: 3),
                            new Wander(2)
                            ),
                        new TimedTransition(2500, "EvaluationStart2")
                        ),
                    new State("EvaluationStart2",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0x0000ff, 0.1, 60),
                        new Shoot(10, shoots: 2, shootAngle: 180, coolDown: 800),
                        new Prioritize(
                            new StayCloseToSpawn(3, range: 5),
                            new Wander(3)
                            ),
                        new HpLessTransition(0.87, "EvaluationEnd"),
                        new TimedTransition(6000, "EvaluationEnd")
                        ),
                    new State("EvaluationEnd",
                        new HpLessTransition(0.875, "HugeMob"),
                        new HpLessTransition(0.952, "Mob"),
                        new HpLessTransition(0.985, "SmallGroup"),
                        new HpLessTransition(0.99999, "Solo")
                        ),
                    new State("HugeMob",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Taunt("You are many, yet the sum of your years is nothing."),
                        new Spawn("Greater Nature Sprite", maxChildren: 6, initialSpawn: 0, coolDown: 400),
                        new TossObject("Ent", range: 3, angle: 0, coolDown: 100000),
                        new TossObject("Ent", range: 3, angle: 180, coolDown: 100000),
                        new TossObject("Ent", range: 5, angle: 10, coolDown: 100000),
                        new TossObject("Ent", range: 5, angle: 190, coolDown: 100000),
                        new TossObject("Ent", range: 5, angle: 70, coolDown: 100000),
                        new TossObject("Ent", range: 7, angle: 20, coolDown: 100000),
                        new TossObject("Ent", range: 7, angle: 200, coolDown: 100000),
                        new TossObject("Ent", range: 7, angle: 80, coolDown: 100000),
                        new TossObject("Ent", range: 10, angle: 30, coolDown: 100000),
                        new TossObject("Ent", range: 10, angle: 210, coolDown: 100000),
                        new TossObject("Ent", range: 10, angle: 90, coolDown: 100000),
                        new TimedTransition(5000, "Wait")
                        ),
                    new State("Mob",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Taunt("Little flies, little flies... we will swat you."),
                        new Spawn("Greater Nature Sprite", maxChildren: 3, initialSpawn: 0, coolDown: 1000),
                        new TossObject("Ent", range: 3, angle: 0, coolDown: 100000),
                        new TossObject("Ent", range: 4, angle: 180, coolDown: 100000),
                        new TossObject("Ent", range: 5, angle: 10, coolDown: 100000),
                        new TossObject("Ent", range: 6, angle: 190, coolDown: 100000),
                        new TossObject("Ent", range: 7, angle: 20, coolDown: 100000),
                        new TimedTransition(5000, "Wait")
                        ),
                    new State("SmallGroup",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Taunt("It will be trivial to dispose of you."),
                        new Spawn("Greater Nature Sprite", maxChildren: 1, initialSpawn: 1, coolDown: 100000),
                        new TossObject("Ent", range: 3, angle: 0, coolDown: 100000),
                        new TossObject("Ent", range: 4.5, angle: 180, coolDown: 100000),
                        new TimedTransition(3000, "Wait")
                        ),
                    new State("Solo",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Taunt("Mmm? Did you say something, mortal?"),
                        new TimedTransition(3000, "Wait")
                        ),
                    new State("Wait",
                        new Transform("Actual Ent Ancient")
                        )
                    )
            )

            .Init("Actual Ent Ancient",
                new State(
                    new Prioritize(
                        new StayCloseToSpawn(2, range: 6),
                        new Wander(2)
                        ),
                    new Spawn("Ent Sapling", maxChildren: 3, initialSpawn: 0, coolDown: 3000),
                    new State("Start",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 160),
                        new Shoot(10, index: 0, shoots: 1),
                        new TimedTransition(1600, "Growing1"),
                        new HpLessTransition(0.9, "Growing1")
                        ),
                    new State("Growing1",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 180),
                        new Shoot(10, index: 1, shoots: 3, shootAngle: 120),
                        new TimedTransition(1600, "Growing2"),
                        new HpLessTransition(0.8, "Growing2")
                        ),
                    new State("Growing2",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 200),
                        new Taunt(0.35, "Little mortals, your years are my minutes."),
                        new Shoot(10, index: 2, shoots: 1),
                        new TimedTransition(1600, "Growing3"),
                        new HpLessTransition(0.7, "Growing3")
                        ),
                    new State("Growing3",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 220),
                        new Shoot(10, index: 3, shoots: 1),
                        new TimedTransition(1600, "Growing4"),
                        new HpLessTransition(0.6, "Growing4")
                        ),
                    new State("Growing4",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 240),
                        new Taunt(0.35, "No axe can fell me!"),
                        new Shoot(10, index: 4, shoots: 3, shootAngle: 120),
                        new TimedTransition(1600, "Growing5"),
                        new HpLessTransition(0.5, "Growing5")
                        ),
                    new State("Growing5",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 260),
                        new Shoot(10, index: 5, shoots: 1),
                        new TimedTransition(1600, "Growing6"),
                        new HpLessTransition(0.45, "Growing6")
                        ),
                    new State("Growing6",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 280),
                        new Taunt(0.35, "Yes, YES..."),
                        new Shoot(10, index: 6, shoots: 1),
                        new TimedTransition(1600, "Growing7"),
                        new HpLessTransition(0.4, "Growing7")
                        ),
                    new State("Growing7",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 300),
                        new Shoot(10, index: 7, shoots: 3, shootAngle: 120),
                        new TimedTransition(1600, "Growing8"),
                        new HpLessTransition(0.36, "Growing8")
                        ),
                    new State("Growing8",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 320),
                        new Taunt(0.35, "I am the FOREST!!"),
                        new Shoot(10, index: 8, shoots: 1),
                        new TimedTransition(1600, "Growing9"),
                        new HpLessTransition(0.32, "Growing9")
                        ),
                    new State("Growing9",
                        new RemCond(ConditionEffectIndex.Invulnerable), // ok
                        new ChangeSize(11, 340),
                        new Taunt(1.0, "YOU WILL DIE!!!"),
                        new Shoot(10, index: 9, shoots: 1),
                        new State("convert_sprites",
                            new EntityOrder(50, "Greater Nature Sprite", "Transform"),
                            new TimedTransition(2000, "shielded")
                            ),
                        new State("received_armor",
                            new AddCond(ConditionEffectIndex.Armored),
                            new TimedTransition(1000, "shielded")
                            ),
                        new State("shielded",
                            new AddCond(ConditionEffectIndex.Armored),
                            new TimedTransition(4000, "unshielded")
                            ),
                        new State("unshielded",
                            new RemCond(ConditionEffectIndex.Armored),
                            new Shoot(10, index: 3, shoots: 3, shootAngle: 120, coolDown: 700),
                            new TimedTransition(4000, "shielded")
                            )
                        )
                    ),
                 new PinkBag(ItemType.Weapon, 6),
                 new PinkBag(ItemType.Armor, 5),
                 new PinkBag(ItemType.Armor, 7),
                 new PinkBag(ItemType.Ability, 1),
                 new PinkBag(ItemType.Ability, 2),
                 new Drops(
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 7),
                        new PurpleBag(ItemType.Ability, 3),
                        new PurpleBag(ItemType.Ring, 2),
                        new PurpleBag(ItemType.Ring, 3)
                        )
                     ),
                new ItemLoot("Health Potion", 0.7),
                new ItemLoot("Magic Potion", 0.7)
            )

            .Init("Ent",
                new State(
                    new Prioritize(
                        new Protect(2.5, "Ent Ancient", sightRange: 12, protectRange: 7, reprotectRange: 7),
                        new Chase(2.5, range: 1, sightRange: 9),
                        new Shoot(10, shoots: 5, shootAngle: 72, direction: 30, coolDown: 1600, coolDownOffset: 800)
                        ),
                    new Shoot(10, aim: 0.4, coolDown: 600),
                    new Decay(90000)
                    ),
                new ItemLoot("Tincture of Dexterity", 0.02)
            )

            .Init("Ent Sapling",
                new State(
                    new Prioritize(
                        new Protect(5.5, "Ent Ancient", sightRange: 10, protectRange: 4, reprotectRange: 4),
                        new Wander(5.5)
                        ),
                    new Shoot(10, coolDown: 1000)
                    )
            )

            .Init("Greater Nature Sprite",
                new State(
                    new AddCond(ConditionEffectIndex.Invulnerable), // ok
                    new Shoot(10, shoots: 4, shootAngle: 10),
                    new Prioritize(
                        new StayCloseToSpawn(15, 11),
                        new Circle(15, 4, sightRange: 7),
                        new Chase(2000, sightRange: 7, range: 2),
                        new Chase(3, sightRange: 7, range: 0.2)
                        ),
                    new State("Idle"),
                    new State("Transform",
                        new Transform("Actual Greater Nature Sprite")
                        ),
                    new Decay(90000)
                    )
            )

            .Init("Actual Greater Nature Sprite",
                new State(
                    new Flashing(0xff484848, 0.6, 1000),
                    new Spawn("Ent", maxChildren: 2, initialSpawn: 0, coolDown: 3000),
                    new HealGroup(15, "Heros", coolDown: 200),
                    new State("armor_ent_ancient",
                        new EntityOrder(30, "Actual Ent Ancient", "received_armor"),
                        new TimedTransition(1000, "last_fight")
                        ),
                    new State("last_fight",
                        new Shoot(10, shoots: 4, shootAngle: 10),
                        new Prioritize(
                            new StayCloseToSpawn(15, 11),
                            new Circle(15, 4, sightRange: 7),
                            new Chase(2000, sightRange: 7, range: 2),
                            new Chase(3, sightRange: 7, range: 0.2)
                            )
                        ),
                    new Decay(60000)
                    ),
                new Drops(
                    new ItemLoot("Magic Potion", 0.25),
                    new ItemLoot("Green Drake Egg", 0.08),
                    new PurpleBag(ItemType.Armor, 8),
                    new WhiteBag("Quiver of Thunder")
                    )
            )
        ;
    }
}