using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ EventBossesPentaractEvent = () => Behav()
            .Init("Pentaract Eye",
                new State(
                    new Prioritize(
                        new Swirl(20, 8, 20, true),
                        new Protect(20, "Pentaract Tower", 20, 6, 4)
                        ),
                    new Shoot(9, 1, coolDown: 1000)
                    )
            )

            .Init("Pentaract Tower",
                new State(
                    new Spawn("Pentaract Eye", 5, coolDown: 5000),
                    new Grenade(4, 100, 8, coolDown: 5000, effect: ConditionEffectIndex.Slowed, effectDuration: 6000),
                    new TransformOnDeath("Pentaract Tower Corpse"),
                    new CopyDamageOnDeath("Pentaract"),
                    new CopyDamageOnDeath("Pentaract Tower Corpse")
                    )
            )

            .Init("Pentaract",
                new State(
                    new AddCond(ConditionEffectIndex.Invincible),
                    new State("Waiting",
                        new EntityNotExistsTransition("Pentaract Tower", 50, "Die")
                        ),
                    new State("Die",
                        new Suicide()
                        )
                    )
            )

            .Init("Pentaract Tower Corpse",
                new State(
                    new AddCond(ConditionEffectIndex.Invulnerable),
                    new State("Waiting",
                        new TimedTransition(15000, "Spawn"),
                        new EntityNotExistsTransition("Pentaract Tower", 50, "Die")
                        ),
                    new State("Spawn",
                        new Transform("Pentaract Tower")
                        ),
                    new State("Die",
                        new Suicide()
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
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new OnlyOne(
                        new BlueBag(Potions.POTION_OF_ATTACK),
                        new BlueBag(Potions.POTION_OF_DEFENSE),
                        new BlueBag(Potions.POTION_OF_SPEED),
                        new BlueBag(Potions.POTION_OF_DEXTERITY),
                        new BlueBag(Potions.POTION_OF_VITALITY),
                        new BlueBag(Potions.POTION_OF_WISDOM)
                        ),
                    new WhiteBag("Seal of Blasphemous Prayer")
                    )
            )
        ;
    }
}