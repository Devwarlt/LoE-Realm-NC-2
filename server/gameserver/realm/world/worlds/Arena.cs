#region

using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class Arena : PublicArena
    {
        public Arena()
        {
            Id = (int)WorldID.ARENA;
            Name = "Arena";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
        }

        protected override void Init() =>
            LoadMap("arena", MapType.Wmap);

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            if (Players.Count == 0)
                return;

            CheckOutOfBounds();

            InitArena(time);
        }

        #region "Entities"

        private new readonly List<string> EntityQuest = new List<string>
        {
            "Crystal Prisoner",
            "Grand Sphinx",
            "Stheno the Snake Queen",
            "Frog King",
            "Cube God",
            "Skull Shrine",
            "Oryx the Mad God 2"
        };

        #endregion "Entities"
    }
}