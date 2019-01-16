#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class SERVER_FULL : OutgoingMessage
    {
        public int Position { get; set; }
        public int Count { get; set; }

        public override MessageID ID => MessageID.SERVER_FULL;
        public override Message CreateInstance() => new SERVER_FULL();

        protected override void Read(NReader rdr)
        {
            Position = rdr.ReadInt32();
            Count = rdr.ReadInt32();
        }
        protected override void Write(NWriter wtr)
        {
            wtr.Write(Position);
            wtr.Write(Count);
        }
    }
}
