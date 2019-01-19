#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using System.Linq;

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

            var prj = player.Owner.Projectiles.Keys.FirstOrDefault(projectile =>
            projectile.ProjectileOwner.Id == message.ObjectId && projectile.ProjectileId == message.BulletId);

            if (prj == null || prj == default(Projectile))
                return;

            if (prj.ProjDesc.Effects.Length != 0)
                foreach (var effect in prj.ProjDesc.Effects)
                    if (effect.Target == 1)
                        continue;
                    else
                        player.ApplyConditionEffect(effect);

            player.ForceHit(prj);
        }
    }
}