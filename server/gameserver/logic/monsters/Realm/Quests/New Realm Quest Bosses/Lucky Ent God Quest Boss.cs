using LoESoft.GameServer.logic.behaviors;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ NewRealmQuestBossesLuckyEntGodQuestBoss = () => Behav()
            .Init("Lucky Ent God",
                new State(
                    new TransformOnDeath("Woodland Labyrinth",probability: 1),
                    new Prioritize(
                        new StayAbove(10, 200),
                        new Chase(10, range: 7),
                        new Wander()
                        ),
                    new Shoot(12, shoots: 5, shootAngle: 10, aim: 1, coolDown: 1250)
                    )
            )
        ;
    }
}