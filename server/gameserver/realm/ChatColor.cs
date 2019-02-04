using LoESoft.Core.config;
using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;

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
        private Player _player { get; set; }

        public ChatColor(Player player) => _player = player;

        public (int stars, int name, int text) GetColor()
        {
            if (_player.AccountType >= (int)AccountType.MOD)
            {
                if (specialColors.TryGetValue(_player.AccountType, out (int, int) color))
                    return (_player.Stars, color.Item1, color.Item2);
            }

            if (_player.AccountType == (int)AccountType.VIP)
            {
                foreach (var i in regularColors)
                    if (_player.Stars.Within(i.Key.Item1, i.Key.Item2))
                        return (_player.Stars, i.Value.Item1, i.Value.Item2);
            }

            return (_player.Stars, 0x123456, 0x123456);
        }

        private readonly Dictionary<(int, int), (int, int)> regularColors = new Dictionary<(int, int), (int, int)>
        {
            { (0, 13), (0x8997dd, 0xe7eaf8) },
            { (14, 27), (0x304cda, 0xd5dbf7) },
            { (28, 41), (0xc0262c, 0xf2d3d4) },
            { (42, 55), (0xf6921d , 0xfde9d1) },
            { (56, 69), (0xffff00, 0xffffcc) },
            { (70, 70), (0xcccccc, 0xffffff) }
        };

        private readonly Dictionary<int, (int, int)> specialColors = new Dictionary<int, (int, int)>
        {
            { (int)AccountType.MOD, (0xdc1f1f, 0xf8d2d2) },
            { (int)AccountType.DEVELOPER, (0x8e28eb, 0xe8d4fb) },
            { (int)AccountType.ADMIN, (0x9acd32, 0xeaf5d6) }
        };
    }
}