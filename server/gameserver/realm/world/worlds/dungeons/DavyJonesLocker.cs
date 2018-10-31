﻿#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.realm.world
{
    public class DavyJonesLocker : World
    {
        public DavyJonesLocker()
        {
            Name = "Davy Jones's Locker";
            Dungeon = true;
            Difficulty = 5;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init() => LoadMap("vault", MapType.Wmap);

        public override int EnterWorld(Entity entity)
        {
            int ret = base.EnterWorld(entity);
            if (entity is Player)
                (entity as Player).Client.SendMessage(new GLOBAL_NOTIFICATION
                {
                    Text = "showKeyUI",
                    Type = 0
                });
            return ret;
        }

        public override void LeaveWorld(Entity entity)
        {
            if (entity is Player)
                (entity as Player).Client.SendMessage(new GLOBAL_NOTIFICATION
                {
                    Text = "showKeyUI",
                    Type = 0
                });
            base.LeaveWorld(entity);
        }
    }
}