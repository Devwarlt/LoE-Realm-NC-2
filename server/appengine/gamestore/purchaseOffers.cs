using LoESoft.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoESoft.AppEngine.gamestore
{
    internal class purchaseOffers : RequestHandler
    {
        private class Offer
        {
            public int ObjectType { get; set; }
            public int Price { get; set; }
            public int CurrencyType { get; set; }
            public int Quantity { get; set; }
        }

        private enum Currency : int
        {
            INVALID = -1,
            GOLD = 0,
            FAME = 1,
            GUILD_FAME = 2,
            FORTUNE_TOKENS = 3,
            EMPIRES_COIN = 4
        }

        private int Credits(DbAccount acc) => acc.Credits;

        private double Fame(DbAccount acc) => acc.Fame;

        private int GuildFame(DbAccount acc) => acc.GuildFame;

        private int FortuneTokens(DbAccount acc) => acc.FortuneTokens;

        private int EmpiresCoin(DbAccount acc) => acc.EmpiresCoin;

        private bool Validate(DbAccount acc, int currency, int total)
        {
            switch ((Currency)currency)
            {
                case Currency.GOLD:
                    return Credits(acc) >= total;

                case Currency.FAME:
                    return Fame(acc) >= total;

                case Currency.GUILD_FAME:
                    return GuildFame(acc) >= total;

                case Currency.FORTUNE_TOKENS:
                    return FortuneTokens(acc) >= total;

                case Currency.EMPIRES_COIN:
                    return EmpiresCoin(acc) >= total;

                case Currency.INVALID:
                default:
                    return false;
            }
        }

        private string ProcessNames(List<int> objectType)
        {
            List<string> names = new List<string>();
            foreach (int i in objectType)
                names.Add($"{GameData.ObjectTypeToId[(ushort)i]}");
            return string.Join(", ", names.ToArray());
        }

        private string GetCurrency(int currency)
        {
            switch ((Currency)currency)
            {
                case Currency.GOLD:
                    return "credits";

                case Currency.FAME:
                    return "fame";

                case Currency.GUILD_FAME:
                    return "guild fame";

                case Currency.FORTUNE_TOKENS:
                    return "fortune tokens";

                case Currency.EMPIRES_COIN:
                    return "empires coin";

                case Currency.INVALID:
                default:
                    return "invalid";
            }
        }

        private void Deduct(DbAccount acc, int currency, int total, List<int> objectType)
        {
            switch ((Currency)currency)
            {
                case Currency.GOLD:
                    {
                        acc.Credits -= total;
                    }
                    break;

                case Currency.FAME:
                    {
                        acc.Fame -= total;
                        acc.TotalFame -= total;
                    }
                    break;

                case Currency.GUILD_FAME:
                    {
                        acc.GuildFame -= total;
                    }
                    break;

                case Currency.FORTUNE_TOKENS:
                    {
                        acc.FortuneTokens -= total;
                    }
                    break;

                case Currency.EMPIRES_COIN:
                    {
                        acc.EmpiresCoin -= total;
                    }
                    break;

                case Currency.INVALID:
                default:
                    return;
            }

            var items = acc.Gifts.ToList();

            foreach (int item in objectType)
                items.Add(item);

            acc.Gifts = items.ToArray();
            acc.FlushAsync();
            acc.Reload();
        }

        private string PurchaseStatus(DbAccount acc, int currency, int total, List<int> objectType)
        {
            bool validation = Validate(acc, currency, total);
            string offerNames = ProcessNames(objectType);
            if (validation)
            {
                Deduct(acc, currency, total, objectType);
                return $"You successfully purchased: {offerNames}.";
            }
            else
                return $"An error occured during your purchase validation and our system removed: {offerNames}.";
        }

        protected override void HandleRequest()
        {
            DbAccount acc;
            LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);

            using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                if (status == LoginStatus.OK)
                {
                    List<Offer> items = new List<Offer>();
                    List<int> total = new List<int> { 0, 0, 0, 0, 0 };
                    List<int> credits = new List<int>();
                    List<int> fame = new List<int>();
                    List<int> guild_fame = new List<int>();
                    List<int> fortune_tokens = new List<int>();
                    List<int> empires_coin = new List<int>();
                    List<List<int>> offers = new List<List<int>> { new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>() };
                    Queue<string> processingPayments = new Queue<string>();

                    foreach (string i in Query["purchasedItems"].Split('|').ToList())
                        items.Add(new Offer()
                        {
                            ObjectType = Convert.ToInt32(i.Split(',')[0]),
                            Price = Convert.ToInt32(i.Split(',')[1]),
                            CurrencyType = Convert.ToInt32(i.Split(',')[2]),
                            Quantity = Convert.ToInt32(i.Split(',')[3])
                        });

                    foreach (Offer j in items)
                        if (j.CurrencyType != -1)
                            total[j.CurrencyType] += j.Quantity * j.Price;

                    foreach (Offer k in items)
                    {
                        if (k.CurrencyType == (int)Currency.GOLD)
                            for (int l = 0; l < k.Quantity; l++)
                                credits.Add(k.ObjectType);
                        if (k.CurrencyType == (int)Currency.FAME)
                            for (int l = 0; l < k.Quantity; l++)
                                fame.Add(k.ObjectType);
                        if (k.CurrencyType == (int)Currency.GUILD_FAME)
                            for (int l = 0; l < k.Quantity; l++)
                                guild_fame.Add(k.ObjectType);
                        if (k.CurrencyType == (int)Currency.FORTUNE_TOKENS)
                            for (int l = 0; l < k.Quantity; l++)
                                fortune_tokens.Add(k.ObjectType);
                        if (k.CurrencyType == (int)Currency.EMPIRES_COIN)
                            for (int l = 0; l < k.Quantity; l++)
                                empires_coin.Add(k.ObjectType);
                    }

                    offers = new List<List<int>> { credits, fame, guild_fame, fortune_tokens, empires_coin };

                    for (int n = 0; n < total.Count; n++)
                        if (total[n] != 0)
                            processingPayments.Enqueue(PurchaseStatus(acc, n, total[n], offers[n]));

                    List<string> output = new List<string>();

                    while (processingPayments.Count > 0)
                        output.Add($"{processingPayments.Dequeue()}");

                    wtr.Write($"~#{string.Join("|", output.ToArray())}");
                }
        }
    }
}