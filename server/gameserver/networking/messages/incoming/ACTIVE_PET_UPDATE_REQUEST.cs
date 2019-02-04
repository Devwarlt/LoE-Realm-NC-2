#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class ACTIVE_PET_UPDATE_REQUEST : IncomingMessage
    {
        public const int FOLLOW_PET = 1;
        public const int UNFOLLOW_PET = 2;
        public const int RELEASE_PET = 3;

        public int CommandId { get; set; }
        public uint PetId { get; set; }

        public override MessageID ID => MessageID.ACTIVE_PET_UPDATE_REQUEST;

        public override Message CreateInstance() => new ACTIVE_PET_UPDATE_REQUEST();

        protected override void Read(NReader rdr)
        {
            CommandId = rdr.ReadByte();
            PetId = (uint)rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((byte)CommandId);
            wtr.Write((int)PetId);
        }
    }
}