#region

using LoESoft.Core;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.terrain;
using Mono.Game;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Wander : CycleBehavior
    {
        private static Cooldown period = new Cooldown(500, 200);
        private readonly float speed;
        private readonly bool avoidGround;
        private readonly string ground;

        public Wander(
            double speed = 4,
            bool avoidGround = false,
            string ground = "Shallow Water"
            )
        {
            this.speed = (float)speed / 10;
            this.avoidGround = avoidGround;
            this.ground = ground;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            WanderStorage storage;

            if (state == null)
                storage = new WanderStorage();
            else
                storage = (WanderStorage)state;

            Status = CycleStatus.NotStarted;

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return;

            Status = CycleStatus.InProgress;

            if (storage.RemainingDistance <= 0)
            {
                storage.Direction = new Vector2(Random.Next(-1, 2), Random.Next(-1, 2));
                storage.Direction.Normalize();
                storage.RemainingDistance = period.Next(Random) / 1000f;
                Status = CycleStatus.Completed;
            }

            float dist = host.EntitySpeed(speed, time);

            if (avoidGround)
            {
                EmbeddedData data = GameServer.Manager.GameData;
                float x = host.X + storage.Direction.X * dist;
                float y = host.Y + storage.Direction.Y * dist;
                WmapTile tile = host.Owner.Map[(int)x, (int)y];
                if (tile.TileId == data.IdToTileType[ground])
                {
                    host.UpdateCount++;
                    storage.RemainingDistance -= dist;
                    state = storage;
                    return;
                }
            }

            host.ValidateAndMove(host.X + storage.Direction.X * dist, host.Y + storage.Direction.Y * dist);

            host.UpdateCount++;

            storage.RemainingDistance -= dist;

            state = storage;
        }

        private class WanderStorage
        {
            public Vector2 Direction;
            public float RemainingDistance;
        }
    }
}