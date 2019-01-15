#region

using LoESoft.Core;
using System;
using System.Linq;

#endregion

namespace LoESoft.AppEngine.account
{
    // guid, password, name
    // <Error>Error.accountNotFound</Error>
    // <Error>Error.nameTooShort</Error>
    // <Error>Error.nameIsNotAlpha</Error>
    // <Error>Error.nameTooLong</Error>
    // <Error>Error.nameAlreadyInUse</Error>
    // <Error>Error.notEnoughGold</Error>
    internal class setName : RequestHandler
    {
        protected override void HandleRequest()
        {
            string name = Query["name"];
            if (name.Length < 3 || name.Length > 15 || !name.All(x => char.IsLetter(x) || char.IsNumber(x)))
                WriteErrorLine("Invalid name");
            else
            {
                foreach (string i in Database.BlackListedNames)
                    if (name.ToLower() == i.ToLower())
                    {
                        WriteErrorLine("Invalid name");
                        return;
                    }
                string key = Database.NAME_LOCK;
                string lockToken = null;
                try
                {
                    while ((lockToken = Database.AcquireLock(key)) == null)
                        ;

                    if (Database.Conn.HashExists("names", name.ToUpperInvariant()))
                    {
                        WriteErrorLine("Duplicated name");
                        return;
                    }

                    DbAccount acc;
                    if (Query["guid"] == null || Query["password"] == null)
                    {
                        WriteErrorLine("Error.incorrectEmailOrPassword");
                        return;
                    }
                    LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);
                    if (status == LoginStatus.OK)
                    {
                        using (IDisposable l = Database.Lock(acc))
                            if (Database.LockOk(l))
                            {
                                if (acc.NameChosen && acc.Credits < 1000)
                                    WriteErrorLine("Not enough credits");
                                else
                                {
                                    if (acc.NameChosen)
                                        Database.UpdateCredit(acc, -1000);
                                    while (!Database.RenameIGN(acc, name, lockToken))
                                        ;
                                    WriteLine("<Success />");
                                }
                            }
                            else
                                WriteErrorLine("Account in use");
                    }
                    else
                        WriteErrorLine(status.GetInfo());
                }
                finally
                {
                    if (lockToken != null)
                        Database.ReleaseLock(key, lockToken);
                }
            }
        }
    }
}