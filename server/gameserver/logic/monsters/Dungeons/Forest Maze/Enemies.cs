using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb          
    {
        private _ ForestMaze = () => Behav()
        .Init("Mama Megamoth",
            new State(
                new State("swaggin around",
                    new Charge(1, 10, 2000),
                    new Wander(2),
                    new Spawn("Mini Megamoth", coolDown: 2000, initialSpawn: 1),
                    new Reproduce("Mini Megamoth", coolDown: 4000, max: 4),
                    new Shoot(rotateRadius: 10, shoots: 1, index: 0, coolDown: 50)
                    )
                )
            )
        .Init("Mini Megamoth",
            new State(
                new State("protect the queen",
                    new EntityExistsTransition("Mama Megamoth", 100, "protecto the queen"),
                    new EntityNotExistsTransition("Mama Megamoth", 100, "oh crap there is no queen")
                    ),
                new State("protecto the queen",
                    new Protect(1, "Mama Megamoth", 100, 3, 1),
                    new Wander(1),
                    new TimedTransition(5000, "swaggin shot time 1")
                    ),
                new State("swaggin shot time 1",
                    new Shoot(rotateRadius: 8, shoots: 1, coolDown: 100),
                    new TimedTransition(3000, "protecto the queen")
                    ),
                new State("oh crap there is no queen",
                    new Wander(5),
                    new Shoot(rotateRadius: 10, shoots: 1, index: 0, aim: 1, coolDown: 1000),
                    new TimedTransition(5000, "swaggin shots tiem")
                    ),
                new State("swaggin shots tiem",
                    new Shoot(rotateRadius: 8, shoots: 1, coolDown: 100),
                    new TimedTransition(3000, "oh crap there is no queen")
                )
            )
        )
        .Init("Armored Squirrel",
            new State(
                new Prioritize(
                    new Chase(0.6, 6, 1, -1, 0),
                    new Wander(7),
                    new Shoot(rotateRadius: 7, shoots: 2, index: 0, aim: 1, coolDown: 1000, shootAngle: 15)
                    )
                )
            )
        .Init("Ultimate Squirrel",
            new State(
                new Prioritize(
                    new Chase(0.4, 6, 1, -1, 0),
                    new Wander(3)
                    ),
                new Shoot(rotateRadius: 7, shoots: 3, index: 0, shootAngle: 20, coolDown: 2000)
                )
            )
        .Init("Forest Goblin",
            new State(
                new Prioritize(
                    new Wander(4),
                    new Chase(0.7, 10, 3, -1, 0),
                    new Shoot(rotateRadius: 4, shoots: 1, index: 0, coolDown: 500)
                    )
                )
            )
        .Init("Forest Goblin Mage",
            new State(
                new Prioritize(
                    new Wander(4),
                    new Shoot(rotateRadius: 10, shoots: 2, index: 0, aim: 1, coolDown: 500, shootAngle: 2)
                    )
                )
            );
    }
}