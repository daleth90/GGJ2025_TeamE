using System;

namespace Physalia
{
    public static class BitExtensions
    {
        public static bool IsBitSet(this byte flags, int bit)
        {
            return (flags & (1 << bit)) == (1 << bit);
        }

        public static byte SetBit(ref this byte flags, int bit, bool value)
        {
            if (value == true)
            {
                return flags |= (byte)(1 << bit);
            }
            else
            {
                return flags &= unchecked((byte)~(1 << bit));
            }
        }

        public static bool IsBitSet(this int flags, int bit)
        {
            return (flags & (1 << bit)) == (1 << bit);
        }

        public static int SetBit(ref this int flags, int bit, bool value)
        {
            if (value == true)
            {
                return flags |= 1 << bit;
            }
            else
            {
                return flags &= ~(1 << bit);
            }
        }

        public static bool IsBitSet(this long flags, int bit)
        {
            return (flags & (1L << bit)) == (1L << bit);
        }

        public static long SetBit(this ref long flags, int bit, bool value)
        {
            if (value == true)
            {
                return flags |= 1L << bit;
            }
            else
            {
                return flags &= ~(1L << bit);
            }
        }

        public static string ToBitwise(this byte flags)
        {
            string result = Convert.ToString(flags, 2).PadLeft(8, '0');
            return $"Bitwise: {flags} => 0x{result}";
        }

        public static string ToBitwise(this int flags)
        {
            string result = Convert.ToString(flags, 2).PadLeft(32, '0');
            for (var i = 8; i < result.Length; i += 9)  // Seperate with ' ' every 8 bits
            {
                result = result.Insert(i, " ");
            }
            return $"Bitwise: {flags} => 0x{result}";
        }

        public static string ToBitwise(this long flags)
        {
            string result = Convert.ToString(flags, 2).PadLeft(64, '0');
            for (var i = 8; i < result.Length; i += 9)  // Seperate with ' ' every 8 bits
            {
                result = result.Insert(i, " ");
            }
            return $"Bitwise: {flags} => 0x{result}";
        }
    }
}
