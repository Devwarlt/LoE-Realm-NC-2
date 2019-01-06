#region

using LoESoft.GameServer.realm.terrain;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm.mapsetpiece
{
    public enum DaysOfWeek : byte
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        None = 7
    }

    public class SetPiece
    {
        public string Name { get; private set; }
        public MapSetPiece MapSetPiece { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        public WmapTerrain[] WmapTerrain { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public bool IsWeeklyEvent { get; private set; }

        public SetPiece(
            string Name,
            MapSetPiece MapSetPiece,
            int Min,
            int Max,
            WmapTerrain[] WmapTerrain,
            DaysOfWeek DayOfWeek = DaysOfWeek.None
            )
        {
            this.Name = Name;
            this.MapSetPiece = MapSetPiece;
            this.Min = Min;
            this.Max = Max;
            this.WmapTerrain = WmapTerrain;

            if (DayOfWeek != DaysOfWeek.None)
            {
                IsWeeklyEvent = true;
                this.DayOfWeek = (DayOfWeek)DayOfWeek;
            }
            else
                IsWeeklyEvent = false;
        }
    }

    public class SetPieces
    {
        public static readonly List<SetPiece> SetPieceCache = new List<SetPiece>
        {
            new SetPiece("Building", new Building(), 10, 50, new WmapTerrain[3] { WmapTerrain.LowForest, WmapTerrain.LowPlains, WmapTerrain.MidForest }),
            new SetPiece("Graveyard", new Graveyard(), 10, 25, new WmapTerrain[2] {WmapTerrain.LowSand, WmapTerrain.LowPlains }),
            new SetPiece("Grove", new Grove(), 5, 10, new WmapTerrain[2] { WmapTerrain.MidForest, WmapTerrain.MidPlains }),
            new SetPiece("Lich Temple", new LichyTemple(), 2, 5, new WmapTerrain[2] { WmapTerrain.MidForest, WmapTerrain.MidPlains }),
            new SetPiece("Ghost King Castle", new Castle(), 1, 3, new WmapTerrain[2] { WmapTerrain.HighForest, WmapTerrain.HighPlains }),
            new SetPiece("Tower", new Tower(), 5, 10, new WmapTerrain[2] { WmapTerrain.HighForest, WmapTerrain.HighPlains }),
            new SetPiece("Temple Type A", new TempleA(), 5, 15, new WmapTerrain[2] { WmapTerrain.MidForest, WmapTerrain.MidPlains }),
            new SetPiece("Temple Type B", new TempleB(), 5, 15, new WmapTerrain[2] { WmapTerrain.MidForest, WmapTerrain.MidPlains }),
            new SetPiece("Oasis", new Oasis(), 0, 3, new WmapTerrain[2] { WmapTerrain.LowSand, WmapTerrain.MidSand }),
            new SetPiece("Pyre", new Pyre(), 0, 3, new WmapTerrain[2] { WmapTerrain.MidSand, WmapTerrain.HighSand }),
            new SetPiece("Lava Fissure", new LavaFissure(), 2, 6, new WmapTerrain[1] { WmapTerrain.Mountains }),
            new SetPiece("Dream Island Horde", new DreamIsle(), 3, 7, new WmapTerrain[2] { WmapTerrain.HighForest, WmapTerrain.HighPlains })
        };

        public static void ApplySetPieces(World world)
        {
            Wmap map = world.Map;
            int w = map.Width, h = map.Height;

            Random rand = new Random();
            HashSet<Rect> rects = new HashSet<Rect>();

            foreach (SetPiece setpiece in SetPieceCache)
            {
                int size = setpiece.MapSetPiece.Size;
                int count = rand.Next(setpiece.Min, setpiece.Max);

                if (setpiece.IsWeeklyEvent)
                    if (setpiece.DayOfWeek != DateTime.Now.DayOfWeek)
                        continue;

                for (int i = 0; i < count; i++)
                {
                    IntPoint pt = new IntPoint();
                    Rect rect;

                    do
                    {
                        pt.X = rand.Next(0, w);
                        pt.Y = rand.Next(0, h);
                        rect = new Rect { x = pt.X, y = pt.Y, w = size, h = size };
                    } while ((Array.IndexOf(setpiece.WmapTerrain, map[pt.X, pt.Y].Terrain) == -1 || rects.Any(_ => Rect.Intersects(rect, _))));

                    setpiece.MapSetPiece.RenderSetPiece(world, pt);

                    rects.Add(rect);
                }
            }
        }

        private struct Rect
        {
            public int h;
            public int w;
            public int x;
            public int y;

            public static bool Intersects(Rect r1, Rect r2)
            {
                return !(r2.x > r1.x + r1.w ||
                         r2.x + r2.w < r1.x ||
                         r2.y > r1.y + r1.h ||
                         r2.y + r2.h < r1.y);
            }
        }

        public static int[,] RotateCW(int[,] mat)
        {
            int M = mat.GetLength(0);
            int N = mat.GetLength(1);
            int[,] ret = new int[N, M];
            for (int r = 0; r < M; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    ret[c, M - 1 - r] = mat[r, c];
                }
            }
            return ret;
        }

        public static int[,] ReflectVert(int[,] mat)
        {
            int M = mat.GetLength(0);
            int N = mat.GetLength(1);
            int[,] ret = new int[M, N];
            for (int x = 0; x < M; x++)
                for (int y = 0; y < N; y++)
                    ret[x, N - y - 1] = mat[x, y];
            return ret;
        }

        public static int[,] ReflectHori(int[,] mat)
        {
            int M = mat.GetLength(0);
            int N = mat.GetLength(1);
            int[,] ret = new int[M, N];
            for (int x = 0; x < M; x++)
                for (int y = 0; y < N; y++)
                    ret[M - x - 1, y] = mat[x, y];
            return ret;
        }
    }
}