#region

using System;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    partial class Player
    {
        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            base.ExportStats(stats);
            stats[StatsType.ACCOUNT_ID_STAT] = AccountId;
            stats[StatsType.NAME_STAT] = Name;

            stats[StatsType.EXP_STAT] = Experience - GetExperience(Level, ExpType.Level);
            stats[StatsType.NEXT_LEVEL_EXP_STAT] = ExperienceGoal - GetExperience(Level, ExpType.Level);
            stats[StatsType.LEVEL_STAT] = Level;

            stats[StatsType.ATTACK_EXP_STAT] = AttackExperience - GetExperience(AttackLevel - 10, ExpType.Stat);
            stats[StatsType.NEXT_ATTACK_EXP_STAT] = AttackGoalExperience - GetExperience(AttackLevel - 10, ExpType.Stat);
            stats[StatsType.ATTACK_LEVEL_STAT] = AttackLevel;

            stats[StatsType.DEFENSE_EXP_STAT] = DefenseExperience - GetExperience(DefenseLevel - 10, ExpType.Stat);
            stats[StatsType.NEXT_DEFENSE_EXP_STAT] = DefenseGoalExperience - GetExperience(DefenseLevel - 10, ExpType.Stat);
            stats[StatsType.DEFENSE_LEVEL_STAT] = DefenseLevel;

            stats[StatsType.FAME_STAT] = CurrentFame;
            stats[StatsType.CURR_FAME_STAT] = Fame;
            stats[StatsType.NEXT_CLASS_QUEST_FAME_STAT] = FameGoal;
            stats[StatsType.NUM_STARS_STAT] = Stars;

            stats[StatsType.GUILD_NAME_STAT] = Guild;
            stats[StatsType.GUILD_RANK_STAT] = GuildRank;

            stats[StatsType.CREDITS_STAT] = Credits;
            stats[StatsType.FORTUNE_TOKEN_STAT] = Tokens;
            stats[StatsType.NAME_CHOSEN_STAT] = NameChosen ? 1 : 0;
            stats[StatsType.TEX1_STAT] = Texture1;
            stats[StatsType.TEX2_STAT] = Texture2;

            if (Glowing)
                stats[StatsType.GLOW_COLOR_STAT] = 1;

            if (AccountType >= (int)Core.config.AccountType.VIP)
                stats[StatsType.GLOW_COLOR_STAT] = new Random().Next(0x000000, 0xffffff);

            stats[StatsType.HP_STAT] = HP;
            stats[StatsType.MP_STAT] = MP;

            stats[StatsType.INVENTORY_0_STAT] = Inventory[0]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_1_STAT] = Inventory[1]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_2_STAT] = Inventory[2]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_3_STAT] = Inventory[3]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_4_STAT] = Inventory[4]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_5_STAT] = Inventory[5]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_6_STAT] = Inventory[6]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_7_STAT] = Inventory[7]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_8_STAT] = Inventory[8]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_9_STAT] = Inventory[9]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_10_STAT] = Inventory[10]?.ObjectType ?? -1;
            stats[StatsType.INVENTORY_11_STAT] = Inventory[11]?.ObjectType ?? -1;

            if (Boost == null)
                CalculateBoost();

            if (Boost != null)
            {
                stats[StatsType.MAX_HP_STAT] = Stats[0] + Boost[0];
                stats[StatsType.MAX_MP_STAT] = Stats[1] + Boost[1];
                stats[StatsType.ATTACK_STAT] = Stats[2] + Boost[2];
                stats[StatsType.DEFENSE_STAT] = Stats[3] + Boost[3];
                stats[StatsType.SPEED_STAT] = Stats[4] + Boost[4];
                stats[StatsType.VITALITY_STAT] = Stats[5] + Boost[5];
                stats[StatsType.WISDOM_STAT] = Stats[6] + Boost[6];
                stats[StatsType.DEXTERITY_STAT] = Stats[7] + Boost[7];

                stats[StatsType.MAX_HP_BOOST_STAT] = Boost[0];
                stats[StatsType.MAX_MP_BOOST_STAT] = Boost[1];
                stats[StatsType.ATTACK_BOOST_STAT] = Boost[2];
                stats[StatsType.DEFENSE_BOOST_STAT] = Boost[3];
                stats[StatsType.SPEED_BOOST_STAT] = Boost[4];
                stats[StatsType.VITALITY_BOOST_STAT] = Boost[5];
                stats[StatsType.WISDOM_BOOST_STAT] = Boost[6];
                stats[StatsType.DEXTERITY_BOOST_STAT] = Boost[7];
            }

            stats[StatsType.SIZE_STAT] = (int)((Resize16x16Skins.IsSkin16x16Type(PlayerSkin) ? 70 : setTypeSkin?.Size ?? Size) * ((Client?.Character?.Size ?? 100) / 100.0f));

            stats[StatsType.HASBACKPACK_STAT] = HasBackpack.GetHashCode();

            stats[StatsType.BACKPACK_0_STAT] = HasBackpack ? (Inventory[12]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_1_STAT] = HasBackpack ? (Inventory[13]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_2_STAT] = HasBackpack ? (Inventory[14]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_3_STAT] = HasBackpack ? (Inventory[15]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_4_STAT] = HasBackpack ? (Inventory[16]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_5_STAT] = HasBackpack ? (Inventory[17]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_6_STAT] = HasBackpack ? (Inventory[18]?.ObjectType ?? -1) : -1;
            stats[StatsType.BACKPACK_7_STAT] = HasBackpack ? (Inventory[19]?.ObjectType ?? -1) : -1;

            stats[StatsType.TEXTURE_STAT] = setTypeSkin?.SkinType ?? PlayerSkin;
            stats[StatsType.HEALTH_POTION_STACK_STAT] = HealthPotions;
            stats[StatsType.MAGIC_POTION_STACK_STAT] = MagicPotions;

            if (Owner?.Name == "Ocean Trench")
                stats[StatsType.BREATH_STAT] = OxygenBar;

            stats[StatsType.XP_BOOSTED_STAT] = XpBoosted ? 1 : 0;
            stats[StatsType.XP_TIMER_STAT] = (int)XpBoostTimeLeft;
            stats[StatsType.LD_TIMER_STAT] = (int)LootDropBoostTimeLeft;
            stats[StatsType.LT_TIMER_STAT] = (int)LootTierBoostTimeLeft;

            stats[StatsType.ACCOUNT_TYPE] = AccountType;
            stats[StatsType.ADMIN] = Admin;

            stats[StatsType.PET_OBJECT_ID] = PetID;

            if (PetID != 0)
            {
                try
                {
                    if (PetHealing != null)
                    {
                        stats[StatsType.PET_HP_HEALING_AVERAGE_MIN] = PetHealing[0][0];
                        stats[StatsType.PET_HP_HEALING_AVERAGE_MAX] = PetHealing[0][1];
                        stats[StatsType.PET_HP_HEALING_AVERAGE_BONUS] = PetHealing[0][2];
                        stats[StatsType.PET_MP_HEALING_AVERAGE_MIN] = PetHealing[1][0];
                        stats[StatsType.PET_MP_HEALING_AVERAGE_MAX] = PetHealing[1][1];
                        stats[StatsType.PET_MP_HEALING_AVERAGE_BONUS] = PetHealing[1][2];
                    }
                    if (PetAttack != null)
                    {
                        stats[StatsType.PET_ATTACK_COOLDOWN] = PetAttack[0];
                        stats[StatsType.PET_ATTACK_CHANCE] = PetAttack[1];
                        stats[StatsType.PET_ATTACK_DAMAGE_MIN] = PetAttack[2];
                        stats[StatsType.PET_ATTACK_DAMAGE_MAX] = PetAttack[3];
                    }
                }
                catch { } // just don't return errors, hold this exception without export any value
            }
        }
    }
}