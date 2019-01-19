#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class TRADESTART : OutgoingMessage
    {
        public TradeItem[] MyItems { get; set; }
        public string YourName { get; set; }
        public TradeItem[] YourItems { get; set; }

        public override MessageID ID => MessageID.TRADESTART;

        public override Message CreateInstance() => new TRADESTART();

        protected override void Read(NReader rdr)
        {
            MyItems = new TradeItem[rdr.ReadInt16()];
            for (int i = 0; i < MyItems.Length; i++)
                MyItems[i] = TradeItem.Read(rdr);

            YourName = rdr.ReadUTF();
            YourItems = new TradeItem[rdr.ReadInt16()];
            for (int i = 0; i < YourItems.Length; i++)
                YourItems[i] = TradeItem.Read(rdr);
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((ushort)MyItems.Length);
            foreach (TradeItem i in MyItems)
                i.Write(wtr);

            wtr.WriteUTF(YourName);
            wtr.Write((ushort)YourItems.Length);
            foreach (TradeItem i in YourItems)
                i.Write(wtr);
        }
    }
}