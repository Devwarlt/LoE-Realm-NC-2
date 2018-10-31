﻿namespace LoESoft.GameServer.realm.world
{
    public class WineCellar : World
    {
        public WineCellar()
        {
            Name = "Wine Cellar";
            Background = 0;
            AllowTeleport = false;
            Dungeon = true;
        }

        protected override void Init() => LoadMap("winecellar", MapType.Wmap);
    }
}