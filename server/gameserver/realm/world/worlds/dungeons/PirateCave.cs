#region

using System;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class PirateCave : World
    {
        public PirateCave()
        {
            Name = "Pirate Cave";
            Background = 0;
            Difficulty = 1;
            AllowTeleport = true;
        }

        protected override void Init() => LoadMap($"dungeons.pirate_cave.pirate_cave_{new Random((int)Seed).Next(1, 10).ToString()}", MapType.Json);
    }
}