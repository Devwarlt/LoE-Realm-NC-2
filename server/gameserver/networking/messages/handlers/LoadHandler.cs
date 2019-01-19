#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using static LoESoft.GameServer.networking.Client;
using FAILURE = LoESoft.GameServer.networking.outgoing.FAILURE;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class LoadHandler : MessageHandlers<LOAD>
    {
        public override MessageID ID => MessageID.LOAD;

        protected override void HandleMessage(Client client, LOAD message)
        {
            client.Character = GameServer.Manager.Database.LoadCharacter(client.Account, message.CharacterId);
            if (client.Character != null)
            {
                if (client.Character.Dead)
                {
                    client.SendMessage(new FAILURE
                    {
                        ErrorId = (int)FailureIDs.DEFAULT,
                        ErrorDescription = "Character is dead."
                    });
                    GameServer.Manager.TryDisconnect(client, DisconnectReason.CHARACTER_IS_DEAD);
                }
                else
                {
                    var target = GameServer.Manager.Worlds[client.TargetWorld];

                    client.SendMessage(new CREATE_SUCCESS
                    {
                        CharacterID = client.Character.CharId,
                        ObjectID = GameServer.Manager.Worlds[client.TargetWorld].EnterWorld(client.Player = new Player(client))
                    });
                    client.State = ProtocolState.Ready;
                }
            }
            else
            {
                client.SendMessage(new FAILURE
                {
                    ErrorId = (int)FailureIDs.DEFAULT,
                    ErrorDescription = "Failed to Load character."
                });
                GameServer.Manager.TryDisconnect(client, DisconnectReason.FAILED_TO_LOAD_CHARACTER);
            }
        }
    }
}