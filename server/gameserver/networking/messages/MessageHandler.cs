using System;
using System.Collections.Generic;

namespace LoESoft.GameServer.networking
{
    internal class MessageHandler
    {
        public static Dictionary<MessageID, IMessage> Handlers = new Dictionary<MessageID, IMessage>();

        static MessageHandler()
        {
            foreach (Type i in typeof(Message).Assembly.GetTypes())
            {
                if (typeof(IMessage).IsAssignableFrom(i) &&
                    !i.IsAbstract && !i.IsInterface)
                {
                    IMessage pkt = (IMessage)Activator.CreateInstance(i);
                    Handlers.Add(pkt.ID, pkt);
                }
            }
        }
    }
}