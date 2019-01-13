#region

using System;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.entity.merchant
{
    partial class Merchant
    {
        private const int BUY_NO_GOLD = 3;
        private const int BUY_NO_FAME = 6;
        private const int BUY_NO_FORTUNETOKENS = 9;
        private const int MERCHANT_SIZE = 100;
        private bool closing;
        private bool newMerchant;
        private int tickcount;
#pragma warning disable CS0108 // 'Merchant.Random' hides inherited member 'Entity.Random'. Use the new keyword if hiding was intended.
        public static Random Random { get; private set; }
#pragma warning restore CS0108 // 'Merchant.Random' hides inherited member 'Entity.Random'. Use the new keyword if hiding was intended.
        private static List<KeyValuePair<string, int>> AddedTypes { get; set; }
        public bool Custom { get; set; }
        public int MType { get; set; }
        public int MRemaining { get; set; }
        public int MTime { get; set; }
        public int Discount { get; set; }
        public static int[] region1list;
        public static int[] region2list;
        public static int[] region3list;
        public static int[] region4list;
        public static int[] region5list;
        public static int[] region6list;
        public static int[] region7list;
        public static int[] region8list;
        public static int[] region9list;
        public static int[] region10list;
      //  public static int[] smallclothlist;
     //   public static int[] accessorylist;
     //   public static int[] largeclothlist;
     //   public static int[] clothinglist;
        internal static readonly List<int> weaponSlotType = new List<int> { 1, 2, 3, 8, 17, 24 };
        internal static readonly List<int> abilitySlotType = new List<int> { 4, 5, 11, 12, 13, 15, 16, 18, 19, 20, 21, 22, 23, 25 };
        internal static readonly List<int> armorSlotType = new List<int> { 6, 7, 14 };
        internal static readonly List<int> ringSlotType = new List<int> { 9 };
    }
}