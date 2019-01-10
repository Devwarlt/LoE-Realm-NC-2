#region

using LoESoft.Core;
using LoESoft.GameServer.networking.outgoing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

#endregion

namespace LoESoft.GameServer.networking
{
    public abstract class Message
    {
        public static Dictionary<MessageID, Message> Messages = new Dictionary<MessageID, Message>();

        static Message()
        {
            foreach (var i in typeof(Message).Assembly.GetTypes())
                if (typeof(Message).IsAssignableFrom(i) && !i.IsAbstract)
                {
                    var pkt = (Message)Activator.CreateInstance(i);

                    if (!(pkt is OutgoingMessage))
                        Messages.Add(pkt.ID, pkt);
                }
        }

        public abstract MessageID ID { get; }

        public abstract Message CreateInstance();

        public abstract void Crypt(Client client, byte[] dat, int offset, int len);

        public void Read(Client client, byte[] body, int offset, int len)
        {
            Crypt(client, body, offset, len);
            Read(new NReader(new MemoryStream(body)));
        }

        public int Write(Client client, byte[] buff, int offset)
        {
            MemoryStream s = new MemoryStream(buff, offset + 5, buff.Length - offset - 5);
            Write(new NWriter(s));

            int len = (int)s.Position;
            Crypt(client, buff, offset + 5, len);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len + 5)), 0, buff, offset, 4);
            buff[offset + 4] = (byte)ID;
            return len + 5;
        }

        public int EWrite(Client client, byte[] buff, int offset)
        {
            var s = new MemoryStream();
            var bodyLength = (int)s.Position;
            var messageLength = bodyLength + 5;

            if (messageLength > buff.Length - offset)
                return 0;

            Buffer.BlockCopy(s.GetBuffer(), 0, buff, offset + 5, bodyLength);

            Crypt(client, buff, offset + 5, bodyLength);

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(messageLength)), 0, buff, offset, 4);

            buff[offset + 4] = (byte)ID;

            return messageLength;
        }

        protected abstract void Read(NReader rdr);

        protected abstract void Write(NWriter wtr);

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder("{");
            PropertyInfo[] arr = GetType().GetProperties();
            for (int i = 0; i < arr.Length; i++)
            {
                if (i != 0)
                    ret.Append(", ");
                ret.AppendFormat("{0}: {1}", arr[i].Name, arr[i].GetValue(this, null));
            }
            ret.Append("}");
            return ret.ToString();
        }
    }
}