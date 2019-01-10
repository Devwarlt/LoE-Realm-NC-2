#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class CLAIM_LOGIN_REWARD_MSG : IncomingMessage
    {
        public string ClaimKey { get; set; }
        public string Type { get; set; }

        public override MessageID ID => MessageID.CLAIM_LOGIN_REWARD_MSG;

        public override Message CreateInstance() => new CLAIM_LOGIN_REWARD_MSG();

        protected override void Read(NReader rdr)
        {
            ClaimKey = rdr.ReadUTF();
            Type = rdr.ReadUTF();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.WriteUTF(ClaimKey);
            wtr.WriteUTF(Type);
        }
    }
}
