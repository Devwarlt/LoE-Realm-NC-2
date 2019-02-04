#region

using LoESoft.Core;
using System;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class QUEST_ROOM_MSG : IncomingMessage
    {
        public override MessageID ID => MessageID.QUEST_ROOM_MSG;

        public override Message CreateInstance() => new QUEST_ROOM_MSG();

        protected override void Read(NReader rdr)
        {
        }

        protected override void Write(NWriter wtr)
        {
            throw new InvalidOperationException();
        }
    }
}