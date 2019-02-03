#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace LoESoft.AppEngine.@char
{
    internal class list : RequestHandler
    {
        protected override void HandleRequest()
        {
            try
            {
                var status = Database.Verify(Query["guid"], Query["password"], out DbAccount acc);

                if (status == LoginStatus.OK || status == LoginStatus.AccountNotExists)
                {
                    if (status == LoginStatus.AccountNotExists)
                        acc = Database.CreateGuestAccount(Query["guid"]);

                    if (acc.Banned)
                    {
                        using (var wtr = new StreamWriter(Context.Response.OutputStream))
                            wtr.WriteLine("<Error>Account under maintenance</Error>");

                        Context.Response.Close();
                    }

                    var ca = new DbClassAvailability(acc);
                    if (ca.IsNull)
                        ca.Init(GameData);
                    ca.FlushAsync();

                    var list = CharList.FromDb(Database, acc);
                    list.Servers = GetServerList();

                    WriteLine(list.ToXml(AppEngine.GameData, acc));
                }
                else
                    WriteErrorLine(status.GetInfo());
            }
            catch (Exception e) { Log.Error(e.ToString()); }
        }

        private Lazy<List<Settings.APPENGINE.ServerItem>> SvrList { get; set; }

        public list()
        {
            SvrList = new Lazy<List<Settings.APPENGINE.ServerItem>>(GetServerList, true);
        }

        private List<Settings.APPENGINE.ServerItem> GetServerList()
        {
            /*var tcpclient = new TcpClient();
            tcpclient.Client.NoDelay = true;
            tcpclient.Client.UseOnlyOverlappedIO = true;
            tcpclient.Client.Ttl = 112;
            tcpclient.Client.SendTimeout = 1000;
            tcpclient.Client.ReceiveTimeout = 1000;*/

            var ret = Settings.APPENGINE.GetServerItem(null);
            return ret;
        }
    }
}