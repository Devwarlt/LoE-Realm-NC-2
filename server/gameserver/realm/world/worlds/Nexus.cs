#region

using LoESoft.GameServer.realm.entity;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class Nexus : World, IDungeon
    {
        public const string LOE_CHICAGO_21 = "loe_chicago_2.1";
        public const string WINTER_RESOURCE = "nexus_winter";
        public const string SUMMER_RESOURCE = "nexus_summer";
		public const string FILISHA_RESOURCE = "SND";

		public Nexus()
        {
            Id = (int) WorldID.NEXUS_ID;
            Name = "Nexus";
            Background = 2;
            AllowTeleport = false;
            Difficulty = -1;
            Dungeon = false;
            SafePlace = true;
        }

        protected override void Init() => LoadMap(FILISHA_RESOURCE, MapType.Json);

        public override void Tick(RealmTime time)
        {
            base.Tick(time);
            UpdatePortals();
        }

        private void UpdatePortals()
        {
            foreach (var i in GameServer.Manager.Monitor.portals)
            {
                foreach (var j in RealmManager.CurrentRealmNames)
                {
                    if (i.Value.Name.StartsWith(j))
                    {
                        if (i.Value.Name == j)
                            (i.Value as Portal).PortalName = i.Value.Name;
                        i.Value.Name = j + " (" + i.Key.Players.Count + ")";
                        i.Value.UpdateCount++;
                        break;
                    }
                }
            }
        }
    }
}