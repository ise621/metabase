using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Metabase.Data;
using Metabase.Enumerations;
using Metabase.GraphQl.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Databases;

[ExtendObjectType(nameof(Query))]
public sealed class DatabaseQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public IQueryable<Database> GetDatabases(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<Database>();
        return
            context.Databases.AsNoTracking()
                .Where(d => d.VerificationState == DatabaseVerificationState.VERIFIED);
    }

    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public IQueryable<Database> GetPendingDatabases(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<Database>();
        return
            context.Databases.AsNoTracking()
                .Where(d => d.VerificationState == DatabaseVerificationState.PENDING);
    }

    public Task<Database?> GetDatabaseAsync(
        Guid uuid,
        DatabaseByIdDataLoader databaseById,
        CancellationToken cancellationToken
    )
    {
        return databaseById.LoadAsync(
            uuid,
            cancellationToken
        );
    }
}