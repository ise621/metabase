using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;
using Metabase.GraphQl.Extensions;

namespace Metabase.GraphQl.Methods;

[ExtendObjectType(nameof(Query))]
public sealed class MethodQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public IQueryable<Method> GetMethods(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<Method>();
        return context.Methods.AsNoTracking();
    }

    public Task<Method?> GetMethodAsync(
        Guid uuid,
        MethodByIdDataLoader methodById,
        CancellationToken cancellationToken
    )
    {
        return methodById.LoadAsync(
            uuid,
            cancellationToken
        );
    }
}