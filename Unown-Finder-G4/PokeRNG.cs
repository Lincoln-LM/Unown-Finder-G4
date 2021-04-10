using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unown_Finder_G4
{
    internal class PokeRNG
    {
        public uint seed;

        public PokeRNG(uint seed)
        {
            this.seed = seed;
        }

        public uint nextUInt()
        {
            seed = seed * 0x41c64e6d + 0x6073;
            return seed;
        }
        public ushort nextUShort()
        {
            return (ushort)(nextUInt() >> 16);
        }
    }
}
