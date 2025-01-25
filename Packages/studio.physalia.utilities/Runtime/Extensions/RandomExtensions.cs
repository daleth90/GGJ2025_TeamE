using System;
using System.Collections.Generic;

namespace Physalia
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int currentIndex = list.Count;
            while (currentIndex > 1)
            {
                currentIndex--;
                int randomIndex = random.Next(currentIndex + 1);

                // Swap
                T temp = list[randomIndex];
                list[randomIndex] = list[currentIndex];
                list[currentIndex] = temp;
            }
        }

        public static T RandomPick<T>(this IReadOnlyList<T> list, Random random)
        {
            int randomIndex = random.Next(list.Count);
            return list[randomIndex];
        }

        public static List<T> RandomPick<T>(this IReadOnlyList<T> list, int number, Random random)
        {
            var buffer = new List<T>(list);
            buffer.Shuffle(random);

            for (int i = buffer.Count - 1; i >= number; i--)
            {
                buffer.RemoveAt(i);
            }

            return buffer;
        }
    }
}
