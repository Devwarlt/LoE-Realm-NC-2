namespace LoESoft.GameServer.realm.world
{
    public class ClothBazaar : World, IDungeon
    {
        public ClothBazaar()
        {
            Id = (int)WorldID.MARKET;
            Name = "Cloth Bazaar";
            Background = 2;
            AllowTeleport = false;
            Difficulty = 0;
            SafePlace = true;
        }

        protected override void Init() => LoadMap("bazzar", MapType.Wmap);
    }
}