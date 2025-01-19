using Intersect.Server.Collections.Sorting;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Extensions;


public static partial class QueryableExtensions
{

    public static bool IsOrdered<TValue>(this IQueryable<TValue> queryable)
    {
        return queryable.Expression.Type == typeof(IOrderedQueryable<TValue>);
    }

    public static IQueryable<TValue> SmartSort<TValue>(this IQueryable<TValue> queryable, Sort sort)
    {
        return sort.Direction == SortDirection.Ascending
            ? SmartSortAscending(queryable, sort)
            : SmartSortDescending(queryable, sort);
    }

    public static IQueryable<TValue> SmartSortAscending<TValue>(
        this IQueryable<TValue> queryable,
        Sort sort
    )
    {
        return queryable is IOrderedQueryable<TValue> orderedQueryable
            ? orderedQueryable.ThenBy(entity => EF.Property<object>(entity, sort.By.First()))
            : queryable.OrderBy(entity => EF.Property<object>(entity, sort.By.First()));
    }

    public static IQueryable<TValue> SmartSortDescending<TValue>(
        this IQueryable<TValue> queryable,
        Sort sort
    )
    {

        return queryable is IOrderedQueryable<TValue> orderedQueryable
            ? orderedQueryable.ThenByDescending(entity => EF.Property<object>(entity, sort.By.First()))
            : queryable.OrderByDescending(entity => EF.Property<object>(entity, sort.By.First()));
    }

}
