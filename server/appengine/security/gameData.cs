using LoESoft.Core.config;
using System.Collections.Generic;

namespace LoESoft.AppEngine.security
{
    internal class gameData : RequestHandler
    {
        private List<string> AllowedCapabilities => new List<string>() { "ActiveX", "PlugIn", "StandAlone" };

        private bool VerifyDomain(string domain) => domain.Contains("http://loe-nc.servegame.com/");

        protected override void HandleRequest()
        {
            if (Settings.SERVER_MODE == Settings.ServerMode.Production)
            {
                var capability = Query["capability"];
                var domain = Query["domain"];
                var isnullcap = string.IsNullOrWhiteSpace(capability);
                var isnulldom = string.IsNullOrWhiteSpace(domain);

                if (isnullcap && isnulldom)
                {
                    SendGDError(GameDataErrors.NullCapabilityAndDomain);
                    return;
                }

                if (string.IsNullOrWhiteSpace(capability))
                {
                    SendGDError(GameDataErrors.NullCapability);
                    return;
                }

                if (string.IsNullOrWhiteSpace(domain))
                {
                    SendGDError(GameDataErrors.NullDomain);
                    return;
                }

                if (!AllowedCapabilities.Contains(capability))
                {
                    SendGDError(GameDataErrors.InvalidCapability);
                    return;
                }

                if (!VerifyDomain(domain))
                {
                    SendGDError(GameDataErrors.InvalidDomain);
                    return;
                }

                var ip = Context.Request.RemoteEndPoint.Address.ToString();

                if (!Manager.CheckWebClient(ip))
                    Manager.AddWebClient(ip, false);
                else
                    Manager.UpdateWebClient(ip, false);
            }

            WriteLine("<Success/>");
        }
    }
}