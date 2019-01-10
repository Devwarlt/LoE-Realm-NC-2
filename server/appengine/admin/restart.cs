#region

using LoESoft.Core;
using LoESoft.Core.config;
using System.Diagnostics;

#endregion

namespace LoESoft.AppEngine.account
{
    internal class restart : RequestHandler
    {
        protected override void HandleRequest()
        {
            DbAccount acc;

            if (Query["guid"] == null || Query["password"] == null)
                WriteErrorLine("Access denied!");
            else
            {
                var status = Database.Verify(Query["guid"], Query["password"], out acc);

                if (status == LoginStatus.OK)
                {
                    if (acc.AccountType >= (int)AccountType.DEVELOPER)
                    {
                        var result = Settings.APPENGINE.SafeRestart(
                            Settings.SERVER_MODE == Settings.ServerMode.Production ?
                            Settings.NETWORKING.INTERNAL.PRODUCTION_DDNS[0] : "localhost",
                            2050);
                        if (result == "Success")
                            WriteLine("You have been restarted the server, please wait...");
                        else if (result == null)
                            Process.Start(Settings.GAMESERVER.FILE);
                        else
                        {
                            if (Query["now"] == null)
                                WriteLine(result);
                            else
                                Process.Start(Settings.GAMESERVER.FILE);
                        }
                    }
                }
                else
                    WriteErrorLine(status.GetInfo());
            }
        }
    }
}