#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.mapsetpiece;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class ApplySetpiece : Behavior
    {
        private readonly string name;

        public ApplySetpiece(
            string name
            )
        {
            this.name = name;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            var piece = (MapSetPiece)Activator.CreateInstance(Type.GetType(
                "LoESoft.GameServer.realm.mapsetpiece." + name, true, true));
            piece.RenderSetPiece(host.Owner, new IntPoint((int)host.X, (int)host.Y));
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
        }
    }
}