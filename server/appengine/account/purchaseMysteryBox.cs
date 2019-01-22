#region

using LoESoft.AppEngine.mysterybox;
using LoESoft.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

#endregion

namespace LoESoft.AppEngine.account
{
    internal class purchaseMysteryBox : RequestHandler
    {
        private Random rand;

        protected override void HandleRequest()
        {
            rand = Query["ignore"] != null ? new Random(int.Parse(Query["ignore"])) : new Random();

            if (Query["guid"] == null || Query["password"] == null)
                WriteErrorLine("Error.incorrectEmailOrPassword");
            else
            {
                DbAccount acc;
                LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);

                if (status == LoginStatus.OK)
                {
                    if (Query["boxId"] == null)
                    {
                        WriteLine("<Error>Box ID not declared.</Error>");
                        return;
                    }

                    SerializeMiniGames box = SerializeMiniGames.GetBox(int.Parse(Query["boxId"]), CONSTANTS.MYSTERY_BOX);

                    if (box == null)
                    {
                        WriteLine($"<Error>Box ID {Query["boxId"]} not found.</Error>");
                        return;
                    }

                    if (box.Sale != null && DateTime.UtcNow <= box.Sale.SaleEnd)
                    {
                        switch (box.Sale.Currency)
                        {
                            case CONSTANTS.GOLD:
                                if (acc.Credits < box.Sale.Price)
                                {
                                    WriteLine("<Error>Not enough gold.</Error>");
                                    return;
                                }
                                break;

                            case CONSTANTS.FAME:
                                if (acc.Fame < box.Sale.Price)
                                {
                                    WriteLine("<Error>Not enough fame.</Error>");
                                    return;
                                }
                                break;

                            default:
                                {
                                    WriteLine($"<Error>Invalid currency type ID {box.Sale.Currency}.</Error>");
                                }
                                return;
                        }
                    }
                    else
                    {
                        switch (box.PriceMB.Currency)
                        {
                            case CONSTANTS.GOLD:
                                if (acc.Credits < box.PriceMB.Amount)
                                {
                                    WriteLine("<Error>Not enough gold.</Error>");
                                    return;
                                }
                                break;

                            case CONSTANTS.FAME:
                                if (acc.Fame < box.PriceMB.Amount)
                                {
                                    WriteLine("<Error>Not enough fame.</Error>");
                                    return;
                                }
                                break;

                            default:
                                {
                                    WriteLine($"<Error>Invalid currency type ID {box.PriceMB.Currency}.</Error>");
                                }
                                return;
                        }
                    }

                    if (Database.CheckMysteryBox(acc, box.BoxId, box.Total))
                    {
                        WriteLine($"<Error>You can only purchase {box.Title} {box.Total} time{((box.Total <= 1) ? "" : "s")}.</Error>");
                        return;
                    }

                    if (box.Total != -1)
                        Database.AddMysteryBox(acc, box.BoxId);

                    MysteryBoxResult res = new MysteryBoxResult
                    {
                        Awards = Utils.GetCommaSepString(GetAwards(box.Contents))
                    };

                    if (box.Sale != null && DateTime.UtcNow <= box.Sale.SaleEnd)
                    {
                        switch (box.Sale.Currency)
                        {
                            case CONSTANTS.GOLD:
                                {
                                    Database.UpdateCredit(acc, -box.Sale.Price);
                                    res.GoldLeft = acc.Credits;
                                }
                                break;

                            case CONSTANTS.FAME:
                                {
                                    Database.UpdateFame(acc, -box.Sale.Price);
                                    res.GoldLeft = acc.Fame;
                                }
                                break;

                            default:
                                {
                                    WriteLine($"<Error>Invalid currency type ID {box.Sale.Currency}.</Error>");
                                }
                                return;
                        }
                    }
                    else
                    {
                        switch (box.PriceMB.Currency)
                        {
                            case CONSTANTS.GOLD:
                                {
                                    Database.UpdateCredit(acc, box.PriceMB.Amount < 0 ? 0 : -box.PriceMB.Amount);
                                    res.GoldLeft = acc.Credits;
                                }
                                break;

                            case CONSTANTS.FAME:
                                {
                                    Database.UpdateFame(acc, box.PriceMB.Amount < 0 ? 0 : -box.PriceMB.Amount);
                                    res.GoldLeft = acc.Fame;
                                }
                                break;

                            default:
                                {
                                    WriteLine($"<Error>Invalid currency type ID {box.PriceMB.Currency}.</Error>");
                                }
                                return;
                        }
                    }
                    if (box.Sale != null && DateTime.UtcNow <= box.Sale.SaleEnd)
                        res.Currency = box.Sale.Currency;
                    else
                        res.Currency = box.PriceMB.Currency;

                    sendMysteryBoxResult(Context.Response.OutputStream, res);

                    int[] gifts = Utils.FromCommaSepString32(res.Awards);

                    List<int> giftsList = acc.Gifts.ToList();

                    foreach (int item in gifts)
                        giftsList.Add(item);

                    acc.Gifts = giftsList.ToArray();

                    acc.FlushAsync();
                    acc.Reload();
                }
                else
                    WriteLine("<Error>Account not found</Error>");
            }
        }

        #region "Process package awards"

        private int[] GetAwards(string items)
        {
            int[] ret = new int[items.Split(';').Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = Utils.FromString(items.Split(';')[0].Split(',')[rand.Next(items.Split(';')[0].Split(',').Length)]);
            return ret.ToArray();
        }

        #endregion "Process package awards"

        #region "Send package result"

        private void sendMysteryBoxResult(Stream stream, MysteryBoxResult res)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode success = doc.CreateElement("Success");
            doc.AppendChild(success);

            XmlNode awards = doc.CreateElement("Awards");
            awards.InnerText = res.Awards.Replace(" ", string.Empty);
            success.AppendChild(awards);

            XmlNode goldLeft = doc.CreateElement(res.Currency == 0 ? "Gold" : "Fame");
            goldLeft.InnerText = res.GoldLeft.ToString();
            success.AppendChild(goldLeft);

            StringWriter wtr = new StringWriter();
            doc.Save(wtr);
            using (StreamWriter output = new StreamWriter(stream))
                output.WriteLine(wtr.ToString());
        }

        #endregion "Send package result"

        #region "Package result structure"

        private class MysteryBoxResult
        {
            public string Awards { get; set; }
            public double GoldLeft { get; set; }
            public int Currency { get; set; }
        }

        #endregion "Package result structure"
    }
}