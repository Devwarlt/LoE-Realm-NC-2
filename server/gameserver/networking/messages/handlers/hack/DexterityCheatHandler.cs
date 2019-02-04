#region

using LoESoft.Core.config;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.networking.messages.handlers.hack
{
    public class DexterityCheatHandler : ICheatHandler
    {
        public Player Player { get; set; }
        public Item Item { get; set; }
        public bool IsAbility { get; set; }
        public int AttackAmount { get; set; }
        public float MinAttackFrequency { get; set; }
        public float MaxAttackFrequency { get; set; }
        public float WeaponRateOfFire { get; set; }

        private bool ByPass => Player.AccountType == (int)AccountType.DEVELOPER;

        CheatID ICheatHandler.ID => CheatID.DEXTERITY;

        public void Handler()
        {
            if (Item == Player.Inventory[1] || Item == Player.Inventory[2] || Item == Player.Inventory[3])
                return;

            if (IsAbility)
                return;

            if ((AttackAmount != Item.NumProjectiles
                || MinAttackFrequency != StatsManager.MinAttackFrequency
                || MaxAttackFrequency != StatsManager.MaxAttackFrequency
                || WeaponRateOfFire != Item.RateOfFire) && !ByPass)
            {
                GameServer.Manager.TryDisconnect(Player.Client, Client.DisconnectReason.DEXTERITY_HACK_MOD);
                return;
            }
        }
    }
}