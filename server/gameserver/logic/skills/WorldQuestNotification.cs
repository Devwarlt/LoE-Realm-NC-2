#region

using CA.Extensions.Concurrent;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class WorldQuestNotification : Behavior
    {
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
        }

        private bool once { get; set; }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (once)
                return;

            once = true;

            var clients = GameServer.Manager.GetManager.Clients.ValueWhereAsParallel(_ => _ != null && _.Player != null);
            for (var i = 0; i < clients.Length; i++)
                clients[i].SendMessage(new TEXT
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@ANNOUNCEMENT",
                    Text = $" <World Quest> LoE Realm need your help! {host.Name} has been spawned at {host.Owner.Name}!",
                    NameColor = 0x123456,
                    TextColor = 0x123456
                });
        }
    }
}
