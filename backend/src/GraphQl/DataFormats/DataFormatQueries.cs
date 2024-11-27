using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Metabase.Data;
using Metabase.GraphQl.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.DataFormats;

[ExtendObjectType(nameof(Query))]
public sealed class DataFormatQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public IQueryable<DataFormat> GetDataFormats(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<DataFormat>();
        return context.DataFormats.AsNoTracking();
    }

    public Task<DataFormat?> GetDataFormatAsync(
        Guid uuid,
        DataFormatByIdDataLoader dataFormatById,
        CancellationToken cancellationToken
    )
    {
        return dataFormatById.LoadAsync(
            uuid,
            cancellationToken
        );
    }
}