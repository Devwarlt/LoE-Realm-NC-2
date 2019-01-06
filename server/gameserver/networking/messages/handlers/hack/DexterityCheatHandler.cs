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

        public DexterityCheatHandler()
        {
        }

        private bool ByPass
        { get { return Player.AccountType == (int) AccountType.DEM_ACCOUNT; } }

        CheatID ICheatHandler.ID
        { get { return CheatID.DEXTERITY; } }

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
                GameServer.log.Info($"Dexterity Cheat Handler ID '{Player.MaxHackEntries++}':" +
                    $"\n\t- Player: {Player.Name} (ID: {Player.AccountId})," +
                    $"\n\t- World: {Player.Owner.Name}," +
                    $"\n\t- Item: {Item.DisplayId} (type: 0x{Item.ObjectType:x4})," +
                    $"\n\t- Attack amount: {AttackAmount} (valid: {Item.NumProjectiles})," +
                    $"\n\t- Min attack frequency: {MinAttackFrequency} (valid: {StatsManager.MinAttackFrequency})," +
                    $"\n\t- Max attack frequency: {MaxAttackFrequency} (valid: {StatsManager.MaxAttackFrequency})," +
                    $"\n\t- Weapon rate of fire: {WeaponRateOfFire} (valid: {Item.RateOfFire}).");

                GameServer.Manager.TryDisconnect(Player.Client, Client.DisconnectReason.DEXTERITY_HACK_MOD);
                return;
            }
        }
    }
}