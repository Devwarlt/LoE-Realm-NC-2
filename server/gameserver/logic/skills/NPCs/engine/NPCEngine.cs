#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.npc;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    /** NPC Engine (LoESoft Games)
	* Author: DV
	* Code Review: Sebafra
	*/

    // Engine only (this engine is not part of Behavior engine, only use it to integrate both)
    public class NPCEngine : Behavior
    {
        // NPC read-only variables (declaration)
        protected List<string> _playerWelcomeMessages { get; set; }

        protected List<string> _playerLeaveMessages { get; set; }
        protected List<string> _NPCLeaveMessages { get; set; }
        protected bool _randomNPCLeaveMessages { get; set; }
        protected double _range { get; set; }
        protected int _NPCStars { get; set; }

        protected NPC _NPC { get; set; }

        // Engine read-only variables
        protected DateTime _now => DateTime.Now; // get current time (real-time)

        protected int _delay = 1000; // in milliseconds

        public NPCEngine(
            List<string> playerWelcomeMessages = null,
            List<string> playerLeaveMessages = null,
            List<string> NPCLeaveMessages = null,
            bool randomNPCLeaveMessages = true,
            double range = 4.00,
            int NPCStars = 0
            )
        {
            _playerWelcomeMessages = playerWelcomeMessages ?? new List<string> { "hi", "hello", "hey", "good morning", "good afternoon", "good evening" };
            _playerLeaveMessages = playerLeaveMessages ?? new List<string> { "bye", "see ya", "goodbye", "see you", "see you soon", "goodnight" };
            _NPCLeaveMessages = NPCLeaveMessages ?? new List<string> { "Farewell, {PLAYER}!", "Good bye, then...", "Cheers!", "See you soon, {PLAYER}!" };
            _randomNPCLeaveMessages = randomNPCLeaveMessages;
            _range = range;
            _NPCStars = NPCStars;
        }

        // first handler, initialize engine (declarations only)
        protected override void OnStateEntry(
            Entity npc,
            RealmTime time,
            ref object state
            )
        {
            _NPC = NPCs.Database.ContainsKey(npc.Name) ? NPCs.Database[npc.Name] : null;
            _NPC.Config(npc, _NPCLeaveMessages, _randomNPCLeaveMessages);
            _NPC.UpdateNPCStars(_NPCStars);
            _NPC.UpdateNPC(npc);
            state = 0;
        }

        // duty loop
        protected override void TickCore(
            Entity npc,
            RealmTime time,
            ref object state
            )
        {
            if (_NPC == null)
                return;
            List<Entity> entities = npc.GetNearestEntities(_range * 2, null).ToList();
            Parallel.ForEach(entities, entity =>
            {
                Player player;
                if (entity is Player)
                    player = entity as Player;
                else
                    return;
                if (_NPC.ReturnPlayersCache().Contains(player.Name) && !entities.Contains(player))
                {
                    _NPC.RemovePlayer(player); // Removing player into NPC's players cache.
                    ChatManager.ChatDataCache.Remove(player.Name); // Removing player from chat data cache.
                    return;
                }
                try
                {
                    if (npc.Dist(player) > _range && _NPC.ReturnPlayersCache().Contains(player.Name))
                    {
                        _NPC.RemovePlayer(player); // Removing player into NPC's players cache.
                        _NPC.Leave(player, false); // Player sent a valid leave message (polite: false), processing NPC reply.
                        ChatManager.ChatDataCache.Remove(player.Name); // Removing player from chat data cache.
                        return;
                    }
                    if (ChatManager.ChatDataCache.ContainsKey(player.Name))
                    {
                        //Chat data cache contains player
                        Tuple<DateTime, string> messageInfo = ChatManager.ChatDataCache[player.Name];
                        if (_NPC.ReturnPlayersCache().Contains(player.Name))
                        {
                            // NPC's players cache contains player.
                            if (npc.Dist(player) <= _range)
                            {
                                if (_playerWelcomeMessages.Contains(messageInfo.Item2.ToLower()))
                                {
                                    _NPC.NoRepeat(player); // Duplicated NPC Welcome message! Sending non-repeat message to player
                                    ChatManager.ChatDataCache.Remove(player.Name); // Removing player from chat data cache.
                                    return;
                                }
                                if (_playerLeaveMessages.Contains(messageInfo.Item2.ToLower()) && messageInfo.Item1 >= _now.AddMilliseconds(-_delay) && messageInfo.Item1 <= _now.AddMilliseconds(_delay))
                                {
                                    _NPC.RemovePlayer(player); // Removing player into NPC's players cache.
                                    _NPC.Leave(player, true); // Player sent a valid leave message (polite: true), processing NPC reply.
                                    ChatManager.ChatDataCache.Remove(player.Name); // Removing player from chat data cache.
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (_playerWelcomeMessages.Contains(messageInfo.Item2.ToLower()) && npc.Dist(player) <= _range && messageInfo.Item1 >= _now.AddMilliseconds(-_delay) && messageInfo.Item1 <= _now.AddMilliseconds(_delay))
                            {
                                _NPC.AddPlayer(player); // Adding player into NPC's players cache.
                                _NPC.Welcome(player); // Processing Welcome message to player target.
                                ChatManager.ChatDataCache.Remove(player.Name); // Removing player from chat data cache.
                            }
                        }
                    }
                }
                catch (InvalidOperationException) { } // collection can be updated, so new handler exception for it
            });
        }
    }
}