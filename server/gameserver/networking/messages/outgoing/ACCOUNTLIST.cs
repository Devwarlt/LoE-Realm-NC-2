#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class ACCOUNTLIST : OutgoingMessage
    {
        public const int LOCKED_LIST_ID = 0;
        public const int IGNORED_LIST_ID = 1;

        public int AccountListId { get; set; }
        public string[] AccountIds { get; set; }
        public int LockAction { get; set; }

        public override MessageID ID => MessageID.ACCOUNTLIST;

        public override Message CreateInstance() => new ACCOUNTLIST();

        protected override void Read(NReader rdr)
        {
            AccountListId = rdr.ReadInt32();
            AccountIds = new string[rdr.ReadInt16()];
            for (int i = 0; i < AccountIds.Length; i++)
                AccountIds[i] = rdr.ReadUTF();
            LockAction = rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(AccountListId);
            wtr.Write((ushort)AccountIds.Length);
            foreach (string i in AccountIds)
                wtr.WriteUTF(i);
            wtr.Write(LockAction);
        }
    }
}