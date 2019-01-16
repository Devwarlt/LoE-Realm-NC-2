#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm.terrain;
using System;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class GroundDamageHandler : MessageHandlers<GROUNDDAMAGE>
    {
        public override MessageID ID => MessageID.GROUNDDAMAGE;

        protected override void HandleMessage(Client client, GROUNDDAMAGE message)
        {
            if (client.Player.HasConditionEffect(ConditionEffectIndex.Paused) ||
                client.Player.HasConditionEffect(ConditionEffectIndex.Invincible) ||
                client.Player.HasConditionEffect(ConditionEffectIndex.Invulnerable))
                return;

            try
            {
                if (client.Player.Owner == null)
                    return;
                WmapTile tile = client.Player.Owner.Map[(int)message.Position.X, (int)message.Position.Y];
                ObjectDesc objDesc = tile.ObjType == 0 ? null : Manager.GameData.ObjectDescs[tile.ObjType];
                TileDesc tileDesc = Manager.GameData.Tiles[tile.TileId];
                if (tileDesc.Damaging && (objDesc == null || !objDesc.ProtectFromGroundDamage))
                {
                    int dmg = (int)client.Player.StatsManager.Random.Obf6((uint)tileDesc.MinDamage, (uint)tileDesc.MaxDamage);
                    dmg = (int)client.Player.StatsManager.GetDefenseDamage(dmg, true);

                    client.Player.HP -= dmg;
                    client.Player.UpdateCount++;
                    if (client.Player.HP <= 0)
                        client.Player.Death(tileDesc.ObjectId);
                }
            }
            catch (Exception ex)
            {
                log4net.Error(ex);
            }
        }
    }
}