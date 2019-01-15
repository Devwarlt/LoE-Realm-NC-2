#region

using LoESoft.Core;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using System.Linq;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class ChooseNameHandler : MessageHandlers<CHOOSENAME>
    {
        public override MessageID ID => MessageID.CHOOSENAME;

        protected override void HandleMessage(Client client, CHOOSENAME message) => Handle(client, message);

        protected void Handle(Client client, CHOOSENAME message)
        {
            string name = message.Name;
            if (name.Length < 3 || name.Length > 15 || !name.All(x => char.IsLetter(x) || char.IsNumber(x)))
                client.SendMessage(new NAMERESULT
                {
                    Success = false,
                    ErrorText = "Error.nameIsNotAlpha"
                });
            else
            {
                string key = Database.NAME_LOCK;
                string lockToken = null;
                try
                {
                    while ((lockToken = Manager.Database.AcquireLock(key)) == null)
                        ;

                    if (Manager.Database.Conn.HashExists("names", name.ToUpperInvariant()))
                    {
                        client.SendMessage(new NAMERESULT
                        {
                            Success = false,
                            ErrorText = "Error.nameAlreadyInUse"
                        });
                        return;
                    }

                    if (client.Account.NameChosen && client.Account.Credits < 1000)
                        client.SendMessage(new NAMERESULT
                        {
                            Success = false,
                            ErrorText = "server.not_enough_gold"
                        });
                    else
                    {
                        if (client.Account.NameChosen)
                            Manager.Database.UpdateCredit(client.Account, -1000);
                        while (!Manager.Database.RenameIGN(client.Account, name, lockToken))
                            ;
                        client.Player.Name = client.Account.Name;
                        client.Player.UpdateCount++;
                        client.SendMessage(new NAMERESULT
                        {
                            Success = true,
                            ErrorText = "server.buy_success"
                        });
                    }
                }
                finally
                {
                    if (lockToken != null)
                        Manager.Database.ReleaseLock(key, lockToken);
                }
            }
        }
    }
}