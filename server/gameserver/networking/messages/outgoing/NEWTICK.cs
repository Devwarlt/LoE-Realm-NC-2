#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class NEWTICK : OutgoingMessage
    {
        public int TickId { get; set; }
        public int TickTime { get; set; }
        public ObjectStatusData[] Statuses { get; set; }

        public override MessageID ID => MessageID.NEWTICK;

        public override Message CreateInstance() => new NEWTICK();

        protected override void Read(NReader rdr)
        {
            TickId = rdr.ReadInt32();
            TickTime = rdr.ReadInt32();

            Statuses = new ObjectStatusData[rdr.ReadInt16()];
            for (int i = 0; i < Statuses.Length; i++)
                Statuses[i] = ObjectStatusData.Read(rdr);
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write(TickId);
            wtr.Write(TickTime);

            wtr.Write((ushort)Statuses.Length);
            foreach (ObjectStatusData i in Statuses)
                i.Write(wtr);
        }
    }
}