using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ MaurthTheSuccubusPrincess = () => Behav()
        .Init("Maurth the Succubus Princess",
            new State(
                new State("Intro",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new Taunt("YOU SHALL NOT PASS!"),
                    new TimedTransition(2000, "Act1")
                    ),
                new State("Idle",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new PlayerWithinTransition(24, "Act1")
                    ),
                new State("IdleRage",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new PlayerWithinTransition(24, "Act2")),
                new State("Act1",
                    new RemCond(ConditionEffectIndex.Invulnerable),
                    new Wander(),
                    new Shoot(shoots: 4, shootAngle: 15, aim: 1, coolDown: 300),
                    new Shoot(range: 12, shoots: 3, shootAngle: 20, index: 1, aim: 1, coolDown: 3000),
                    new Shoot(range: 14, index: 2, aim: 1, coolDown: 14000),
                    new HpLessTransition(0.25, "IntroAct2"),
                    new NoPlayerWithinTransition(24, "Idle")
                    ),
                new State("IntroAct2",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new HealGroup(0, "Edition1", 500, 18750),
                    new TimedTransition(5000, "Act2")
                    ),
                new State("Act2",
                    new Taunt("NO ONE CAN BEAT ME!!!"),
                    new Flashing(0xFF0000, 5, 10000),
                    new AddCond(ConditionEffectIndex.Armored),
                    new RemCond(ConditionEffectIndex.Invulnerable),
                    new Wander(),
                    new Chase(6),
                    new Shoot(shoots: 4, shootAngle: 15, aim: 1, coolDown: 200),
                    new Shoot(range: 12, shoots: 3, shootAngle: 20, index: 1, aim: 1, coolDown: 1500),
                    new Shoot(range: 14, index: 2, aim: 1, coolDown: 7000),
                    new NoPlayerWithinTransition(24, "IdleRage")
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
                        new PurpleBag(ItemType.Armor, 8),
                        new PurpleBag(ItemType.Armor, 9)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Armor, 10),
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12)
                        ),
                    new OnlyOne(
                        new PurpleBag(ItemType.Ring, 3),
                        new PurpleBag(ItemType.Ring, 4)
                        ),
                    new CyanBag(ItemType.Ring, 5),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4, EggType.TIER_5 }),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK, true),
                        new BlueBag(Potions.POTION_OF_SPEED, true),
                        new BlueBag(Potions.POTION_OF_VITALITY, true),
                        new BlueBag(Potions.POTION_OF_WISDOM, true)
                        ),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_DEFENSE),
                        new BlueBag(Potions.POTION_OF_DEXTERITY)
                        ),
                    new WhiteBag(new string[] { "The Succubus Bloodstone", "Crossbow of the Frozen North", "Galatic Axe", "Phantom Light-X" })
                    )
            )
        ;
    }
}