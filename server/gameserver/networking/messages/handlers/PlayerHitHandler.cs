#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class PlayerHitHandler : MessageHandlers<PLAYERHIT>
    {
        public override MessageID ID => MessageID.PLAYERHIT;

        protected override void HandleMessage(Client client, PLAYERHIT message) => Handle(client.Player, message);

        private void Handle(Player player, PLAYERHIT message)
        {
            if (player == null)
                return;

            var entity = player.Owner.GetEntity(message.ObjectId);

            if (entity == null)
                return;

            var prj = entity.Owner.GetProjectileFromId(message.ObjectId, message.BulletId);

            if (prj == null)
                return;

            prj.Owner.RemoveProjectileFromId(message.ObjectId, message.BulletId);

            if (prj.ProjDesc.Effects.Length != 0)
                foreach (ConditionEffect effect in prj.ProjDesc.Effects)
                    if (effect.Target == 1)
                        continue;
                    else
                        player.ApplyConditionEffect(effect);

            player.ForceHit(prj.Damage, entity, prj.ProjDesc.ArmorPiercing);
        }
    }
}