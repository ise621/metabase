using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Components;

public sealed class ComponentVariantOfByComponentIdDataLoader
    : AssociationsByAssociateIdDataLoader<ComponentVariant>
{
    public ComponentVariantOfByComponentIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
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
}