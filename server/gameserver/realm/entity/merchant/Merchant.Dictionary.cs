using System;
using System.Collections.Generic;

namespace LoESoft.GameServer.realm.entity.merchant
{
    partial class Merchant
    {
        internal static class BLACKLIST
        {
            internal static readonly int[] keys =
            {
                1897, 12288, 12289, 12290, 29035, 3466, 538, 887, 2996, 2998, 1601, 2355, 5705, 3285, 1544, 1563, 1584, 1576, 3311,
                3133, 8848, 28645, 0x575a
            };

            internal static readonly string[] eggs =
            {
                ""
            };

            internal static readonly string[] weapons =
            {
                "Bow of Eternal Frost",
                "Frostbite",
                "Present Dispensing Wand",
                "An Icicle",
                "Staff of Yuletide Carols",
                "Salju"
            };

            internal static readonly string[] small =
            {
                "Small Ivory Dragon Scale Cloth",
                "Small Green Dragon Scale Cloth",
                "Small Midnight Dragon Scale Cloth",
                "Small Blue Dragon Scale Cloth",
                "Small Red Dragon Scale Cloth",
                "Small Jester Argyle Cloth",
                "Small Alchemist Cloth",
                "Small Mosaic Cloth",
                "Small Spooky Cloth",
                "Small Flame Cloth",
                "Small Heavy Chainmail Cloth"
            };

            internal static readonly string[] large =
            {
                "Large Ivory Dragon Scale Cloth",
                "Large Green Dragon Scale Cloth",
                "Large Midnight Dragon Scale Cloth",
                "Large Blue Dragon Scale Cloth",
                "Large Red Dragon Scale Cloth",
                "Large Jester Argyle Cloth",
                "Large Alchemist Cloth",
                "Large Mosaic Cloth",
                "Large Spooky Cloth",
                "Large Flame Cloth",
                "Large Heavy Chainmail Cloth"
            };
        }

        public static readonly Dictionary<int, Tuple<int, CurrencyType>> prices = new Dictionary<int, Tuple<int, CurrencyType>>
        {
            #region "Region 1 & 2"
            
            { 0x236E, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // glife
            { 0x236F, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gmana
            { 0x2368, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gatt
            { 0x2369, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gdef
            { 0x236A, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gspd
            { 0x236D, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gdex
            { 0x236B, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gvit
            { 0x236C, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // gwis

            { 0xae9, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //life
            { 0xaea, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //mana
            { 0xa1f, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //att
            { 0xa20, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //def
            { 0xa21, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //spd
            { 0xa4c, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //dex
            { 0xa34, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //vit
            { 0xa35, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, //wis

            //{ 1793, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Undead Lair Key
            //{ 308, new Tuple<int, CurrencyType>(250, CurrencyType.Fame) }, // Halloween Cemetery Key
            //{ 1797, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Pirate Cave Key
            //{ 1798, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Spider Den Key
            //{ 1802, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Abyss of Demons Key
            //{ 1803, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Snake Pit Key
            //{ 1808, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // Tomb of the Ancients Key
            //{ 1823, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Sprite World Key
            //{ 3089, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // Ocean Trench Key
            //{ 3097, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Totem Key
            //{ 29836, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Ice Cave Key
            //{ 3107, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Manor Key
            //{ 3118, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Davy's Key
            //{ 3119, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Lab Key
            //{ 3170, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // Candy Key
            //{ 3183, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Cemetery Key
            //{ 3284, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Draconis Key
            //{ 3277, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Forest Maze Key
            //{ 3279, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Woodland Labyrinth Key
            //{ 3278, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Deadwater Docks Key
            //{ 3290, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // The Crawling Depths Key
            //{ 3293, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // Shatters Key
            //{ 8852, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Shaitan's Key
            //{ 9042, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Theatre Key
            //{ 29804, new Tuple<int, CurrencyType>(200, CurrencyType.Fame) }, // Puppet Master's Encore Key
            //{ 573, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Toxic Sewers Key
            //{ 283, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // The Hive Key
            //{ 32695, new Tuple<int, CurrencyType>(250, CurrencyType.Fame) }, // Ice Tomb Key
            //{ 303, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Mountain Temple Key
            #endregion "Region 1 & 2"

            #region "Region 4"
            { 0x32a, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame) }, // Char Slot Unlocker
            { 0x32b, new Tuple<int, CurrencyType>(2500, CurrencyType.Fame) } // Vault Chest Unlocker

            //{ 3273, new Tuple<int, CurrencyType>(20, CurrencyType.Fame) }, // Soft Drink
            //{ 3275, new Tuple<int, CurrencyType>(50, CurrencyType.Fame) }, // Fries
            //{ 3270, new Tuple<int, CurrencyType>(100, CurrencyType.Fame) }, // Great Taco
            //{ 3269, new Tuple<int, CurrencyType>(150, CurrencyType.Fame) }, // Power Pizza
            //{ 3268, new Tuple<int, CurrencyType>(240, CurrencyType.Fame) }, // Chocolate Cream Sandwich Cookie
            //{ 3274, new Tuple<int, CurrencyType>(330, CurrencyType.Fame) }, // Grapes of Wrath
            //{ 3272, new Tuple<int, CurrencyType>(450, CurrencyType.Fame) }, // Superburger
            //{ 3271, new Tuple<int, CurrencyType>(700, CurrencyType.Fame) }, // Double Cheeseburger Deluxe
            //{ 3276, new Tuple<int, CurrencyType>(1000, CurrencyType.Fame) }, // Ambrosia
            //{ 3280, new Tuple<int, CurrencyType>(40, CurrencyType.Fame) }, // Cranberries
            //{ 3281, new Tuple<int, CurrencyType>(60, CurrencyType.Fame) }, // Ear of Corn
            //{ 3282, new Tuple<int, CurrencyType>(90, CurrencyType.Fame) }, // Sliced Yam
            //{ 3283, new Tuple<int, CurrencyType>(120, CurrencyType.Fame) }, // Pumpkin Pie
            //{ 3286, new Tuple<int, CurrencyType>(300, CurrencyType.Fame) } // Thanksgiving Turkey
            #endregion "Region 4"
        };
    }
}