﻿using System.Collections.Generic;

namespace LoESoft.GameServer.realm
{
    internal partial class Realm
    {
        public void UpdateHeroes()
        {
            foreach (var i in RealmEventCache)
                RealmHeroCache.Add(i.Name);
        }

        public List<string> RealmHeroCache = new List<string>
        {
            "Lich",
            "Actual Lich",
            "Ent Ancient",
            "Actual Ent Ancient",
            "Phoenix Reborn",
            "Oasis Giant",
            "Ghost King",
            "Cyclops God",
            "Red Demon"
        };

        public bool HandleHeroes()
        {
            if (CountEnemies(RealmHeroCache.ToArray()) != 0)
                return false;

            return RealmClosed = true;
        }
    }
}