#region

using BookSleeve;
using Newtonsoft.Json;
using System;
using System.Text;

#endregion

namespace LoESoft.Core
{
    public class InterServerEventArgs<T> : EventArgs
    {
        public InterServerEventArgs(string instId, T val)
        {
            InstanceId = instId;
            Content = val;
        }

        public string InstanceId { get; private set; }
        public T Content { get; private set; }
    }

    public class InterServerChannel
    {
        public string InstanceId { get; private set; }
        public Database Database { get; private set; }

        private RedisSubscriberConnection conn;

        public InterServerChannel(Database db, string instId)
        {
            Database = db;
            conn = db.Connection.GetOpenSubscriberChannel();
            InstanceId = instId;
        }

        private struct Message<T> where T : struct
        {
            public string InstId { get; set; }
            public string TargetInst { get; set; }
            public T Content { get; set; }
        }

        public void Publish<T>(string channel, T val, string target = null) where T : struct
        {
            Message<T> message = new Message<T>()
            {
                InstId = InstanceId,
                TargetInst = target,
                Content = val
            };
            try
            { Database.Connection.Publish(channel, JsonConvert.SerializeObject(message)); }
            catch { }
        }

        public void AddHandler<T>(string channel, EventHandler<InterServerEventArgs<T>> handler) where T : struct
        {
            conn.Subscribe(channel, (s, buff) =>
            {
                var message = JsonConvert.DeserializeObject<Message<T>>(Encoding.UTF8.GetString(buff));
                if (message.TargetInst != null &&
                    message.TargetInst != InstanceId)
                    return;
                handler(this, new InterServerEventArgs<T>(message.InstId, message.Content));
            });
        }
    }
}