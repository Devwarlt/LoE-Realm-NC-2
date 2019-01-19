#region

using LoESoft.Core;
using System;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class MARKET_RESULT : OutgoingMessage
    {
        public const int MARKET_ERROR = 0;
        public const int MARKET_SUCCESS = 1;
        public const int MARKET_REQUEST_RESULT = 2;

        public byte CommandId { get; set; }
        public string Message { get; set; }
        public PlayerShopItem[] Items { get; set; }

        public override MessageID ID => MessageID.MARKET_RESULT;

        public override Message CreateInstance() => new MARKET_RESULT();

        protected override void Read(NReader rdr)
        {
            throw new InvalidOperationException();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(CommandId);

            switch (CommandId)
            {
                case MARKET_ERROR:
                case MARKET_SUCCESS:
                    wtr.WriteUTF(Message);
                    break;

                case MARKET_REQUEST_RESULT:
                    wtr.Write(Items.Length);
                    foreach (var playerShopItem in Items)
                        playerShopItem.Write(wtr);
                    break;
            }
        }

        public static MARKET_RESULT Error(string errorMessage)
        {
            return new MARKET_RESULT
            {
                CommandId = MARKET_ERROR,
                Message = errorMessage
            };
        }

        public static MARKET_RESULT Success(string successMessage)
        {
            return new MARKET_RESULT
            {
                CommandId = MARKET_SUCCESS,
                Message = successMessage
            };
        }
    }
}