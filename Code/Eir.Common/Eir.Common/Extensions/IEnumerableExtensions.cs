using System;
using System.Collections.Generic;
using System.Linq;

namespace Eir.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        private static readonly Random _random = new Random();

        public static T PickRandom<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException();
            }

            T[] array = items.ToArray();
            if (array.Length == 0)
            {
                throw new ArgumentException("No items to pick from!");
            }

            return array[_random.Next(array.Length)];
        }

        public static T PickRandomOrDefault<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException();
            }

            T[] array = items.ToArray();
            if (array.Length == 0)
            {
                return default(T);
            }

            return array[_random.Next(array.Length)];
        }

        public static IEnumerable<T[]> BatchesOf<T>(this IEnumerable<T> sequence, int batchSize)
        {
            List<T> batch = new List<T>(batchSize);
            foreach (T obj in sequence)
            {
                batch.Add(obj);
                if (batch.Count >= batchSize)
                {
                    yield return batch.ToArray();
                    batch.Clear();
                }
            }
            if (batch.Count > 0)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }
    }
}