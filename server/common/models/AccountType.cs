namespace LoESoft.Core.config
{
    public enum AccountType : int
    {
        REGULAR = 0,
        VIP = 1,
        MOD = 2,
        DEVELOPER = 3,
        ADMIN = 4
    }

    public class AccountTypePerks
    {
        private AccountType _accountType { get; set; }
        private bool _accessToDrastaCitadel { get; set; }
        private bool _byPassKeysRequirements { get; set; }
        private bool _byPassEggsRequirements { get; set; }
        private bool _priorityToLogin { get; set; }

        public AccountTypePerks(int accountType)
        {
            _accountType = (AccountType)accountType;
            _accessToDrastaCitadel = _accountType >= AccountType.VIP;
            _byPassKeysRequirements = _accountType >= AccountType.VIP;
            _byPassEggsRequirements = _accountType >= AccountType.VIP;
            _priorityToLogin = _accountType >= AccountType.VIP;
        }

        public bool AccessToDrastaCitadel() => _accessToDrastaCitadel;

        public bool ByPassKeysRequirements() => _byPassKeysRequirements;

        public bool ByPassEggsRequirements() => _byPassEggsRequirements;

        public bool PriorityToLogin() => _priorityToLogin;

        public ConditionEffect SetAccountTypeIcon()
        {
            var icon = new ConditionEffect { DurationMS = -1 };

            switch (_accountType)
            {
                case AccountType.REGULAR:
                    icon.Effect = ConditionEffectIndex.RegularAccount;
                    break;

                case AccountType.VIP:
                    icon.Effect = ConditionEffectIndex.VipAccount;
                    break;

                case AccountType.MOD:
                    icon.Effect = ConditionEffectIndex.ModAccount;
                    break;

                case AccountType.DEVELOPER:
                    icon.Effect = ConditionEffectIndex.DeveloperAccount;
                    break;

                case AccountType.ADMIN:
                    icon.Effect = ConditionEffectIndex.AdminAccount;
                    break;
            }

            return icon;
        }

        public double MerchantDiscount() => _accountType == AccountType.VIP ? 0.9 : 1;
    }
}