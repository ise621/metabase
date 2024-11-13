using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.DataFormats;

public sealed class DataFormatByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<DataFormat>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.DataFormats
        )
{
}