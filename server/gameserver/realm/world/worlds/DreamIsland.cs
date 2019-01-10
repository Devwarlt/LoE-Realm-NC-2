namespace LoESoft.GameServer.realm.world
{
    public class DreamIsland : World, IDungeon
    {
        private string Expansion_1 => "dream-island-expansion1";

        public DreamIsland()
        {
            Id = (int)WorldID.DREAM_ISLAND;
            Name = "Dream Island";
            Background = 2;
            AllowTeleport = false;
            Difficulty = 5;
        }

        protected override void Init() => LoadMap(Expansion_1, MapType.Json);
    }
}