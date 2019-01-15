using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ UndertakerTheGreatJuggernaut = () => Behav()
        .Init("Undertaker the Great Juggernaut",
            new State(
                new State("Awaiting",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new WorldQuestNotification(),
                    new PlayerWithinTransition(24, "Intro1")
                    ),
                new State("Intro1",
                    new Taunt("Of all the kingdoms this is the most chaotic I've ever seen..."),
                    new TimedTransition(2000, "Intro2")
                    ),
                new State("Intro2",
                    new Taunt("These humans... Every time they disturb the kingdom of Oryx!"),
                    new TimedTransition(2000, "Intro3")
                    ),
                new State("Intro3",
                    new Taunt("This may not be acceptable anymore! I'm gonna do my duty..."),
                    new TimedTransition(2000, "Intro4")
                    ),
                new State("Intro4",
                    new Taunt("This time I'm the LAW here!"),
                    new TimedTransition(2000, "Act1")
                    ),
                new State("Act1",
                    new TossObject("shtrs Titanum", 5, 0, int.MaxValue - 1, 0, false, true),
                    new TossObject("shtrs Titanum", 5, 90, int.MaxValue - 1, 0, false, true),
                    new TossObject("shtrs Titanum", 5, 180, int.MaxValue - 1, 0, false, true),
                    new TossObject("shtrs Titanum", 5, 270, int.MaxValue - 1, 0, false, true),
                    new TossObject("shtrs Titanum", 10, 0, int.MaxValue - 1, 500, false, true),
                    new TossObject("shtrs Titanum", 10, 90, int.MaxValue - 1, 500, false, true),
                    new TossObject("shtrs Titanum", 10, 180, int.MaxValue - 1, 500, false, true),
                    new TossObject("shtrs Titanum", 10, 270, int.MaxValue - 1, 500, false, true),
                    new TossObject("shtrs Titanum", 15, 0, int.MaxValue - 1, 1000, false, true),
                    new TossObject("shtrs Titanum", 15, 90, int.MaxValue - 1, 1000, false, true),
                    new TossObject("shtrs Titanum", 15, 180, int.MaxValue - 1, 1000, false, true),
                    new TossObject("shtrs Titanum", 15, 270, int.MaxValue - 1, 1000, false, true),
                    new Taunt("Prepare to fight it's just beginning!"),
                    new TimedTransition(10000, "AwaitAct1")
                    ),
                new State("AwaitAct1",
                    new EntityNotExistsTransition("shtrs Titanum", 20, "Act2")
                    ),
                new State("IdleAct2",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new RemCond(ConditionEffectIndex.Armored),
                    new PlayerWithinTransition(24, "Act2")
                    ),
                new State("Act2",
                    new Wander(),
                    new RemCond(ConditionEffectIndex.Invulnerable),
                    new AddCond(ConditionEffectIndex.Armored),
                    new Shoot(8, 8, 360 / 8, 0, coolDown: 2000),
                    new Reproduce("Henchman of Oryx", 8, 5, 1, 10000),
                    new HpLessTransition(.8, "Act3"),
                    new NoPlayerWithinTransition(24, "IdleAct2")
                    ),
                new State("IdleAct3",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new RemCond(ConditionEffectIndex.Armored),
                    new PlayerWithinTransition(24, "Act3")
                    ),
                new State("Act3",
                    new Chase(4),
                    new RemCond(ConditionEffectIndex.Invulnerable),
                    new AddCond(ConditionEffectIndex.Armored),
                    new Shoot(8, 8, 360 / 8, 0, coolDown: 2000),
                    new Shoot(range: 4, index: 1, coolDown: 4000),
                    new Shoot(range: 2, shoots: 2, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 200),
                    new Shoot(range: 3, shoots: 3, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 400),
                    new Shoot(range: 4, shoots: 4, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 600),
                    new Shoot(range: 5, shoots: 5, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 800),
                    new HpLessTransition(.6, "Act4"),
                    new NoPlayerWithinTransition(24, "IdleAct3")
                    ),
                new State("IdleAct4",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new RemCond(ConditionEffectIndex.Armored),
                    new PlayerWithinTransition(24, "Act4")
                    ),
                new State("Act4",
                    new Chase(),
                    new RemCond(ConditionEffectIndex.Invulnerable),
                    new AddCond(ConditionEffectIndex.Armored),
                    new Reproduce("Henchman of Oryx", 8, 5, 1, 10000),
                    new Shoot(8, 8, 360 / 8, 0, coolDown: 2000),
                    new Shoot(range: 4, index: 1, coolDown: 4000),
                    new Shoot(range: 2, shoots: 2, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 200),
                    new Shoot(range: 3, shoots: 3, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 400),
                    new Shoot(range: 4, shoots: 4, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 600),
                    new Shoot(range: 5, shoots: 5, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 800),
                    new Shoot(range: 4, index: 0, coolDown: 2000),
                    new Shoot(range: 4, shoots: 2, shootAngle: 10, index: 0, coolDown: 2000, coolDownOffset: 200),
                    new HpLessTransition(.05, "IntroAct5"),
                    new NoPlayerWithinTransition(24, "IdleAct4")
                    ),
                new State("IntroAct5",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new RemCond(ConditionEffectIndex.Armored),
                    new Taunt("GATHERING POWER!"),
                    new HealGroup(0, "Edition1", 500, 71250),
                    new Flashing(0xCD0000, 1, 999999),
                    new TimedTransition(5000, "Act5")
                    ),
                new State("IdleAct5",
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new RemCond(ConditionEffectIndex.Berserk),
                    new RemCond(ConditionEffectIndex.Speedy),
                    new PlayerWithinTransition(24, "Act5")
                    ),
                new State("Act5",
                    new Chase(6),
                    new RemCond(ConditionEffectIndex.Invulnerable),
                    new AddCond(ConditionEffectIndex.Berserk),
                    new AddCond(ConditionEffectIndex.Speedy),
                    new Reproduce("Henchman of Oryx", 8, 5, 1, 10000),
                    new Shoot(8, 8, 360 / 8, 0, coolDown: 2000),
                    new Shoot(range: 4, index: 1, coolDown: 4000),
                    new Shoot(range: 2, shoots: 2, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 200),
                    new Shoot(range: 3, shoots: 3, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 400),
                    new Shoot(range: 4, shoots: 4, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 600),
                    new Shoot(range: 5, shoots: 5, shootAngle: 22.5, index: 1, coolDown: 4000, coolDownOffset: 800),
                    new Shoot(range: 4, index: 0, coolDown: 2000),
                    new Shoot(range: 4, shoots: 2, shootAngle: 10, index: 0, coolDown: 2000, coolDownOffset: 200),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 0, coolDown: 2400),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 15, coolDown: 2400, coolDownOffset: 500),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 30, coolDown: 2400, coolDownOffset: 500 * 2),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 45, coolDown: 2400, coolDownOffset: 500 * 3),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 60, coolDown: 2400, coolDownOffset: 500 * 4),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 75, coolDown: 2400, coolDownOffset: 500 * 5),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 90, coolDown: 2400, coolDownOffset: 500 * 6),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 75, coolDown: 2400, coolDownOffset: 500 * 7),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 60, coolDown: 2400, coolDownOffset: 500 * 8),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 45, coolDown: 2400, coolDownOffset: 500 * 9),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 30, coolDown: 2400, coolDownOffset: 500 * 10),
                    new Shoot(range: 6, shoots: 4, index: 1, shootAngle: 90, direction: 15, coolDown: 2400, coolDownOffset: 500 * 11),
                    new HpLessTransition(.01, "Death"),
                    new NoPlayerWithinTransition(24, "IdleAct5")
                    ),
                new State("Death",
                    new Taunt("I can't believe that I have failed you my Lord..."),
                    new Shoot(36, 8, 360 / 36, 0, coolDown: 5000)
                    )
                ),
                new Drops(
                    new OnlyOne(
                        new PinkBag(ItemType.Weapon, 8),
                        new PinkBag(ItemType.Weapon, 9),
                        new PinkBag(ItemType.Ability, 4),
                        new PinkBag(ItemType.Armor, 8),
                        new PinkBag(ItemType.Armor, 9),
                        new PinkBag(ItemType.Ring, 3),
                        new PinkBag(ItemType.Ring, 4)
                        ),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4, EggType.TIER_5 }),
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 10),
                        new PurpleBag(ItemType.Weapon, 11),
                        new PurpleBag(ItemType.Weapon, 12),
                        new PurpleBag(ItemType.Armor, 10),
                        new PurpleBag(ItemType.Armor, 11),
                        new PurpleBag(ItemType.Armor, 12),
                        new PurpleBag(ItemType.Armor, 13),
                        new PurpleBag(ItemType.Ability, 5),
                        new PurpleBag(ItemType.Ability, 6),
                        new PurpleBag(ItemType.Ring, 5),
                        new PurpleBag(ItemType.Ring, 6)
                        ),
                    new OnlyOne(
                        new CyanBag("Sword of Splendor"),
                        new CyanBag("Bow of Mystical Energy"),
                        new CyanBag("Dagger of Sinister Deeds"),
                        new CyanBag("Staff of the Vital Unity"),
                        new CyanBag("Wand of Evocation"),
                        new CyanBag("Sadamune")
                        ),
                    new OnlyOne(
                        new CyanBag("Robe of the Star Mother"),
                        new CyanBag("Dominion Armor"),
                        new CyanBag("Wyrmhide Armor")
                        ),
                    new OnlyOne(
                        new WhiteBag("Enchanted Ice Blade"),
                        new WhiteBag("Staff of Iceblast"),
                        new WhiteBag("Eternal Snowflake Wand"),
                        new WhiteBag("Arctic Bow"),
                        new WhiteBag("Frost Drake Hide Armor"),
                        new WhiteBag("Frost Elementalist Robe"),
                        new WhiteBag("Ice Crown"),
                        new WhiteBag("Frost Citadel Armor"),
                        new WhiteBag("Penguin Knight Skin"),
                        new WhiteBag("Frimar Knight Skin"),
                        new WhiteBag("Blizzard Sorcerer Skin")
                        ),
                    new WhiteBag("Helm of the Juggernaut"),
                    new WhiteBag("The Rusher Skin"),
                    new BlueBag(Potions.POTION_OF_LIFE, true),
                    new BlueBag(Potions.POTION_OF_MANA, true),
                    new OnlyOne(
                        new BlueBag(GreaterPotions.GREATER_POTION_OF_LIFE),
                        new BlueBag(GreaterPotions.GREATER_POTION_OF_MANA)
                        ),
                    new OnlyOne(
                        new BlueBag(GreaterPotions.GREATER_POTION_OF_LIFE),
                        new BlueBag(GreaterPotions.GREATER_POTION_OF_MANA)
                        ),
                    new OnlyOne(
                        new BlueBag(GreaterPotions.GREATER_POTION_OF_LIFE),
                        new BlueBag(GreaterPotions.GREATER_POTION_OF_MANA)
                        ),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK),
                        new BlueBag(Potions.POTION_OF_DEFENSE),
                        new BlueBag(Potions.POTION_OF_SPEED),
                        new BlueBag(Potions.POTION_OF_DEXTERITY),
                        new BlueBag(Potions.POTION_OF_VITALITY),
                        new BlueBag(Potions.POTION_OF_WISDOM)
                        ),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK),
                        new BlueBag(Potions.POTION_OF_DEFENSE),
                        new BlueBag(Potions.POTION_OF_SPEED),
                        new BlueBag(Potions.POTION_OF_DEXTERITY),
                        new BlueBag(Potions.POTION_OF_VITALITY),
                        new BlueBag(Potions.POTION_OF_WISDOM)
                        ),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK),
                        new BlueBag(Potions.POTION_OF_DEFENSE),
                        new BlueBag(Potions.POTION_OF_SPEED),
                        new BlueBag(Potions.POTION_OF_DEXTERITY),
                        new BlueBag(Potions.POTION_OF_VITALITY),
                        new BlueBag(Potions.POTION_OF_WISDOM)
                        )
                    )
            )
        ;
    }
}