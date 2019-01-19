using LoESoft.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LoESoft.AppEngine.account
{
    internal class accountInformation : RequestHandler
    {
        protected override void HandleRequest()
        {
            using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
            {
                using (StreamReader rdr = new StreamReader($"account/struct/AccountInformationStruct.json"))
                {
                    DbAccount acc;
                    LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);
                    string JSONData = rdr.ReadToEnd();
                    if (status == LoginStatus.OK)
                    {
                        List<string> data = new List<string>();

                        AccountInformation user = new AccountInformation();
                        user.name = acc.Name;
                        user.accountType = acc.AccountType;
                        user.accountLifetime = acc.AccountLifetime;
                        user.email = acc.UUID;
                        user.guildId = Convert.ToInt32(acc.GuildId);
                        user.guildName = user.guildId == -1 ? string.Empty : Database.GetGuild(acc.GuildId).Name;
                        user.guildRank = user.guildId == -1 ? -1 : acc.GuildRank;
                        user.isBanned = acc.Banned;
                        user.isRegistered = acc.Verified;
                        user.isAgeVerified = acc.IsAgeVerified == 1 ? true : false;
                        user.isNameChosen = acc.NameChosen;
                        user.isAccountMuted = acc.Muted;
                        user.totalFame = acc.TotalFame;
                        user.registration = acc.RegTime;
                        user.vaultQuantity = acc.VaultCount;
                        user.characterSlotQuantity = acc.MaxCharSlot;
                        user.credits = acc.Credits;
                        user.fame = acc.Fame;
                        user.authenticationToken = acc.AuthToken;

                        data.Add(JSONData.Replace("{NAME}", user.name));
                        data.Add(data[0].Replace("{FORMAT_ACCOUNT_TYPE}", user.formatAccountType()));
                        data.Add(data[1].Replace("{EMAIL}", user.email));
                        data.Add(data[2].Replace("{FORMAT_GUILD}", user.formatGuild()));
                        data.Add(data[3].Replace("{IS_BANNED}", $"{user.isBanned}"));
                        data.Add(data[4].Replace("{IS_REGISTERED}", $"{user.isRegistered}"));
                        data.Add(data[5].Replace("{IS_AGE_VERIFIED}", $"{user.isAgeVerified}"));
                        data.Add(data[6].Replace("{IS_NAME_CHOSEN}", $"{user.isNameChosen}"));
                        data.Add(data[7].Replace("{IS_MUTED}", $"{user.isAccountMuted}"));
                        data.Add(data[8].Replace("{TOTAL_FAME}", $"{user.totalFame}"));
                        data.Add(data[9].Replace("{REGISTRATION}", $"{user.registration}"));
                        data.Add(data[10].Replace("{FORMAT_VAULT}", user.formatVault()));
                        data.Add(data[11].Replace("{FORMAT_CHARACTER_SLOT}", user.formatCharacterSlot()));
                        data.Add(data[12].Replace("{FORMAT_CREDITS}", user.formatCredits()));
                        data.Add(data[13].Replace("{FORMAT_FAME}", user.formatFame()));
                        data.Add(data[14].Replace("{AUTH_TOKEN}", user.authenticationToken));

                        wtr.Write(JsonConvert.DeserializeObject<List<AccountInformationMessages>>(data[data.Count - 1])[0].message);
                    }
                    else
                        wtr.Write(JsonConvert.DeserializeObject<List<AccountInformationMessages>>(JSONData)[0].error);
                }
            }
        }

        private class AccountInformation
        {
            public string name;
            public int accountType;
            public DateTime accountLifetime;
            public string email;
            public int guildId;
            public string guildName;
            public int guildRank;
            public bool isBanned;
            public bool isRegistered;
            public bool isAgeVerified;
            public bool isNameChosen;
            public bool isAccountMuted;
            public int totalFame;
            public DateTime registration;
            public int vaultQuantity;
            public int characterSlotQuantity;
            public int credits;
            public int fame;
            public string authenticationToken;

            private enum AccountType : int
            {
                FREE_ACCOUNT = 0,
                VIP_ACCOUNT = 1,
                LEGENDS_OF_LOE_ACCOUNT = 2,
                TUTOR_ACCOUNT = 3,
                LOESOFT_ACCOUNT = 4
            }

            private enum GuildRank : int
            {
                INITIATE = 0,
                MEMBER = 10,
                OFFICER = 20,
                LEADER = 30,
                FOUNDER = 40
            }

            public string formatAccountType()
            {
                bool isVip = accountLifetime > DateTime.UtcNow;
                int days = isVip ? (accountLifetime - DateTime.UtcNow).Days : 0;
                switch (accountType)
                {
                    case (int)AccountType.FREE_ACCOUNT:
                        return "Free Account";

                    case (int)AccountType.VIP_ACCOUNT:
                        return $"Vip Account\n<i>Your Vip Account end {(days > 1 ? $"in <b>{days} days</b>" : $"<b>today</b>")} at {string.Format(new DateProvider(), "{0}", accountLifetime)}.</i>";

                    case (int)AccountType.LEGENDS_OF_LOE_ACCOUNT:
                        return "Legends of LoE Account";

                    case (int)AccountType.TUTOR_ACCOUNT:
                        return "Tutor Account";

                    case (int)AccountType.LOESOFT_ACCOUNT:
                        return "LoESoft Account";
                }
                return null;
            }

            public string formatGuild()
            {
                if (guildId == -1)
                    return "Not in guild";
                string rank = null;
                switch (guildRank)
                {
                    case (int)GuildRank.INITIATE:
                        rank = "Initiate";
                        break;

                    case (int)GuildRank.MEMBER:
                        rank = "Member";
                        break;

                    case (int)GuildRank.OFFICER:
                        rank = "Officer";
                        break;

                    case (int)GuildRank.LEADER:
                        rank = "Leader";
                        break;

                    case (int)GuildRank.FOUNDER:
                        rank = "Founder";
                        break;
                }
                return $"{rank} of <b>{guildName}</b>";
            }

            public string formatVault() => $"<b>{vaultQuantity}</b> vault{(vaultQuantity > 1 ? "s" : "")}";

            public string formatCharacterSlot() => $"<b>{characterSlotQuantity}</b> character slot{(characterSlotQuantity > 1 ? "s" : "")}";

            public string formatCredits() => $"<b>{credits}</b> realm gold{(credits > 1 ? "s" : "")}";

            public string formatFame() => $"<b>{fame}</b> fame";
        }

        private struct AccountInformationMessages
        {
#pragma warning disable CS0649 // Field 'accountInformation.AccountInformationMessages.message' is never assigned to, and will always have its default value null
            public string message;
#pragma warning restore CS0649 // Field 'accountInformation.AccountInformationMessages.message' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'accountInformation.AccountInformationMessages.error' is never assigned to, and will always have its default value null
            public string error;
#pragma warning restore CS0649 // Field 'accountInformation.AccountInformationMessages.error' is never assigned to, and will always have its default value null
        }
    }
}