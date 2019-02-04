#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class CREATE : IncomingMessage
    {
        public int ClassType { get; set; }
        public int SkinType { get; set; }

        public override MessageID ID => MessageID.CREATE;

        public override Message CreateInstance() => new CREATE();

        protected override void Read(NReader rdr)
        {
            ClassType = rdr.ReadInt16();
            SkinType = rdr.ReadInt16();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((ushort)ClassType);
            wtr.Write((ushort)SkinType);
        }
    }
}