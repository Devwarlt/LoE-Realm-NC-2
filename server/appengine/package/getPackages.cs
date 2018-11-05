#region

using log4net;
using System;
using System.IO;
using System.Xml;

#endregion

namespace LoESoft.AppEngine.package
{
    internal class getPackages : RequestHandler
    {
        protected override void HandleRequest()
        {
            string response = SerializePackageResponse.Serialize();
            using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                wtr.Write(response);
        }

        internal class SerializePackageResponse
        {
            internal static ILog log = LogManager.GetLogger(nameof(SerializePackageResponse));

            internal int PackageId { get; set; }
            internal string Name { get; set; }
            internal int Price { get; set; }
            internal int Quantity { get; set; }
            internal int MaxPurchase { get; set; }
            internal int Weight { get; set; }
            internal string BgURL { get; set; }
            internal DateTime EndDate { get; set; }
            internal string Contents { get; set; }

            internal static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
            {
                //Unix timestamp is seconds past epoch
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime;
            }

            internal static SerializePackageResponse GetPackage(int id)
            {
                XmlDocument doc = new XmlDocument();

                string response = File.ReadAllText("package/packageResponse.xml");

                if (response == null)
                    return null;

                doc.LoadXml(response);

                XmlNodeList packageResponse = doc.GetElementsByTagName("Package");

                if (packageResponse.Count > 0)
                {
                    for (int i = 0; i < packageResponse.Count; i++)
                    {
                        if (packageResponse[i].Attributes["id"].Value == $"{id}")
                        {
                            int packageid_ = id;
                            string name_ = packageResponse[i]["Name"].InnerText;
                            int price_ = Convert.ToInt32(packageResponse[i]["Price"].InnerText);
                            int quantity_ = Convert.ToInt32(packageResponse[i]["Quantity"].InnerText);
                            int maxpurchase_ = Convert.ToInt32(packageResponse[i]["MaxPurchase"].InnerText);
                            int weight_ = Convert.ToInt32(packageResponse[i]["Weight"].InnerText);
                            string bgurl_ = packageResponse[i]["BgURL"].InnerText;
                            DateTime enddate_ = UnixTimeStampToDateTime(Convert.ToDouble(packageResponse[i]["EndDate"].InnerText));
                            string contents_ = packageResponse[i]["Contents"].InnerText;
                            return new SerializePackageResponse
                            {
                                PackageId = packageid_,
                                Name = name_,
                                Price = price_,
                                Quantity = quantity_,
                                MaxPurchase = maxpurchase_,
                                Weight = weight_,
                                BgURL = bgurl_,
                                EndDate = enddate_,
                                Contents = contents_
                            };
                        }
                    }
                }
                return null;
            }

            internal static string Serialize()
            {
                XmlDocument doc = new XmlDocument();

                string response = File.ReadAllText("package/packageResponse.xml");

                if (response == null)
                    return null;

                doc.LoadXml(response);

                try
                {
                    StringWriter wtr = new StringWriter();
                    doc.Save(wtr);
                    return wtr.ToString();
                }
                catch (Exception error)
                {
                    log.Error($"Unhandle exception: {error}.");
                    return null;
                }
            }
        }
    }
}