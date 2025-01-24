using System.Linq;
using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.Extensions;

public static class SortingContextExtensions
{
    public static void StabilizeOrder<T>(this ISortingContext sorting) where T : IEntity
    {
        // this signals that the expression was not handled within the resolver
        // and the sorting middleware should take over.
        sorting.Handled(false);
        sorting.OnAfterSortingApplied<IQueryable<T>>(
            static (sortingApplied, query) =>
            {
                if (sortingApplied && query is IOrderedQueryable<T> ordered)
                {
                    return ordered.ThenBy(_ => _.Id);
                }
                return query.OrderBy(_ => _.Id);
            });
    }
}