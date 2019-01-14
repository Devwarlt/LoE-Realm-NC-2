using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.loot;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ NewRealmQuestBossesLuckyEntGodQuestBoss = () => Behav()
            .Init("Lucky Ent God",
                new State(
                    //new TransformOnDeath("Woodland Labyrinth", probability: 1),
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, shoots: 5, shootAngle: 10, aim: 1, coolDown: 1250)
                    ),
                new Drops(
                    new MostDamagers(2, new BlueBag(Potions.POTION_OF_LIFE, true)),
                    new BlueBag(
                        new string[]
                        {
                            Potions.POTION_OF_ATTACK,
                            Potions.POTION_OF_VITALITY
                        },
                        new bool[]
                        {
                            true,
                            true,
                            true,
                            true
                        }),
                    new EggBasket(new EggType[] { EggType.TIER_0, EggType.TIER_1, EggType.TIER_2, EggType.TIER_3, EggType.TIER_4 }),
                    new CyanBag(ItemType.Weapon, 12),
                    new CyanBag(ItemType.Armor, 13),
                    new CyanBag("Woodland Huntress Skin"),
                    new WhiteBag("Leaf Bow")
                    )
            )
        ;
    }
}