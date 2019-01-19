#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class CHANGETRADE : IncomingMessage
    {
        public bool[] Offers { get; set; }

        public override MessageID ID => MessageID.CHANGETRADE;

        public override Message CreateInstance() => new CHANGETRADE();

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