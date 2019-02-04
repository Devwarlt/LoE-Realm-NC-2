using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ SetpieceBossesGreatCoilSnakeSetpiece = () => Behav()
            .Init("Great Coil Snake",
                new State(
                    //new DropPortalOnDeath("Forbidden Jungle Portal", 20, PortalDespawnTimeSec: 100),
                    new Prioritize(
                        new StayCloseToSpawn(8, 5),
                        new Wander()
                        ),
                    new State("Waiting",
                        new PlayerWithinTransition(15, "Attacking")
                        ),
                    new State("Attacking",
                        new Shoot(10, index: 0, coolDown: 700, coolDownOffset: 600),
                        new Shoot(10, 10, 36, 1, coolDown: 2000),
                        new TossObject("Great Snake Egg", 4, 0, 5000, coolDownOffset: 0),
                        new TossObject("Great Snake Egg", 4, 90, 5000, 600),
                        new TossObject("Great Snake Egg", 4, 180, 5000, 1200),
                        new TossObject("Great Snake Egg", 4, 270, 5000, 1800),
                        new NoPlayerWithinTransition(15, "Waiting")
                        )
                    )
            )

            .Init("Great Snake Egg",
                new State(
                    new TransformOnDeath("Great Temple Snake", 1, 2),
                    new State("Wait",
                        new TimedTransition(2500, "Explode"),
                        new PlayerWithinTransition(2, "Explode")
                        ),
                    new State("Explode",
                        new Suicide()
                        )
                    )
            )

            .Init("Great Temple Snake",
                new State(
                    new State("init",
                        new Prioritize(
                            new Chase(6),
                            new Wander()
                            ),
                        new Shoot(10, 2, 7, 0, coolDown: 1000, coolDownOffset: 0),
                        new Shoot(10, 6, 60, 1, coolDown: 2000, coolDownOffset: 600),
                        new TimedTransition(2500, "die")
                        ),
                    new State("die", new Suicide())
                    )
            )
        ;
    }
}