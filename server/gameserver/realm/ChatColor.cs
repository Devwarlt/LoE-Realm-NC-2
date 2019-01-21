using LoESoft.Core.config;
using System.Collections.Generic;
using System.Linq;

namespace LoESoft.GameServer.realm
{
    /// <summary>
    /// Color-Hex:
    /// http://www.color-hex.com/
    ///
    /// FlexTool:
    /// http://www.flextool.com.br/tabela_cores.html
    /// </summary>
    public class ChatColor
    {
        private int _accountType { get; set; }
        private int _stars { get; set; }

        public ChatColor(int stars, int accountType)
        {
            _stars = stars;
            _accountType = accountType;
        }

        public (int stars, int name, int text) GetColor()
        {
            if (_accountType >= (int)AccountType.MOD)
            {
                if (specialColors.TryGetValue(_accountType, out (int, int) color))
                    return (_stars, color.Item1, color.Item2);
            }
            else if (_accountType == (int)AccountType.VIP)
            {
                foreach (var i in regularColors)
                    if (i.Key.Contains(_stars))
                        return (_stars, i.Value.Item1, i.Value.Item2);
            }

            return (_stars, 0x123456, 0x123456);
        }

        private readonly Dictionary<IEnumerable<int>, (int, int)> regularColors = new Dictionary<IEnumerable<int>, (int, int)>
        {
            { Enumerable.Range(0, 13), (0x8997dd, 0xe7eaf8) },
            { Enumerable.Range(14, 27), (0x304cda, 0xd5dbf7) },
            { Enumerable.Range(28, 41), (0xc0262c, 0xf2d3d4) },
            { Enumerable.Range(42, 55), (0xf6921d , 0xfde9d1) },
            { Enumerable.Range(56, 69), (0xffff00, 0xffffcc) },
            { Enumerable.Range(70, 70), (0xcccccc, 0xffffff) }
        };

        private readonly Dictionary<int, (int, int)> specialColors = new Dictionary<int, (int, int)>
        {
            { (int)AccountType.MOD, (0xdc1f1f, 0xf8d2d2) },
            { (int)AccountType.DEVELOPER, (0x8e28eb, 0xe8d4fb) },
            { (int)AccountType.ADMIN, (0x9acd32, 0xeaf5d6) }
        };
    }
}