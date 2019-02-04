#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class MoveHandler : MessageHandlers<MOVE>
    {
        public override MessageID ID => MessageID.MOVE;

        protected override void HandleMessage(Client client, MOVE message) => Handle(client.Player, message);

        private void Handle(Player player, MOVE message)
        {
            if (player?.Owner == null || player.HasConditionEffect(ConditionEffectIndex.Paralyzed) || message.Position.X == -1 || message.Position.Y == -1)
                return;

            var newX = message.Position.X;
            var newY = message.Position.Y;

            if (newX != player.X || newY != player.Y)
            {
                CheckLabConditions(player, message);
                player.Move(newX, newY);
                player.UpdateCount++;
            }
        }

        private static void CheckLabConditions(Entity player, MOVE packet)
        {
            var tile = player.Owner.Map[(int)packet.Position.X, (int)packet.Position.Y];

            switch (tile.TileId)
            {
                //Green water
                case 0xa9:
                case 0x82:
                    if (tile.ObjId != 0)
                        return;

                    if (!player.HasConditionEffect(ConditionEffectIndex.Hexed) ||
                        !player.HasConditionEffect(ConditionEffectIndex.Stunned) ||
                        !player.HasConditionEffect(ConditionEffectIndex.Speedy))
                    {
                        player.ApplyConditionEffect(ConditionEffectIndex.Hexed);
                        player.ApplyConditionEffect(ConditionEffectIndex.Stunned);
                        player.ApplyConditionEffect(ConditionEffectIndex.Speedy);
                    }
                    break;
                //Blue water
                case 0xa7:
                case 0x83:
                    if (tile.ObjId != 0)
                        return;

                    if (player.HasConditionEffect(ConditionEffectIndex.Hexed) ||
                        player.HasConditionEffect(ConditionEffectIndex.Stunned) ||
                        player.HasConditionEffect(ConditionEffectIndex.Speedy))
                    {
                        player.ApplyConditionEffect(ConditionEffectIndex.Hexed, 0);
                        player.ApplyConditionEffect(ConditionEffectIndex.Stunned, 0);
                        player.ApplyConditionEffect(ConditionEffectIndex.Speedy, 0);
                    }
                    break;
            }
        }
    }
}