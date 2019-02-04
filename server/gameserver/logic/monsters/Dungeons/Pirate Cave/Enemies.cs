using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.logic.transitions;

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ PirateCaveEnemies = () => Behav()
            .Init("Cave Pirate Brawler",
                new State(
                    new Wander(),
                    new Chase(range: 1, speed: 8),
                    new Shoot(range: 3.6, index: 0)
                )
            )
            .Init("Cave Pirate Sailor",
                    new State(
                        new Wander(),
                        new Chase(range: 1, speed: 8),
                        new Shoot(index: 0, range: 3.6, coolDown: 1000)
                        )
                    )
            .Init("Cave Pirate Veteran",
                    new State(
                        new Wander(),
                        new Chase(range: 1, speed: 8),
                        new Shoot(index: 0, range: 3.6, coolDown: 1000)
                        )
                    )
            .Init("Cave Pirate Cabin Boy",
                    new State(
                        new Wander()
                        )
                    )
            .Init("Cave Pirate Hunchback",
                    new State(
                        new Wander()
                        )
                    )
            .Init("Cave Pirate Macaw",
                    new State(
                        new Wander()
                        )
                    )
            .Init("Cave Pirate Moll",
                    new State(
                        new Wander()
                        )
                    )
            .Init("Cave Pirate Monkey",
                    new State(
                        new Wander()
                        )
                    )
            .Init("Cave Pirate Parrot",
                    new State(
                        new Wander()
                        //		new Circle(target: "Cave Pirate Brawler")
                        )
                    )
                .Init("Pirate Admiral",
                new State(
                    new Wander(),
                    new Protect(speed: 5, protectRange: 6, target: "Dreadstump the Pirate King"),
                    new Shoot(range: 9, index: 0, coolDown: 1500)
                )
            )
            .Init("Pirate Captain",
                new State(
                    new Wander(),
                    new Protect(speed: 5, protectRange: 6, target: "Dreadstump the Pirate King"),
                    new Shoot(range: 9, index: 0, coolDown: 1500)
                )
            )
        .Init("Pirate Commander",
                new State(
                    new Wander(),
                    new Protect(speed: 5, protectRange: 6, target: "Dreadstump the Pirate King"),
                    new Shoot(range: 9, index: 0, coolDown: 1500)
                )
            )
            .Init("Pirate Lieutenant",
                new State(
                    new Wander(),
                    new Protect(speed: 5, protectRange: 6, target: "Dreadstump the Pirate King"),
                    new Shoot(range: 9, index: 0, coolDown: 1500)
                )
            )
          .Init("Dreadstump the Pirate King", //check this idk if i did this correct lmao
            new State(
                new State("Idle",
                    new PlayerWithinTransition(15, "begin")
                ),
                new State("begin",
                    new Taunt("I will drink my rum out of your skull!"),
                    new StayCloseToSpawn(1, 7),
                    new Wander(),
                    new Taunt(probability: 0.4, cooldown: 10000, text: "Arrr..."),
                    new Shoot(range: 11.25, index: 0, coolDown: 2000, aim: 0.9),
                    new Shoot(range: 8, index: 1, coolDown: 1000, aim: 0.9),
                    new TimedTransition(1000, "2")
                        ),
                new State("2",
                    new Taunt("Eat Cannonballs!"),
                    new Chase(range: 15, speed: 2),
                    new Shoot(index: 0, range: 11.25, coolDown: 1000, aim: 0.9),
                    new TimedTransition(1000, "begin")
                    )
                    )
            )
        ;
    }
}