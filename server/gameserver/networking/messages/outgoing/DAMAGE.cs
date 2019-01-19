#region

using LoESoft.Core;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class DAMAGE : OutgoingMessage
    {
        public int TargetId { get; set; }
        public ConditionEffects Effects { get; set; }
        public ushort Damage { get; set; }
        public bool Killed { get; set; }
        public byte BulletId { get; set; }
        public int ObjectId { get; set; }

        public override MessageID ID => MessageID.DAMAGE;

        public override Message CreateInstance() => new DAMAGE();

        protected override void Read(NReader rdr)
        {
            TargetId = rdr.ReadInt32();
            byte c = rdr.ReadByte();
            Effects = 0;
            for (int i = 0; i < c; i++)
                Effects |= (ConditionEffects)(1 << rdr.ReadByte());
            Damage = rdr.ReadUInt16();
            Killed = rdr.ReadBoolean();
            BulletId = rdr.ReadByte();
            ObjectId = rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(TargetId);
            List<byte> eff = new List<byte>();
            for (byte i = 1; i < 255; i++)
                if ((Effects & (ConditionEffects)(1 << i)) != 0)
                    eff.Add(i);
            wtr.Write((byte)eff.Count);
            foreach (byte i in eff)
                wtr.Write(i);
            wtr.Write(Damage);
            wtr.Write(Killed);
            wtr.Write(BulletId);
            wtr.Write(ObjectId);
        }
    }
}