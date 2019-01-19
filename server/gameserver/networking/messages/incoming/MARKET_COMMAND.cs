#region

using LoESoft.Core;
using System;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class MARKET_COMMAND : IncomingMessage
    {
        public int CommandId { get; set; }
        public uint OfferId { get; set; }
        public MarketOffer[] NewOffers { get; set; }

        private enum Command : int
        {
            REQUEST_MY_ITEMS = 0,
            ADD_OFFER = 1,
            REMOVE_OFFER = 2
        }

        public override MessageID ID => MessageID.MARKET_COMMAND;

        public override Message CreateInstance() => new MARKET_COMMAND();

        protected override void Read(NReader rdr)
        {
            CommandId = rdr.ReadInt32();

            switch (CommandId)
            {
                case (int)Command.ADD_OFFER:
                    NewOffers = new MarketOffer[rdr.ReadInt32()];
                    for (int i = 0; i < NewOffers.Length; i++)
                        NewOffers[i] = MarketOffer.Read(rdr);
                    break;

                case (int)Command.REMOVE_OFFER:
                    OfferId = rdr.ReadUInt32();
                    break;

                case (int)Command.REQUEST_MY_ITEMS:
                default:
                    break;
            }
        }

        protected override void Write(NWriter wtr)
        {
            throw new InvalidOperationException();
        }
    }
}