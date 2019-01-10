#region

using LoESoft.GameServer.networking.outgoing;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    partial class Player
    {
        public string GetLanguageString(string key, params KeyValuePair<string, object>[] tokens)
        {
            var ret = "{\"key\":\"" + key + "\"";
            if (tokens != null)
            {
                ret += ",\"tokens\":{";
                for (var i = 0; i < tokens.Length; i++)
                {
                    ret += "\"" + tokens[i].Key + "\":\"" + tokens[i].Value + "\"";
                    if (i + 1 != tokens.Length)
                        ret += ",";
                }
                ret += "}";
            }
            ret += "}";
            return ret;
        }

        public void SendInfoWithTokens(string key, params KeyValuePair<string, object>[] tokens)
        {
            var toSend = "{\"key\":\"" + key + "\"";
            if (tokens != null)
            {
                toSend += ",\"tokens\":{";
                for (var i = 0; i < tokens.Length; i++)
                {
                    toSend += "\"" + tokens[i].Key + "\":\"" + tokens[i].Value + "\"";
                    if (i + 1 != tokens.Length)
                        toSend += ",";
                }
                toSend += "}";
            }
            toSend += "}";
            SendInfo(toSend);
        }
		public void SendLootAnnounce(string text)
		{
			Client.SendMessage(new TEXT()
			{
				BubbleTime = 0,
				Stars = Stars,
				Name = "Gazer",
				Text = text,
				NameColor = 0xFFB6C1,
				TextColor = 0xFFB6C1
			});
		}
		public void SendDeathAnnounce(string text)
		{
			Client.SendMessage(new TEXT()
			{
				BubbleTime = 5,
				Stars = Stars,
				Name = "Gazer",
				Text = text,
				NameColor = 0xADD8E6,
				TextColor = 0xADD8E6
			});
		}
		public void SendInfo(string text)
        {
            Client.SendMessage(new TEXT()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "",
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void SendError(string text)
        {
            Client.SendMessage(new TEXT()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "*Error*",
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void SendpsrText(string text)
        {
            Client.SendMessage(new TEXT()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "*psr*",
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void SendHelp(string text)
        {
            Client.SendMessage(new TEXT()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "*Help*",
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void SendEnemy(string name, string text)
        {
            Client.SendMessage(new TEXT()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "#" + name,
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void SendText(string sender, string text)
        {
            Client.SendMessage(new TEXT()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = sender,
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void SendGuild(string text)
        {
            Client.SendMessage(new TEXT
            {
                BubbleTime = 0,
                Stars = Stars,
                Name = "",
                Recipient = "*Guild*",
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }

        public void GuildRecieved(int objId, int stars, string from, string text)
        {
            Client.SendMessage(new TEXT
            {
                BubbleTime = 10,
                Stars = stars,
                Name = "*Guild*",
                Recipient = from,
                Text = text,
                NameColor = 0x123456,
                TextColor = 0x123456
            });
        }
    }
}