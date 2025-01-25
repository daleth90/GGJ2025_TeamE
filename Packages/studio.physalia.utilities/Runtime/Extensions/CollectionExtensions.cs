using System;
using System.Collections.Generic;

namespace Physalia
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                hashSet.Add(item);
            }
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T value) where T : IEquatable<T>
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
