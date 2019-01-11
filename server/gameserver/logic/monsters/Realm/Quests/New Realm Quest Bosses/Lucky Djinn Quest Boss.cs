using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ NewRealmQuestBossesLuckyDjinnQuestBoss = () => Behav()
            .Init("Lucky Djinn",
                new State(
                    new TransformOnDeath("The Crawling Depths", probability: 1),
                    new State("Idle",
                        new Prioritize(
                            new StayAbove(10, 200),
                            new Wander(8)
                            ),
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new PlayerWithinTransition(8, "Attacking")
                        ),
                    new State("Attacking",
                        new State("Bullet",
                            new RemCond(ConditionEffectIndex.Invulnerable), // ok
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 0, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 100, coolDownOffset: 200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 110, coolDownOffset: 400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 120, coolDownOffset: 600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 130, coolDownOffset: 800, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 140, coolDownOffset: 1000, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 150, coolDownOffset: 1200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 160, coolDownOffset: 1400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 170, coolDownOffset: 1600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 180, coolDownOffset: 1800, shootAngle: 90),
                            new Shoot(1, shoots: 8, coolDown: 10000, direction: 180, coolDownOffset: 2000, shootAngle: 45),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 180, coolDownOffset: 0, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 170, coolDownOffset: 200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 160, coolDownOffset: 400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 150, coolDownOffset: 600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 140, coolDownOffset: 800, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 130, coolDownOffset: 1000, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 120, coolDownOffset: 1200, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 110, coolDownOffset: 1400, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 100, coolDownOffset: 1600, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 1800, shootAngle: 90),
                            new Shoot(1, shoots: 4, coolDown: 10000, direction: 90, coolDownOffset: 2000, shootAngle: 22.5),
                            new TimedTransition(2000, "Wait")
                            ),
                        new State("Wait",
                            new Chase(7, range: 0.5),
                            new Flashing(0xff00ff00, 0.1, 20),
                            new AddCond(ConditionEffectIndex.Invulnerable), // ok
                            new TimedTransition(2000, "Bullet")
                            ),
                        new NoPlayerWithinTransition(13, "Idle"),
                        new HpLessTransition(0.5, "FlashBeforeExplode")
                        ),
                    new State("FlashBeforeExplode",
                        new AddCond(ConditionEffectIndex.Invulnerable), // ok
                        new Flashing(0xff0000, 0.3, 3),
                        new TimedTransition(1000, "Explode")
                        ),
                    new State("Explode",
                        new Shoot(0, shoots: 10, shootAngle: 36, direction: 0),
                        new Suicide()
                        )
                    )
            )
        ;
    }
}