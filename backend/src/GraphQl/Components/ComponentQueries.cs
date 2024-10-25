using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;

namespace Metabase.GraphQl.Components;

[ExtendObjectType(nameof(Query))]
public sealed class ComponentQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public IQueryable<Component> GetComponents(
        ApplicationDbContext context
    )
    {
        return context.Components.AsNoTracking();
    }

    public Task<Component?> GetComponentAsync(
        Guid uuid,
        ComponentByIdDataLoader componentById,
        CancellationToken cancellationToken
    )
    {
        return componentById.LoadAsync(
            uuid,
            cancellationToken
        );
    }
}