using System.Collections.Generic;
using System.Linq;

using Intersect.Server.Web.RestApi.Payloads;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Extensions
{

    public static class EnumerableExtensions
    {

        public static IEnumerable<TValue> Sort<TValue>(
            this IEnumerable<TValue> queryable,
            IReadOnlyCollection<Sort> sort
        )
        {
            if (sort == null || sort.Count < 1)
            {
                return queryable;
            }

            var sorted = queryable;
            var orderedOnce = false;
            foreach (var sortPair in sort)
            {
                if (string.IsNullOrWhiteSpace(sortPair.By))
                {
                    continue;
                }

                object OrderLambda(TValue entity)
                {
                    return EF.Property<object>(entity, sortPair.By);
                }

                if (sortPair.Direction == SortDirection.Ascending)
                {
                    sorted = orderedOnce
                        ? ((IOrderedEnumerable<TValue>) sorted).ThenBy(OrderLambda)
                        : sorted.OrderBy(OrderLambda);
                }
                else
                {
                    sorted = orderedOnce
                        ? ((IOrderedEnumerable<TValue>) sorted).ThenByDescending(OrderLambda)
                        : sorted.OrderByDescending(OrderLambda);
                }

                orderedOnce = true;
            }

            return sorted;
        }

    }

}
