using System.Collections.Generic;
using System.Linq;

namespace Election.Extensions
{
    public static class IEnumerableHasDuplicates
    {
        public static bool HasDuplicates<T>(this IEnumerable<T> collection)
        {
            var list = collection.ToList();
            var set = new HashSet<T>(list);

            return set.Count() < list.Count();
        }
    }
}