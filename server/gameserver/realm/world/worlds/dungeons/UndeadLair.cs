namespace LoESoft.GameServer.realm.world
{

    public class UndeadLair : World
    {
		private string map => "udl";
		public UndeadLair()
        {
            Name = "Undead Lair";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init() => LoadMap(map, MapType.Json);
    }
}