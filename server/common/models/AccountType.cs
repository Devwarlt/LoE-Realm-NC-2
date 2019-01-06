namespace LoESoft.Core.config
{
    public enum AccountType : int
    {
        FREE_ACCOUNT = 0,
        VIP_ACCOUNT = 1,
        GM_ACCOUNT = 2,
        CM_ACCOUNT = 3,
        DEM_ACCOUNT = 4
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
            _accessToDrastaCitadel = _accountType >= AccountType.VIP_ACCOUNT;
            _byPassKeysRequirements = _accountType >= AccountType.VIP_ACCOUNT;
            _byPassEggsRequirements = _accountType >= AccountType.VIP_ACCOUNT;
            _priorityToLogin = _accountType >= AccountType.VIP_ACCOUNT;
        }

        public int Experience(int level, int experience)
        {
            if (_accountType == AccountType.VIP_ACCOUNT)
                return level < 20 ? (int)(experience * 1.5) : (int)(experience * 1.05);

            return experience;
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
                case AccountType.FREE_ACCOUNT:
                    icon.Effect = ConditionEffectIndex.FreeAccount;
                    break;

                case AccountType.VIP_ACCOUNT:
                    icon.Effect = ConditionEffectIndex.VipAccount;
                    break;

                case AccountType.GM_ACCOUNT:
                    icon.Effect = ConditionEffectIndex.GmAccount;
                    break;

                case AccountType.CM_ACCOUNT:
                    icon.Effect = ConditionEffectIndex.CmAccount;
                    break;

                case AccountType.DEM_ACCOUNT:
                    icon.Effect = ConditionEffectIndex.DemAccount;
                    break;
            }

            return icon;
        }

        public double MerchantDiscount() => _accountType == AccountType.VIP_ACCOUNT ? 0.9 : 1;
    }
}