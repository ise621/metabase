using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Components;

public sealed class ComponentConcretizationsByComponentIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<ComponentConcretizationAndGeneralization>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.ComponentConcretizationAndGeneralizations.AsNoTracking().Where(x =>
                    ids.Contains(x.GeneralComponentId)
                ),
        x => x.GeneralComponentId
        )
{
}