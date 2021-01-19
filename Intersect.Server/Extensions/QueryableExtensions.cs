using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Intersect.Server.Web.RestApi.Payloads;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Extensions
{

    public static class QueryableExtensions
    {

        public static IQueryable<TValue> Sort<TValue>(
            this IQueryable<TValue> queryable,
            IReadOnlyCollection<Sort> sort
        )
        {
            return DoSort(queryable, sort);
        }

        public static bool IsOrdered<TValue>(this IQueryable<TValue> queryable)
        {
            return queryable.Expression.Type == typeof(IOrderedQueryable<TValue>);
        }

        public static IOrderedQueryable<TValue> SmartOrderBy<TValue>(
            this IQueryable<TValue> queryable,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Ascending
                ? queryable.OrderBy(entity => EF.Property<object>(entity, sort.By))
                : queryable.OrderByDescending(entity => EF.Property<object>(entity, sort.By));
        }

        public static IOrderedQueryable<TValue> SmartThenBy<TValue>(
            this IOrderedQueryable<TValue> queryable,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Ascending
                ? queryable.ThenBy(entity => EF.Property<object>(entity, sort.By))
                : queryable.ThenByDescending(entity => EF.Property<object>(entity, sort.By));
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
            return queryable.IsOrdered()
                ? (queryable as IOrderedQueryable<TValue>)?.ThenBy(entity => EF.Property<object>(entity, sort.By))
                : queryable.OrderBy(entity => EF.Property<object>(entity, sort.By));
        }

        public static IQueryable<TValue> SmartSortDescending<TValue>(
            this IQueryable<TValue> queryable,
            Sort sort
        )
        {
            return queryable.IsOrdered()
                ? (queryable as IOrderedQueryable<TValue>)?.ThenByDescending(
                    entity => EF.Property<object>(entity, sort.By)
                )
                : queryable.OrderByDescending(entity => EF.Property<object>(entity, sort.By));
        }

        public static IQueryable<TValue> DoSort<TValue>(
            IQueryable<TValue> queryable,
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

                Expression<Func<TValue, object>> orderLambda = entity => EF.Property<object>(entity, sortPair.By);

                if (sortPair.Direction == SortDirection.Ascending)
                {
                    sorted = orderedOnce
                        ? ((IOrderedQueryable<TValue>) sorted).ThenBy(orderLambda)
                        : sorted.OrderBy(orderLambda);
                }
                else
                {
                    sorted = orderedOnce
                        ? ((IOrderedQueryable<TValue>) sorted).ThenByDescending(orderLambda)
                        : sorted.OrderByDescending(orderLambda);
                }

                orderedOnce = true;
            }

            return sorted;
        }

    }

}
