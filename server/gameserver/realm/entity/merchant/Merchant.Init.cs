#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.Core.models;
using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity.merchant
{
    partial class Merchant : SellableObject
    {
        public Merchant(ushort objType, World owner = null)
            : base(objType)
        {
            MType = -1;
            Size = MERCHANT_SIZE;
            if (owner != null)
                Owner = owner;

            if (Random == null)
                Random = new Random();
            if (AddedTypes == null)
                AddedTypes = new List<KeyValuePair<string, int>>();
            if (owner != null)
                ResolveMType();
        }

        public override void Init(World owner)
        {
            base.Init(owner);
            ResolveMType();
            UpdateCount++;
            if (MType == -1)
                Owner.LeaveWorld(this);
        }

        public override void Tick(RealmTime time)
        {
            try
            {
                if (Size == 0 && MType != -1)
                {
                    Size = MERCHANT_SIZE;
                    UpdateCount++;
                }

                if (!closing)
                {
                    tickcount++;
                    if (tickcount % (GameServer.Manager.TPS * 60) == 0) //once per minute after spawning
                    {
                        MTime--;
                        UpdateCount++;
                    }
                }

                if (MRemaining == 0 && MType != -1)
                {
                    if (AddedTypes.Contains(new KeyValuePair<string, int>(Owner.Name, MType)))
                        AddedTypes.Remove(new KeyValuePair<string, int>(Owner.Name, MType));
                    Recreate(this);
                    UpdateCount++;
                }

                if (MTime == -1 && Owner != null)
                {
                    if (AddedTypes.Contains(new KeyValuePair<string, int>(Owner.Name, MType)))
                        AddedTypes.Remove(new KeyValuePair<string, int>(Owner.Name, MType));
                    Recreate(this);
                    UpdateCount++;
                }

                if (MTime == 1 && !closing)
                {
                    closing = true;
                    Owner?.Timers.Add(new WorldTimer(30 * 1000, (w1, t1) =>
                    {
                        MTime--;
                        UpdateCount++;
                        w1.Timers.Add(new WorldTimer(30 * 1000, (w2, t2) =>
                        {
                            MTime--;
                            UpdateCount++;
                        }));
                    }));
                }

                if (MType == -1)
                    Owner?.LeaveWorld(this);

                base.Tick(time);
            }
            catch (Exception) { }
        }

        public void ResolveMType()
        {
            MType = -1;
            var list = new int[0];
            if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_1)
                list = region1list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_2)
                list = region2list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_3)
                list = region3list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_4)
                list = region4list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_5)
                list = region5list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_6)
                list = region6list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_7)
                list = region7list;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_8)
                list = region8list;
            /*else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_12)
                list = accessorylist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_13)
                list = largeclothlist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_14)
                list = smallclothlist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_15)
                list = clothinglist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_16)
                list = accessorylist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_17)
                list = largeclothlist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_18)
                list = smallclothlist;
            else if (Owner.Map[(int)X, (int)Y].Region == TileRegion.Store_19)
                list = clothinglist*/
            ;

            if (AddedTypes == null)
                AddedTypes = new List<KeyValuePair<string, int>>();
            list.Shuffle();
            foreach (var t1 in list.Where(t1 => !AddedTypes.Contains(new KeyValuePair<string, int>(Owner.Name, t1))))
            {
                AddedTypes.Add(new KeyValuePair<string, int>(Owner.Name, t1));
                MType = t1;
                MTime = Random.Next(6, 15);
                MRemaining = Random.Next(6, 11);
                newMerchant = true;
                Owner.Timers.Add(new WorldTimer(30000, (w, t) =>
                {
                    newMerchant = false;
                    UpdateCount++;
                }));

                var s = Random.Next(0, 100);

                if (s < 2 * Settings.EVENT_RATE)
                    Discount = 50;
                else if (s < 5 * Settings.EVENT_RATE)
                    Discount = 25;
                else if (s < 10 * Settings.EVENT_RATE)
                    Discount = 15;
                else if (s < 15 * Settings.EVENT_RATE)
                    Discount = 10;
                else
                    Discount = 0;

                if (prices.TryGetValue(MType, out Tuple<int, CurrencyType> price))
                {
                    if (Discount != 0)
                        Price = (int)(price.Item1 - (price.Item1 * ((double)Discount / 100))) < 1 ?
                            price.Item1 : (int)(price.Item1 - (price.Item1 * ((double)Discount / 100)));
                    else
                        Price = price.Item1;
                    Currency = price.Item2;
                }

                break;
            }
            UpdateCount++;
        }

        /// <summary>
        /// Return to log formatted structure for price dictionary
        /// </summary>
        /// <param name="objtype"></param>
        /// <param name="objid"></param>
        /// <param name="price"></param>
        private static void _data(int objtype, string objid, int price = 0) => Log.Info("{ " + objtype + ", new Tuple<int, CurrencyType>(" + price + ", CurrencyType.Gold) }, // " + objid);

        /// <summary>
        /// Return price based in feedpower
        /// </summary>
        /// <param name="fp"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static int Egg(int fp, string name)
        {
            switch (fp)
            {
                case 300:
                    return 300;

                case 700:
                    return name.Contains("Mystery") ? 1000 : (name.Contains("Humanoid") ? 2000 : 1200);

                default:
                    return 3500;
            }
        }

        /// <summary>
        /// Return price based in tier
        /// </summary>
        /// <param name="tier"></param>
        /// <returns></returns>
        private static int Ability(int tier)
        {
            switch (tier)
            {
                case 4:
                    return 175;

                default:
                    return 400;
            }
        }

        /// <summary>
        /// Return price based in tier
        /// </summary>
        /// <param name="tier"></param>
        /// <param name="slottype"></param>
        /// <returns></returns>
        private static int Weapon(int tier, int slottype)
        {
            switch (tier)
            {
                case 8:
                    return 51;

                case 9:
                    return 150;

                case 10:
                    return 225;

                case 11:
                    return 450;

                default:
                    return Processtops(slottype, "weapon");
            }
        }

        /// <summary>
        /// Return price based in tier
        /// </summary>
        /// <param name="tier"></param>
        /// <param name="slottype"></param>
        /// <returns></returns>
        private static int Armor(int tier, int slottype)
        {
            switch (tier)
            {
                case 9:
                    return 51;

                case 10:
                    return 100;

                case 11:
                    return 225;

                case 12:
                    return 425;

                default:
                    return Processtops(slottype, "armor");
            }
        }

        private static int Ring(int tier, string name)
        {
            switch (tier)
            {
                case 4:
                    return (name.Contains("Health") || name.Contains("Magic") || name.Contains("Defense")) ? 250 : 100;

                default:
                    return (name.Contains("Health") || name.Contains("Magic") || name.Contains("Defense")) ? 400 : 200;
            }
        }

        /// <summary>
        /// Return price based in tops slot type
        /// </summary>
        /// <param name="slottype"></param>
        /// <returns></returns>
        private static int Processtops(int slottype, string type)
        {
            if (type == "weapon")
            {
                if (slottype == 12 || slottype == 17) // Sword of Acclaim and Staff of the Cosmic Whole
                    return 900;
                else if (slottype == 24) // Masamune
                    return 700;
                else if (slottype == 2) // Dagger of Foul Malevolence
                    return 650;
                else if (slottype == 3) // Bow of Covert Havens
                    return 600;
                else // Wand of Recompense
                    return 550;
            }
            else if (type == "armor")
            {
                if (slottype == 6) // Hydra Skin Armor
                    return 800;
                else // Acropolis Armor and Robe of the Grand Sorcerer
                    return 850;
            }
            else
                return int.MaxValue;
        }

        /// <summary>
        /// Nexus Shop
        /// Total of 8 regions
        /// - Region 1: keys            [ok]
        /// - Region 2: keys            [ok]
        /// - Region 3: skins           [ok]
        /// - Region 4: foods           [ok]
        /// - Region 5: eggs            [ok]
        /// - Region 6: abilities       [ok]
        /// - Region 7: weapons         [ok]
        /// - Region 8: armors + rings  [ok]
        /// </summary>
        /// <param name="data"></param>

        public static void HandleMerchant(EmbeddedData data)
        {
            List<int> region1list = new List<int>();
            List<int> region2list = new List<int>();
            List<int> region3list = new List<int>();
            List<int> region4list = new List<int>();
            List<int> region5list = new List<int>();
            List<int> region6list = new List<int>();
            List<int> region7list = new List<int>();
            List<int> region8list = new List<int>();
            List<int> accessorylist = new List<int>();
            List<int> clothinglist = new List<int>();
            List<int> smallclothlist = new List<int>();
            List<int> largeclothlist = new List<int>();

            // region 1 + region 2
            //foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => BLACKLIST.keys.All(i => (i != _.Value.ObjectType))))
            //    if (item.Value.SlotType == 10 && item.Value.ObjectId.Contains("Key") && item.Value.Class == "Equipment" && item.Value.Soulbound && item.Value.Consumable)
            //    {
            //        region1list.Add(item.Value.ObjectType);
            //        region2list.Add(item.Value.ObjectType);
            //    }
            region1list.Add(0x236E); // glife
            region1list.Add(0x236F); // gmana
            region1list.Add(0x2368); // gatt
            region1list.Add(0x2369); // gdef
            region1list.Add(0x236A); // gspd
            region1list.Add(0x236D); // gdex
            region1list.Add(0x236B); // gvit
            region1list.Add(0x236C); // gwis
            region2list.Add(0xae9); // life
            region2list.Add(0xaea); // mana
            region2list.Add(0xa1f); // att
            region2list.Add(0xa20); // def
            region2list.Add(0xa21); // spd
            region2list.Add(0xa4c); // dex
            region2list.Add(0xa34); // vit
            region2list.Add(0xa35); // wis

            // region 3
            foreach (KeyValuePair<ushort, Item> item in data.Items)
                if (item.Value.SlotType == 10 && item.Value.ObjectId.Contains("(SB)") && item.Value.ObjectId.Contains("Skin") && item.Value.Class == "Equipment" && item.Value.Soulbound && item.Value.Consumable)
                {
                    region3list.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(1000, CurrencyType.Fame));
                }

            // region 4
            //foreach (KeyValuePair<ushort, Item> item in data.Items)
            //    if (item.Value.SlotType == 10 && item.Value.Description.Contains("Food for your pet.") && item.Value.Soulbound)
            //        region4list.Add(item.Value.ObjectType);
            region4list.Add(0x32a);
            //region4list.Add(0x32b);
            region4list.Add(0xc6c);
            region4list.Add(0x575a);

            // region 5
            foreach (KeyValuePair<ushort, Item> item in data.Items)
                if (item.Value.SlotType == 26 && item.Value.ObjectId.Contains("Egg") && item.Value.Consumable && item.Value.Soulbound && item.Value.FeedPower >= 300 && (item.Value.Tier < 3 || item.Value.ObjectId.Contains("Mystery")))
                {
                    region5list.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(Egg(item.Value.FeedPower, item.Value.ObjectId), CurrencyType.Fame));
                }

            // region 6
            foreach (KeyValuePair<ushort, Item> item in data.Items)
                if (abilitySlotType.Contains(item.Value.SlotType) && !item.Value.Soulbound && item.Value.Tier >= 5 && item.Value.Tier <= 6)
                {
                    region6list.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(Ability(item.Value.Tier), CurrencyType.Fame));
                }

            // region 7
            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => BLACKLIST.weapons.All(i => (i != _.Value.ObjectId))))
                if (!item.Value.ObjectId.Contains("Infected") && weaponSlotType.Contains(item.Value.SlotType) && !item.Value.Soulbound && item.Value.Tier >= 8 && item.Value.Tier <= 12)
                {
                    region7list.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(Weapon(item.Value.Tier, item.Value.SlotType), CurrencyType.Fame));
                }

            // region 8
            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => BLACKLIST.weapons.All(i => (i != _.Value.ObjectId))))
                if ((armorSlotType.Contains(item.Value.SlotType) && item.Value.Tier >= 9 && item.Value.Tier <= 13) || (ringSlotType.Contains(item.Value.SlotType) && item.Value.Tier >= 4 && item.Value.Tier <= 5) && !item.Value.Soulbound)
                {
                    region8list.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(item.Value.ObjectId.Contains("Ring") ? Ring(item.Value.Tier, item.Value.ObjectId) : Armor(item.Value.Tier, item.Value.SlotType), CurrencyType.Fame));
                }

            // restricted clothes (small)
            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => BLACKLIST.small.All(i => (i != _.Value.ObjectId))))
                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Cloth") && item.Value.ObjectId.Contains("Small"))
                {
                    smallclothlist.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(100, CurrencyType.Fame));
                }

            // restricted clothes (large)
            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => BLACKLIST.large.All(i => (i != _.Value.ObjectId))))
                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Cloth") && item.Value.ObjectId.Contains("Large"))
                {
                    largeclothlist.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(100, CurrencyType.Fame));
                }

            // accessory dye
            foreach (KeyValuePair<ushort, Item> item in data.Items)
                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Accessory") && item.Value.Class == "Dye")
                {
                    accessorylist.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(50, CurrencyType.Fame));
                }

            // clothing dye
            foreach (KeyValuePair<ushort, Item> item in data.Items)
                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Clothing") && item.Value.Class == "Dye")
                {
                    clothinglist.Add(item.Value.ObjectType);
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(50, CurrencyType.Fame));
                }

            // Regions
            Merchant.region1list = region1list.ToArray();
            Merchant.region2list = region2list.ToArray();
            Merchant.region3list = region3list.ToArray();
            Merchant.region4list = region4list.ToArray();
            Merchant.region5list = region5list.ToArray();
            Merchant.region6list = region6list.ToArray();
            Merchant.region7list = region7list.ToArray();
            Merchant.region8list = region8list.ToArray();

            // Dye
            Merchant.accessorylist = accessorylist.ToArray();
            Merchant.clothinglist = clothinglist.ToArray();

            // Cloth
            Merchant.smallclothlist = smallclothlist.ToArray();
            Merchant.largeclothlist = largeclothlist.ToArray();
        }
    }
}