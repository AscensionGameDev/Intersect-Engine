using Intersect.Server.Web.RestApi.Types;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Extensions;


public static partial class EnumerableExtensions
{

    public static IEnumerable<TValue> Sort<TValue>(
        this IEnumerable<TValue> queryable,
        IReadOnlyCollection<Sort> sorts
    )
    {
        if (sorts == null || sorts.Count < 1)
        {
            return queryable;
        }

        var sorted = queryable;
        var orderedOnce = false;
        foreach (var (by, direction) in sorts.SelectMany(sort => sort.By.Select(by => (by, sort.Direction))))
        {
            if (string.IsNullOrWhiteSpace(by))
            {
                continue;
            }

            if (direction == SortDirection.Ascending)
            {
                sorted = orderedOnce
                    ? ((IOrderedEnumerable<TValue>)sorted).ThenBy(OrderLambda)
                    : sorted.OrderBy(OrderLambda);
            }
            else
            {
                sorted = orderedOnce
                    ? ((IOrderedEnumerable<TValue>)sorted).ThenByDescending(OrderLambda)
                    : sorted.OrderByDescending(OrderLambda);
            }

            orderedOnce = true;
            continue;

            object OrderLambda(TValue entity) => EF.Property<object>(entity, by);
        }

        return sorted;
    }

}
