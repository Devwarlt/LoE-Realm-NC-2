using System;
namespace LoESoft.GameServer.realm.world
{
    public class AbyssofDemons : World
    {
		private Random r = new Random();
		public string map => "abyss";
        public AbyssofDemons()
        {
            Name = "Abyss of Demons";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init() => LoadMap($"dungeons.abyss.abyss{r.Next(1, 2).ToString()}",MapType.Wmap);
    }
}