#region

using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm
{
    internal static class Sight
    {
        private static readonly Dictionary<int, IntPoint[]> points = new Dictionary<int, IntPoint[]>();

        public static List<IntPoint> GetSightCircle(int radius)
        {
            if (!points.TryGetValue(radius, out IntPoint[] ret))
            {
                var pts = new List<IntPoint>();

                for (int y = -radius; y <= radius; y++)
                    for (int x = -radius; x <= radius; x++)
                    {
                        if (x * x + y * y <= radius * radius)
                            pts.Add(new IntPoint(x, y));
                    }
                ret = points[radius] = pts.ToArray();
            }

            return ret.ToList();
        }

        public static List<IntPoint> RayCast(Player player, int radius = 15)
        {
            var RayTiles = new List<IntPoint>();
            int angle = 0;
            while (angle < 360)
            {
                int distance = 0;
                while (distance < radius)
                {
                    int x = (int)(distance * Math.Cos(angle));
                    int y = (int)(distance * Math.Sin(angle));
                    if ((x * x + y * y) <= (radius * radius))
                    {
                        RayTiles.Add(new IntPoint(x, y));
                        GameServer.Manager.GameData.ObjectDescs.TryGetValue(player.Owner.Map[(int)player.X + x, (int)player.Y + y].ObjType, out ObjectDesc desc);
                        if (desc != null && desc.BlocksSight)
                            break;
                        RayTiles.Add(new IntPoint(x, y));
                    }
                    distance++;
                }
                angle++;
            }
            return RayTiles;
        }
    }
}