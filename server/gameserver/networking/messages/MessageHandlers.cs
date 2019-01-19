using LoESoft.GameServer.networking.incoming;
using log4net;

namespace LoESoft.GameServer.networking
{
    internal abstract class MessageHandlers<T> : IMessage where T : IncomingMessage
    {
        protected ILog log4net;

        public Client Client { get; private set; }

        public abstract MessageID ID { get; }

        public MessageHandlers()
        {
            log4net = LogManager.GetLogger(GetType());
        }

        protected abstract void HandleMessage(Client client, T message);

        public void Handle(Client client, IncomingMessage message)
        {
            Client = client;

            HandleMessage(client, (T)message);
        }

        protected void SendFailure(string text) => Client.SendMessage(new FAILURE { ErrorId = 0, ErrorDescription = text });

        public void NotImplementedMessageHandler()
        {
            return;
        }
    }
}