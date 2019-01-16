#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class EnemyHitHandler : MessageHandlers<ENEMYHIT>
    {
        public override MessageID ID => MessageID.ENEMYHIT;

        protected override void HandleMessage(Client client, ENEMYHIT message) => Handle(client.Player, Manager.Logic.GameTime, message);

        private void Handle(Player player, RealmTime time, ENEMYHIT message)
        {
            if (player == null)
                return;

            var entity = player.Owner.GetEntity(message.TargetId);

            if (entity == null)
                return;

            var prj = player.Owner.GetProjectileFromId(player.Id, message.BulletId);

            if (prj == null)
                return;

            if (!prj.ProjDesc.MultiHit)
                prj.Owner.RemoveProjectileFromId(player.Id, message.BulletId);

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