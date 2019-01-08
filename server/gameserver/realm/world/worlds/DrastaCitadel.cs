namespace LoESoft.GameServer.realm.world
{
    public class DrastaCitadel : World, IDungeon
    {
        public DrastaCitadel()
        {
            Id = (int)WorldID.DRASTA_CITADEL_ID;
            Name = "Drasta Citadel";
            Background = 2;
            AllowTeleport = false;
            Difficulty = 0;
            SafePlace = true;
        }

        protected override void Init() => LoadMap("drasta", MapType.Json);
    }
}