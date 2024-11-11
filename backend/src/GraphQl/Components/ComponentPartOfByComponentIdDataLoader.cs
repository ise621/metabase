using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Components;

public sealed class ComponentPartOfByComponentIdDataLoader
    : AssociationsByAssociateIdDataLoader<ComponentAssembly>
{
    public ComponentPartOfByComponentIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            (dbContext, ids) =>
                dbContext.ComponentAssemblies.AsNoTracking().Where(x =>
                    ids.Contains(x.PartComponentId)
                ),
            x => x.PartComponentId
        )
    {
    }
}