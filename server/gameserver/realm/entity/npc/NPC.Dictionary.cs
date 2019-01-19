#region

using LoESoft.GameServer.logic.behaviors;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.npc;
using LoESoft.GameServer.realm.entity.npc.npcs;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.logic
{
    public class NPCs
    {
        // Do not change it!
        public static readonly Dictionary<string, NPC> Database = new Dictionary<string, NPC>
        {
            { "NPC Gazer", new Gazer() }
        };

        public NPCs()
        {
            // Process all NPCs creating new instance for each one
            foreach (var i in Database)
                i.Value.Config(Entity.Resolve(i.Key), null, false);
        }
    }

    partial class BehaviorDb
    {
        private _ NPCCache = () => Behav()
            .Init("NPC Gazer", new State(new NPCEngine(NPCStars: 70)))
        ;
    }
}