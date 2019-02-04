#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class TRADECHANGED : OutgoingMessage
    {
        public bool[] Offers { get; set; }

        public override MessageID ID => MessageID.TRADECHANGED;

        public override Message CreateInstance() => new TRADECHANGED();

        protected override void Read(NReader rdr)
        {
            Offers = new bool[rdr.ReadInt16()];
            for (int i = 0; i < Offers.Length; i++)
                Offers[i] = rdr.ReadBoolean();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((ushort)Offers.Length);
            foreach (bool i in Offers)
                wtr.Write(i);
        }
    }
}