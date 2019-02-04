#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class PLAYSOUND : OutgoingMessage
    {
        public int OwnerId { get; set; }
        public int SoundId { get; set; }

        public override MessageID ID => MessageID.PLAYSOUND;

        public override Message CreateInstance() => new PLAYSOUND();

        protected override void Read(NReader rdr)
        {
            OwnerId = rdr.ReadInt32();
            SoundId = rdr.ReadByte();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(OwnerId);
            wtr.Write((byte)SoundId);
        }
    }
}