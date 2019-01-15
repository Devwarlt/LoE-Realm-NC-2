#region

using LoESoft.AppEngine.mysterybox;
using LoESoft.Core;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.AppEngine.account
{
    internal class playFortuneGame : RequestHandler
    {
        private static Dictionary<string, string[]> CurrentGames = new Dictionary<string, string[]>();
        private Random rand;
        private int currency;
        private int rows;
        private int price;
        private string data;

        protected override void HandleRequest()
        {
            rand = Query["ignore"] != null ? new Random(int.Parse(Query["ignore"])) : new Random();

            if (Query["guid"] == null || Query["password"] == null)
                WriteErrorLine("Error.incorrectEmailOrPassword");
            else
            {
                DbAccount acc;
                LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);

                currency = -1;
                int.TryParse(Query["currency"], out currency);
                rows = -1;
                int.TryParse(Query["status"], out rows);
                price = -1;
                data = null;

                if (status == LoginStatus.OK)
                {
                    List<int> giftsList = acc.Gifts.ToList();
                    int[] gifts = null;
                    List<string> candidates = new List<string>(3);

                    if (Query["gameId"] == null)
                    {
                        WriteLine("<Error>Fortune Game ID not declared.</Error>");
                        return;
                    }

                    SerializeMiniGames box = SerializeMiniGames.GetBox(int.Parse(Query["gameId"]), CONSTANTS.FORTUNE_GAME);

                    if (box == null)
                    {
                        WriteLine($"<Error>Fortune Game ID {Query["gameId"]} not found.</Error>");
                        return;
                    }

                    switch (currency)
                    {
                        case CONSTANTS.GOLD:
                            if (acc.Credits < box.PriceFG.firstInGold || acc.Credits < box.PriceFG.secondInGold)
                            {
                                WriteLine("<Error>Not enough gold.</Error>");
                                return;
                            }
                            break;

                        case CONSTANTS.FORTUNETOKENS:
                            if (acc.FortuneTokens < box.PriceFG.firstInToken)
                            {
                                WriteLine("<Error>Not enough fortune tokens.</Error>");
                                return;
                            }
                            break;

                        default:
                            {
                                WriteLine($"<Error>Invalid currency type ID {currency}.</Error>");
                            }
                            return;
                    }

                    do
                    {
                        string item = Utils.GetCommaSepString(GetAwards(box.Contents));
                        if (!candidates.Contains(item))
                            candidates.Add(item);
                    } while (candidates.Count < 3);
                    switch (rows)
                    {
                        case 0:
                            {
                                switch (currency)
                                {
                                    case CONSTANTS.GOLD:
                                        if (acc.Credits < box.PriceFG.firstInGold)
                                        {
                                            WriteLine("<Error>Not enough gold.</Error>");
                                            return;
                                        }
                                        if (CurrentGames.ContainsKey(acc.AccountId))
                                            CurrentGames.Remove(acc.AccountId);
                                        CurrentGames.Add(acc.AccountId, candidates.ToArray());
                                        price = box.PriceFG.firstInGold;
                                        data =
                                            string.Format(@"<Success>
                                            <Candidates>{0}</Candidates>
                                            <Gold>{1}</Gold>
                                        </Success>", Utils.GetCommaSepString(candidates.ToArray()), acc.Credits - price);
                                        break;

                                    case CONSTANTS.FORTUNETOKENS:
                                        if (acc.FortuneTokens < box.PriceFG.firstInToken)
                                        {
                                            WriteLine("<Error>Not enough fortune tokens.</Error>");
                                            return;
                                        }
                                        if (CurrentGames.ContainsKey(acc.AccountId))
                                            CurrentGames.Remove(acc.AccountId);
                                        CurrentGames.Add(acc.AccountId, candidates.ToArray());
                                        price = box.PriceFG.firstInToken;
                                        data =
                                            string.Format(@"<Success>
                                            <Candidates>{0}</Candidates>
                                            <FortuneToken>{1}</FortuneToken>
                                        </Success>", Utils.GetCommaSepString(candidates.ToArray()), acc.FortuneTokens - price);
                                        break;

                                    default:
                                        {
                                            WriteLine($"<Error>Invalid currency type ID {currency}.</Error>");
                                        }
                                        return;
                                }
                            }
                            break;

                        case 1:
                            {
                                switch (currency)
                                {
                                    case CONSTANTS.GOLD:
                                        {
                                            if (CurrentGames.ContainsKey(acc.AccountId))
                                            {
                                                candidates = CurrentGames[acc.AccountId].ToList();
                                                candidates.Shuffle();
                                                data =
                                                    string.Format(@"<Success>
                                                    <Awards>{0}</Awards>
                                                </Success>", candidates[int.Parse(Query["choice"])]);
                                                gifts = Utils.FromCommaSepString32(candidates[int.Parse(Query["choice"])]);
                                                foreach (int i in gifts)
                                                    giftsList.Add(i);
                                                candidates.Remove(candidates[int.Parse(Query["choice"])]);
                                                CurrentGames[acc.AccountId] = candidates.ToArray();
                                            }
                                        }
                                        break;

                                    case CONSTANTS.FORTUNETOKENS:
                                        {
                                            if (CurrentGames.ContainsKey(acc.AccountId))
                                            {
                                                candidates = CurrentGames[acc.AccountId].ToList();
                                                candidates.Shuffle();
                                                data =
                                                    string.Format(@"<Success>
                                                    <Awards>{0}</Awards>
                                                </Success>", candidates[int.Parse(Query["choice"])]);
                                                gifts = Utils.FromCommaSepString32(candidates[int.Parse(Query["choice"])]);
                                                foreach (int i in gifts)
                                                    giftsList.Add(i);
                                                candidates.Remove(candidates[int.Parse(Query["choice"])]);
                                                CurrentGames[acc.AccountId] = candidates.ToArray();
                                            }
                                        }
                                        break;

                                    default:
                                        {
                                            WriteLine($"<Error>Invalid currency type ID {currency}.</Error>");
                                        }
                                        return;
                                }
                            }
                            break;

                        case 2:
                            {
                                switch (currency)
                                {
                                    case CONSTANTS.GOLD:
                                        {
                                            if (CurrentGames.ContainsKey(acc.AccountId))
                                            {
                                                if (acc.Credits < box.PriceFG.secondInGold)
                                                {
                                                    WriteLine("<Error>Not enough gold.</Error>");
                                                    return;
                                                }
                                                candidates = CurrentGames[acc.AccountId].ToList();
                                                candidates.Shuffle();
                                                price = box.PriceFG.secondInGold;
                                                data =
                                                    string.Format(@"<Success>
                                                    <Awards>{0}</Awards>
                                                </Success>", candidates[int.Parse(Query["choice"])]);
                                                gifts = Utils.FromCommaSepString32(candidates[int.Parse(Query["choice"])]);
                                                foreach (int i in gifts)
                                                    giftsList.Add(i);
                                                candidates.Remove(candidates[int.Parse(Query["choice"])]);
                                                CurrentGames.Remove(acc.AccountId);
                                            }
                                        }
                                        break;

                                    case CONSTANTS.FORTUNETOKENS:
                                        {
                                            WriteLine($"<Error>You can not play twiche with a Fortune Token.</Error>");
                                        }
                                        return;

                                    default:
                                        {
                                            WriteLine($"<Error>Invalid currency type ID {currency}.</Error>");
                                        }
                                        return;
                                }
                            }
                            break;

                        default:
                            {
                                WriteLine($"<Error>Unreachable data stage {rows}.</Error>");
                            }
                            return;
                    }

                    switch (currency)
                    {
                        case CONSTANTS.GOLD:
                            {
                                Database.UpdateCredit(acc, price < 0 ? 0 : -price);
                            }
                            break;

                        case CONSTANTS.FORTUNETOKENS:
                            {
                                Database.UpdateTokens(acc, price < 0 ? 0 : -price);
                            }
                            break;

                        default:
                            {
                                WriteLine($"<Error>Invalid currency type ID {currency}.</Error>");
                            }
                            return;
                    }

                    acc.Gifts = giftsList.ToArray();

                    acc.FlushAsync();
                    acc.Reload();

                    WriteLine(data);
                }
                else
                    WriteErrorLine(status.GetInfo());
            }
        }

        private int[] GetAwards(string items)
        {
            int[] ret = new int[items.Split(';').Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = Utils.FromString(items.Split(';')[0].Split(',')[rand.Next(items.Split(';')[0].Split(',').Length)]);
            return ret.ToArray();
        }
    }
}