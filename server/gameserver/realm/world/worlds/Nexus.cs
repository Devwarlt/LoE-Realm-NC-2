#region

using LoESoft.GameServer.realm.entity;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class Nexus : World, IDungeon
    {
        public const string LOE_CHICAGO_BUILD_2_1 = "loe_chicago_2.1";
        public const string LOE_CHICAGO_BUILD_3_2_7 = "loe_chicago_3.2.7";
        public const string SEBS_NEXUS = "new_nexus"; //TODO

        public Nexus()
        {
            Id = (int)WorldID.NEXUS_ID;
            Name = "Nexus";
            Background = 2;
            AllowTeleport = false;
            Difficulty = -1;
            Dungeon = false;
            SafePlace = true;
        }

        protected override void Init() => LoadMap(LOE_CHICAGO_BUILD_3_2_7, MapType.Json);

        public override void Tick(RealmTime time)
        {
            base.Tick(time);
            UpdatePortals();
        }

        private void UpdatePortals()
        {
            foreach (var i in GameServer.Manager.Monitor.portals)
                foreach (var j in RealmManager.CurrentRealmNames)
                    if (i.Value.Name.StartsWith(j))
                    {
                        if (i.Value.Name == j)
                            (i.Value as Portal).PortalName = i.Value.Name;

                        i.Value.Name = j + " (" + i.Key.Players.Count + "/" + i.Key.MaxPlayers + ")";
                        i.Value.UpdateCount++;
                        break;
                    }
        }
    }
}