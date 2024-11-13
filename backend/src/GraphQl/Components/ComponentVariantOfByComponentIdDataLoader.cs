using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Components;

public sealed class ComponentVariantOfByComponentIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<ComponentVariant>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.ComponentVariants.AsNoTracking().Where(x =>
                    ids.Contains(x.ToComponentId)
                ),
        x => x.ToComponentId
        )
{
}