#region

using LoESoft.Core;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class PETUPGRADEREQUEST : IncomingMessage
    {
        public const int UPGRADE_PET_YARD = 1;
        public const int FEED_PET = 2;
        public const int FUSE_PET = 3;

        public byte CommandId { get; set; }
        public int PetId1 { get; set; }
        public int PetId2 { get; set; }
        public int ObjectId { get; set; }
        public ObjectSlot ObjectSlot { get; set; }
        public CurrencyType Currency { get; set; }

        public override MessageID ID => MessageID.PETUPGRADEREQUEST;

        public override Message CreateInstance() => new PETUPGRADEREQUEST();

        protected override void Read(NReader rdr)
        {
            CommandId = rdr.ReadByte();
            PetId1 = rdr.ReadInt32();
            PetId2 = rdr.ReadInt32();
            ObjectId = rdr.ReadInt32();
            ObjectSlot = ObjectSlot.Read(rdr);
            Currency = (CurrencyType)rdr.ReadByte();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(CommandId);
            wtr.Write(PetId1);
            wtr.Write(PetId2);
            wtr.Write(ObjectId);
            ObjectSlot.Write(wtr);
            wtr.Write((byte)Currency);
        }
    }
}