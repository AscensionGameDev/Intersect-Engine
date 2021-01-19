using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Utilities
{

    public struct SortPair
    {

        public string Key;

        public bool Ascending;

    }

    public enum SortOrder
    {

        Ascending,

        Descending

    }

    public static class SortOrderExtensions
    {

        public static string ToShortString(this SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "asc";

                case SortOrder.Descending:
                    return "desc";

                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            }
        }

        public static string ToLongString(this SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "ascending";

                case SortOrder.Descending:
                    return "descending";

                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            }
        }

        public static bool Matches(this SortOrder sortOrder, string sortOrderString)
        {
            return string.Equals(
                       sortOrder.ToShortString(), sortOrderString, StringComparison.InvariantCultureIgnoreCase
                   ) ||
                   string.Equals(
                       sortOrder.ToLongString(), sortOrderString, StringComparison.InvariantCultureIgnoreCase
                   );
        }

    }

    public static class SortHelper
    {

        public static SortOrder ToSortOrder(string str)
        {
            return SortOrder.Descending.Matches(str) ? SortOrder.Descending : SortOrder.Ascending;
        }

        public static IOrderedEnumerable<T> OrderBy<T, TKey>(
            IEnumerable<T> enumerable,
            Func<T, TKey> keySelector,
            bool ascending
        )
        {
            return ascending ? enumerable?.OrderBy(keySelector) : enumerable?.OrderByDescending(keySelector);
        }

        public static IEnumerable<T> OrderBy<T>(IEnumerable<T> enumerable, params SortPair[] sortPairs)
        {
            var orderedEnumerable = enumerable;
            sortPairs?.ToList()
                .ForEach(
                    sortPair => orderedEnumerable = OrderBy(
                        orderedEnumerable, entry => ((dynamic) entry)?[sortPair.Key], sortPair.Ascending
                    )
                );

            return orderedEnumerable;
        }

    }

}
