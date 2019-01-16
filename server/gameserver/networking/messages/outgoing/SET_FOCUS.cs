#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class SET_FOCUS : OutgoingMessage
    {
        public int ObjectId { get; set; }

        public override MessageID ID => MessageID.SET_FOCUS;

        public override Message CreateInstance() => new SET_FOCUS();

        protected override void Read(NReader rdr)
        {
            ObjectId = rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(ObjectId);
        }
    }
}