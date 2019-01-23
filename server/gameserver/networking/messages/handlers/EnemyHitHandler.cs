#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using System.Linq;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class EnemyHitHandler : MessageHandlers<ENEMYHIT>
    {
        public override MessageID ID => MessageID.ENEMYHIT;

        protected override void HandleMessage(Client client, ENEMYHIT message) => Handle(client.Player, GameServer.Manager.Logic.GameTime, message);

        private void Handle(Player player, RealmTime time, ENEMYHIT message)
        {
            if (player == null)
                return;

            var entity = player.Owner.GetEntity(message.TargetId);

            if (entity == null)
                return;

            var prj = player.Owner.Projectiles.Keys.FirstOrDefault(projectile =>
            projectile.ProjectileOwner.Id == player.Id && projectile.ProjectileId == message.BulletId);

            if (prj == null || prj == default(Projectile))
                return;

            if (prj.ProjDesc.Effects.Length != 0)
                foreach (var effect in prj.ProjDesc.Effects)
                    if (effect.Target == 1)
                        continue;
                    else
                        entity.ApplyConditionEffect(effect);

            prj.ForceHit(entity, time, message.Killed);
        }
    }
}