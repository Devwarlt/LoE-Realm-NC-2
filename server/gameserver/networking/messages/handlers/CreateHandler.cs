#region

using LoESoft.Core;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using System.Linq;
using static LoESoft.GameServer.networking.Client;
using FAILURE = LoESoft.GameServer.networking.outgoing.FAILURE;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class CreateHandler : MessageHandlers<CREATE>
    {
        public override MessageID ID => MessageID.CREATE;

        protected override void HandleMessage(Client client, CREATE message) => Handle(client, message);

        private void Handle(Client client, CREATE message)
        {
            int skin = client.Account.OwnedSkins.Contains(message.SkinType) ? message.SkinType : 0;

            var status = GameServer.Manager.Database.CreateCharacter(GameServer.Manager.GameData, client.Account, (ushort)message.ClassType, skin, out DbChar character);

            if (status == CreateStatus.ReachCharLimit)
            {
                client.SendMessage(new FAILURE
                {
                    ErrorDescription = "Failed to Load character."
                });
                GameServer.Manager.TryDisconnect(client, DisconnectReason.FAILED_TO_LOAD_CHARACTER);
                return;
            }
            client.Character = character;

            if (status == CreateStatus.OK)
            {
                client.SendMessage(new CREATE_SUCCESS
                {
                    CharacterID = client.Character.CharId,
                    ObjectID = GameServer.Manager.Worlds[client.TargetWorld].EnterWorld(client.Player = new Player(client))
                });

                client.State = ProtocolState.Ready;
            }
            else
            {
                client.SendMessage(new FAILURE
                {
                    ErrorDescription = "Failed to Load character."
                });
            }
        }
    }
}