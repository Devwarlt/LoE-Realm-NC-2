#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class QUEUE_PING : OutgoingMessage
    {
        public int Serial { get; set; }
        public int Position { get; set; }
        public int Count { get; set; }

        public override MessageID ID => MessageID.QUEUE_PING;
        public override Message CreateInstance() => new QUEUE_PING();

        protected override void Read(NReader rdr)
        {
            Serial = rdr.ReadInt32();
            Position = rdr.ReadInt32();
            Count = rdr.ReadInt32();
        }
        protected override void Write(NWriter wtr)
        {
            wtr.Write(Serial);
            wtr.Write(Position);
            wtr.Write(Count);
        }
    }
}
