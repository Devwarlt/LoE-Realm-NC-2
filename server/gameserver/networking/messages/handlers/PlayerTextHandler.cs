#region

using LoESoft.GameServer.networking.incoming;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class PlayerTextHandler : MessageHandlers<PLAYERTEXT>
    {
        public override MessageID ID => MessageID.PLAYERTEXT;

        protected override void HandleMessage(Client client, PLAYERTEXT message)
        {
            if (client.Player.Owner == null)
                return;

            if (message.Text[0] == '/')
                GameServer.Manager.Commands.Execute(client.Player, Manager.Logic.GameTime, message.Text);
            else
            {
                if (!client.Player.NameChosen)
                {
                    client.Player.SendInfo("{\"key\":\"server.must_be_named\"}");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(message.Text))
                    GameServer.Manager.Chat.Say(client.Player, message.Text);
                else
                    client.Player.SendInfo("{\"key\":\"server.invalid_chars\"}");
            }
        }
    }
}