using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ DynoBot = () => Behav()
            .Init("Dyno Bot",
				 new State(
					new State("default",
						new AddCond(ConditionEffectIndex.Invincible),
						new PlayerWithinTransition(8, "1")
						),
					new State("1",
						new AddCond(ConditionEffectIndex.Invincible),
						new Taunt("I am the Dyno Bot from Discord. My features includes muting, kicking, and banning. Somehow I got lost and ended up in LoE Realm. So now I am the offical bot of LoE Realms."),
						new TimedTransition(5000, "2")
						),
					new State("2",
						new Flashing(0xFF0000, 2, 2),
						new Wander(2),
						new Taunt("You have broken Discord's TOS. Please allow me to ip-ban your account and terminate it."),
						new RemCond(ConditionEffectIndex.Invincible),
						new AddCond(ConditionEffectIndex.Armored),
						new Shoot(8, shoots: 8, index: 0, coolDown: 4550, coolDownOffset: 1500),
						new Shoot(8, shoots: 10, index: 1, coolDown: 2000, shootAngle: 22),
						new TimedTransition(6400, "3")
						),
					new State("3",
						new Prioritize(
							new Chase(5, 8, 1),
							new Wander(4)
							),
						new Shoot(8, shoots: 6, shootAngle: 14, index: 1, coolDown: 3200),
						new Shoot(8, shoots: 6, aim: 1, shootAngle: 28, index: 1, coolDown: 2440),
						new Shoot(10, shoots: 5, index: 0, coolDown: 1600),
						new TimedTransition(6000, "4")
						),
					new State("4",
						new Prioritize(
							new StayCloseToSpawn(4),
							new Wander(4)
							),
						new Heal(range:0,coolDown: 2000, amount: 400),
						new Shoot(9, shoots: 7, index: 1, coolDown: new Cooldown(3000, 1000)),
						new Shoot(10, shoots: 8, shootAngle: 14, index: 0, coolDown: 1500),
						new TimedTransition(6200, "5")
						),
					new State("5",
						new Flashing(0x00F0FF, 2, 2),
						new Grenade(5, 300, range: 8, coolDown: 1000),
						new Taunt("You have been warned in LoE Realm's Discord for spamming and being toxic. Please stop. Thank you for understanding. Regardless, im about to kick you. "),
						new ReturnToSpawn(speed: 1),
						new Shoot(8, shoots: 7, index: 1, coolDown: 750, shootAngle: 12),
						new TimedTransition(8000, "6")
						),
					new State("6",
						new Taunt("You have been warned 3 times now in LoE Realm. Prepared to be banned."),
						new Wander(3),
						new Shoot(9, shoots: 14, index: 0, coolDown: 2000),
						new Shoot(9, shoots: 6, index: 1, aim: 2, coolDown: 1000),
						new TimedTransition(6000, "2")
						)
					),
				new Drops(
                    new BlueBag(Potions.POTION_OF_DEXTERITY),
                    new OnlyOne(
                        new PurpleBag(ItemType.Weapon, 9),
                        new PurpleBag(ItemType.Weapon, 10)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Weapon, 10),
                        new CyanBag(ItemType.Weapon, 11)
                        ),
                    new OnlyOne(
                        new PurpleBag(ItemType.Ability, 2),
                        new PurpleBag(ItemType.Ability, 3)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Ability, 4),
                        new CyanBag(ItemType.Ability, 5)
                        ),
                    new OnlyOne(
                        new PurpleBag(ItemType.Armor, 9),
                        new PurpleBag(ItemType.Armor, 10)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Armor, 11),
                        new CyanBag(ItemType.Armor, 12)
                        ),
                    new OnlyOne(
                        new PurpleBag(ItemType.Ring, 2),
                        new PurpleBag(ItemType.Ring, 3)
                        ),
                    new OnlyOne(
                        new CyanBag(ItemType.Ring, 4),
                        new CyanBag(ItemType.Ring, 5)
                        ),
                    new WhiteBag(new[] { "Doom Spoon", "The Mad Jester's Garment" })
                    )
            )
        ;
    }
}