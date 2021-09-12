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
    }
}
