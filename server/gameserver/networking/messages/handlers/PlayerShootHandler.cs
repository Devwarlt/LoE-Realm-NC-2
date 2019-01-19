#region

using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.messages.handlers.hack;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using System.Linq;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class PlayerShootPacketHandler : MessageHandlers<PLAYERSHOOT>
    {
        public override MessageID ID => MessageID.PLAYERSHOOT;

        protected override void HandleMessage(Client client, PLAYERSHOOT message) => Handle(client.Player, message);

        private void Handle(Player player, PLAYERSHOOT message)
        {
            if (!GameServer.Manager.GameData.Items.TryGetValue((ushort)message.ContainerType, out Item item))
                return;

            var ability = TierLoot.AbilitySlotType.ToList().Contains(item.SlotType);

            var _cheatHandler = new DexterityCheatHandler()
            {
                Player = player,
                Item = item,
                IsAbility = ability,
                AttackAmount = message.AttackAmount,
                MinAttackFrequency = message.MinAttackFrequency,
                MaxAttackFrequency = message.MaxAttackFrequency,
                WeaponRateOfFire = message.WeaponRateOfFire
            };

            _cheatHandler.Handler();

            var _projectile = player.PlayerShootProjectile(message.BulletId, item.Projectiles[0], item.ObjectType, GameServer.Manager.Logic.GameTime.TotalElapsedMs, message.Position, message.Angle);
            var allyshot = new ALLYSHOOT
            {
                Angle = message.Angle,
                BulletId = message.BulletId,
                ContainerType = message.ContainerType,
                OwnerId = player.Id
            };

            player.Owner.EnterWorld(_projectile);

            if (ability)
                player.BroadcastSync(allyshot, p => p.Dist(player) <= 14);
            else
                player.BroadcastSync(allyshot, p => p != player && p.Dist(player) <= 14);

            player.FameCounter.Shoot(_projectile);
        }
    }
}