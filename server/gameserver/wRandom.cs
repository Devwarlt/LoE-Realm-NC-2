#region

using System;

#endregion

namespace LoESoft.GameServer
{
    public class wRandom
    {
        private uint seed;

        public wRandom() : this((uint)Environment.TickCount)
        {
        }

        public wRandom(uint seed)
        {
            this.seed = seed;
        }

        public uint NextInt()
        {
            return Gen();
        }

        private uint Gen()
        {
            uint lb = 16807 * (seed & 0xFFFF);
            uint hb = 16807 * (uint)((int)seed >> 16);
            lb = lb + ((hb & 32767) << 16);
            lb = lb + (uint)((int)hb >> 15);
            if (lb > 2147483647)
            {
                lb = lb - 2147483647;
            }
            return seed = lb;
        }

        public uint CurrentSeed
        {
            get { return seed; }
            set { seed = value; }
        }

        public int Next(int min, int max)
        {
            return (int)(min == max ? min : (min + (Sample() % (max - min))));
        }

        private uint Sample()
        {
            uint lb = 16807 * (seed & 0xFFFF);
            uint hb = 16807 * (seed >> 16);
            lb = lb + ((hb & 32767) << 16);
            lb = lb + (hb >> 15);
            if (lb > 2147483647)
            {
                lb = lb - 2147483647;
            }
            return seed = lb;
        }
    }
}