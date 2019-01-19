#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class QUEUE_PONG : IncomingMessage
    {
        public int Serial { get; set; }
        public int Time { get; set; }

        public override MessageID ID => MessageID.QUEUE_PONG;

        public override Message CreateInstance() => new QUEUE_PONG();

        protected override void Read(NReader rdr)
        {
            Serial = rdr.ReadInt32();
            Time = rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(Serial);
            wtr.Write(Time);
        }
    }
}