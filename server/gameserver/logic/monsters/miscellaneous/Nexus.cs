#region

using LoESoft.GameServer.logic.behaviors;

#endregion

namespace LoESoft.GameServer.logic
{
    partial class BehaviorDb
    {
        private _ Nexus = () => Behav()
            .Init("White Fountain", new State(new NexusHealHp(5, 100, 1000)))
            .Init("Winter Fountain Frozen", new State(new NexusHealHp(5, 100, 1000)))
            .Init("Halloween Fountain", new State(new NexusHealHp(5, 100, 1000, 0xFFA500)))
        ;
    }
}