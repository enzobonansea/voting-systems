using System.Collections.Generic;
using System.Linq;

namespace Election.Extensions
{
    public static class IEnumerableHasDuplicates
    {
        /// <summary>
        /// Determines if a collection has duplicates or not.
        /// Running time complexity: O(n) where n = quantity of items inside collection
        /// </summary>
        public static bool HasDuplicates<T>(this IEnumerable<T> collection)
        {
            var list = collection.ToList();
            var set = list.ToHashSet();

            return set.Count() < list.Count();
        }
    }
}