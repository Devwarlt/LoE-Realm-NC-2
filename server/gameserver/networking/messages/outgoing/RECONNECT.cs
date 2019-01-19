#region

using LoESoft.Core;

#endregion

namespace LoESoft.GameServer.networking.outgoing
{
    public class RECONNECT : OutgoingMessage
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int GameId { get; set; }
        public int KeyTime { get; set; }
        public bool IsFromArena { get; set; }
        public byte[] Key { get; set; }

        public override MessageID ID => MessageID.RECONNECT;

        public override Message CreateInstance() => new RECONNECT();

        protected override void Read(NReader rdr)
        {
            Name = rdr.ReadUTF();
            Host = rdr.ReadUTF();
            Port = rdr.ReadInt32();
            GameId = rdr.ReadInt32();
            KeyTime = rdr.ReadInt32();
            IsFromArena = rdr.ReadBoolean();
            Key = new byte[rdr.ReadInt16()];
            Key = rdr.ReadBytes(Key.Length);
        }

        protected override void Write(NWriter wtr)
        {
            wtr.WriteUTF(Name);
            wtr.WriteUTF(Host);
            wtr.Write(Port);
            wtr.Write(GameId);
            wtr.Write(KeyTime);
            wtr.Write(IsFromArena);
            wtr.Write((ushort)Key.Length);
            wtr.Write(Key);
        }
    }
}