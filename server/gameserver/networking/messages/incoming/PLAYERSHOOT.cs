#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.incoming
{
    public class PLAYERSHOOT : IncomingMessage
    {
        public byte BulletId { get; set; }
        public short ContainerType { get; set; }
        public Position Position { get; set; }
        public float Angle { get; set; }
        public int AttackAmount { get; set; }
        public bool IsDazed { get; set; }
        public bool IsBeserk { get; set; }
        public float MinAttackFrequency { get; set; }
        public float MaxAttackFrequency { get; set; }
        public float WeaponRateOfFire { get; set; }
        public bool IsAbility { get; set; }

        public override MessageID ID => MessageID.PLAYERSHOOT;

        public override Message CreateInstance() => new PLAYERSHOOT();

        protected override void Read(NReader rdr)
        {
            BulletId = rdr.ReadByte();
            ContainerType = rdr.ReadInt16();
            Position = Position.Read(rdr);
            Angle = rdr.ReadSingle();
            AttackAmount = rdr.ReadInt32();
            IsDazed = rdr.ReadBoolean();
            IsBeserk = rdr.ReadBoolean();
            MinAttackFrequency = rdr.ReadSingle();
            MaxAttackFrequency = rdr.ReadSingle();
            WeaponRateOfFire = rdr.ReadSingle();
            IsAbility = rdr.ReadBoolean();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(BulletId);
            wtr.Write(ContainerType);
            Position.Write(wtr);
            wtr.Write(Angle);
            wtr.Write(AttackAmount);
            wtr.Write(IsDazed);
            wtr.Write(IsBeserk);
            wtr.Write(MinAttackFrequency);
            wtr.Write(MaxAttackFrequency);
            wtr.Write(WeaponRateOfFire);
            wtr.Write(IsAbility);
        }
    }
}