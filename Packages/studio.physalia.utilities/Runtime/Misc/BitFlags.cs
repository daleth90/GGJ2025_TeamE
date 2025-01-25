using System.Collections.Generic;

namespace Physalia
{
    /// <summary>
    /// This class is used for bit flags count over 64.
    /// </summary>
    public class BitFlags : List<long>
    {
        public BitFlags() : base() { }

        public BitFlags(BitFlags other) : base(other) { }

        public BitFlags(int flagCount) : base((flagCount % 64 > 0) ? (flagCount / 64 + 1) : (flagCount / 64)) { }

        public bool IsBitSet(int index)
        {
            int pageIndex = index / 64;
            int bitIndex = index % 64;
            return IsBitSet(pageIndex, bitIndex);
        }

        public bool IsBitSet(int pageIndex, int bitIndex)
        {
            if (pageIndex >= Count)
            {
                return false;
            }

            long page = this[pageIndex];
            return page.IsBitSet(bitIndex);
        }

        public void SetBit(int index, bool value)
        {
            int pageIndex = index / 64;
            int bitIndex = index % 64;
            SetBit(pageIndex, bitIndex, value);
        }

        public void SetBit(int pageIndex, int bitIndex, bool value)
        {
            int pageCount = Count;
            for (var i = pageCount; i <= pageIndex; i++)
            {
                Add(0);
            }

            long page = this[pageIndex];
            page.SetBit(bitIndex, value);
            this[pageIndex] = page;
        }
    }
}
