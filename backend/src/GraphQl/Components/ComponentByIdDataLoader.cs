using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Components;

public sealed class ComponentByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<Component>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.Components
        )
{
}