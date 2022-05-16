using System.Collections.Generic;
using System.Linq;

namespace Erabikata.Backend;

public class EnumerableComparer<T, E> : IEqualityComparer<E> where E : IEnumerable<T>
{
    public bool Equals(E? x, E? y)
    {
        return ReferenceEquals(x, y) || x != null && y != null && x.SequenceEqual(y);
    }

    public int GetHashCode(E obj)
    {
        // Will not throw an OverflowException
        unchecked
        {
            return obj.Where(e => e != null)
                .Select(e => e!.GetHashCode())
                .Aggregate(17, (a, b) => 23 * a + b);
        }
    }
}
