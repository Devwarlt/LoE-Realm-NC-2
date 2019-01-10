#region

using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class Test : World, IDungeon
    {
        public string js = null;

        public Test()
        {
            Id = (int) WorldID.TEST_ID;
            Name = "Test";
            Background = 0;
            Dungeon = true;
        }

        public void LoadJson(string json)
        {
            js = json;
            LoadMap(json);
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (KeyValuePair<int, Player> i in Players)
            {
                if (i.Value.Client.Account.AccountType != (int) LoESoft.Core.config.AccountType.DEVELOPER || !i.Value.Client.Account.Admin)
                {
                    i.Value.SendError(string.Format("[Staff Member: {0}] You cannot access Test world with account type {1}.", i.Value.Client.Account.Admin, nameof(i.Value.Client.Account.AccountType)));
                    GameServer.Manager.TryDisconnect(i.Value.Client, DisconnectReason.ACCESS_DENIED_DUE_RESTART);
                }
            }
        }

        protected override void Init()
        {
        }
    }
}