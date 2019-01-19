#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class SHOWEFFECT : OutgoingMessage
    {
        public EffectType EffectType { get; set; }
        public int TargetId { get; set; }
        public Position PosA { get; set; }
        public Position PosB { get; set; }
        public ARGB Color { get; set; }

        public override MessageID ID => MessageID.SHOWEFFECT;

        public override Message CreateInstance() => new SHOWEFFECT();

        protected override void Read(NReader rdr)
        {
            EffectType = (EffectType)rdr.ReadByte();
            TargetId = rdr.ReadInt32();
            PosA = Position.Read(rdr);
            PosB = Position.Read(rdr);
            Color = ARGB.Read(rdr);
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((byte)EffectType);
            wtr.Write(TargetId);
            PosA.Write(wtr);
            PosB.Write(wtr);
            Color.Write(wtr);
        }
    }
}