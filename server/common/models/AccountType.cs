﻿namespace LoESoft.Core.config
{
    public enum AccountType : int
    {
        REGULAR = 0,
        VIP = 1,
		DESIGNER=2,
        MOD = 3,
        DEVELOPER = 4,
        NORGA = 5
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

        public int Experience(int level, int experience)
        {
            if (_accountType == AccountType.VIP)
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
                case AccountType.REGULAR:
                    icon.Effect = ConditionEffectIndex.RegularAccount;
                    break;

                case AccountType.VIP:
                    icon.Effect = ConditionEffectIndex.VipAccount;
                    break;
				case AccountType.DESIGNER:
					icon.Effect = ConditionEffectIndex.DesignerAccount;
					break;
				case AccountType.MOD:
                    icon.Effect = ConditionEffectIndex.ModAccount;
                    break;

                case AccountType.DEVELOPER:
                    icon.Effect = ConditionEffectIndex.DeveloperAccount;
                    break;

                case AccountType.NORGA:
                    icon.Effect = ConditionEffectIndex.NorgaAccount;
                    break;
            }

            return icon;
        }

        public double MerchantDiscount() => _accountType == AccountType.VIP ? 0.9 : 1;
    }
}