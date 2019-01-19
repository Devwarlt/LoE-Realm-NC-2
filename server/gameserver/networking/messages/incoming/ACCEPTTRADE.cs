#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class ACCEPTTRADE : IncomingMessage
    {
        public bool[] MyOffers { get; set; }
        public bool[] YourOffers { get; set; }

        public override MessageID ID => MessageID.ACCEPTTRADE;

        public override Message CreateInstance() => new ACCEPTTRADE();

        protected override void Read(NReader rdr)
        {
            MyOffers = new bool[rdr.ReadInt16()];
            for (int i = 0; i < MyOffers.Length; i++)
                MyOffers[i] = rdr.ReadBoolean();

            YourOffers = new bool[rdr.ReadInt16()];
            for (int i = 0; i < YourOffers.Length; i++)
                YourOffers[i] = rdr.ReadBoolean();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((ushort)MyOffers.Length);
            foreach (bool i in MyOffers)
                wtr.Write(i);
            wtr.Write((ushort)YourOffers.Length);
            foreach (bool i in YourOffers)
                wtr.Write(i);
        }
    }
}