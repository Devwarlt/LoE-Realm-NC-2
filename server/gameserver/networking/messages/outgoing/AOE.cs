#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class AOE : OutgoingMessage
    {
        public Position Position { get; set; }
        public float Radius { get; set; }
        public ushort Damage { get; set; }
        public ConditionEffectIndex Effects { get; set; }
        public float EffectDuration { get; set; }
        public short OriginType { get; set; }
        public ARGB Color { get; set; }

        public override MessageID ID => MessageID.AOE;

        public override Message CreateInstance() => new AOE();

        protected override void Read(NReader rdr)
        {
            Position = Position.Read(rdr);
            Radius = rdr.ReadSingle();
            Damage = rdr.ReadUInt16();
            Effects = (ConditionEffectIndex)rdr.ReadByte();
            EffectDuration = rdr.ReadSingle();
            OriginType = rdr.ReadInt16();
            Color = ARGB.Read(rdr);
        }

        protected override void Write(NWriter wtr)
        {
            Position.Write(wtr);
            wtr.Write(Radius);
            wtr.Write(Damage);
            wtr.Write((byte)Effects);
            wtr.Write(EffectDuration);
            wtr.Write(OriginType);
            Color.Write(wtr);
        }
    }
}