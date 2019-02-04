#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class UPDATE : OutgoingMessage
    {
        public TileData[] Tiles { get; set; }
        public ObjectDef[] NewObjects { get; set; }
        public int[] RemovedObjectIds { get; set; }

        public override MessageID ID => MessageID.UPDATE;

        public override Message CreateInstance() => new UPDATE();

        protected override void Read(NReader rdr)
        {
            Tiles = new TileData[rdr.ReadInt16()];
            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = new TileData
                {
                    X = rdr.ReadInt16(),
                    Y = rdr.ReadInt16(),
                    Tile = rdr.ReadUInt16()
                };
            }

            NewObjects = new ObjectDef[rdr.ReadInt16()];
            for (int i = 0; i < NewObjects.Length; i++)
                NewObjects[i] = ObjectDef.Read(rdr);

            RemovedObjectIds = new int[rdr.ReadInt16()];
            for (int i = 0; i < RemovedObjectIds.Length; i++)
                RemovedObjectIds[i] = rdr.ReadInt32();
        }

        protected override void Write(NWriter wtr)
        {
            wtr.Write((short)Tiles.Length);
            foreach (TileData i in Tiles)
            {
                wtr.Write(i.X);
                wtr.Write(i.Y);
                wtr.Write((short)i.Tile);
            }
            wtr.Write((short)NewObjects.Length);
            foreach (ObjectDef i in NewObjects)
                i.Write(wtr);

            wtr.Write((short)RemovedObjectIds.Length);
            foreach (int i in RemovedObjectIds)
                wtr.Write(i);
        }

        public struct TileData
        {
            public int Tile;
            public short X;
            public short Y;
        }
    }
}