#region

using LoESoft.Core;
using System;

#endregion

namespace LoESoft.AppEngine.account
{
    internal class purchaseCharSlot : RequestHandler
    {
        // guid, password
        // <Error>Not enough Gold</Error>
        protected override void HandleRequest()
        {
            DbAccount acc;
            if (Query["guid"] == null || Query["password"] == null)
                WriteErrorLine("Error.incorrectEmailOrPassword");
            else
            {
                LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);
                if (status == LoginStatus.OK)
                {
                    using (IDisposable l = Database.Lock(acc))
                    {
                        if (!Database.LockOk(l))
                        {
                            WriteErrorLine("Account in use");
                            return;
                        }
                        else if (acc.Credits < 100)
                        {
                            WriteErrorLine("Not enough Gold");
                            return;
                        }
                        Database.UpdateCredit(acc, -100); //should be rebuilded
                        acc.MaxCharSlot++;
                        acc.Flush();
                    }
                    WriteLine("<Success />");
                }
                else
                    WriteErrorLine(status.GetInfo());
            }
        }
    }
}