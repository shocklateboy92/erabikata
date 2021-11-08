using System;
using System.Collections.Generic;

namespace Erabikata.Backend.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(int, T)> WithIndicies<T>(this IEnumerable<T> enumerable)
        {
            var index = 0;
            foreach (var element in enumerable)
            {
                yield return (index, element);
                index++;
            }
        }

        public static IEnumerable<T> WithoutAdjacentDuplicates<T, I>(
            this IEnumerable<T> enumerable,
            Func<T, I> propertySelector
        ) {
            var last = default(I);
            foreach (var item in enumerable)
            {
                var prop = propertySelector(item);
                if (!Equals(last, prop))
                {
                    last = prop;
                    yield return item;
                }
            }
        }
    }
}
