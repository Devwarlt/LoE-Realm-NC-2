#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    internal class LOGIN_REWARD_MSG : OutgoingMessage
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int Gold { get; set; }

        public override MessageID ID => MessageID.LOGIN_REWARD_MSG;

        public override Message CreateInstance() => new LOGIN_REWARD_MSG();

        protected override void Read(NReader rdr)
        {
            ItemId = rdr.ReadInt32();
            Quantity = rdr.ReadInt32();
            Gold = rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(ItemId);
            wtr.Write(Quantity);
            wtr.Write(Gold);
        }
    }
}