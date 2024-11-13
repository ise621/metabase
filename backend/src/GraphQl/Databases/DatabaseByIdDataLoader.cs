using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Databases;

public sealed class DatabaseByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<Database>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.Databases
        )
{
}