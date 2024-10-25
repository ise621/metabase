using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Components;

public sealed class PendingComponentManufacturersByComponentIdDataLoader
    : AssociationsByAssociateIdDataLoader<ComponentManufacturer>
{
    public PendingComponentManufacturersByComponentIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            (dbContext, ids) =>
                dbContext.ComponentManufacturers.AsNoTracking().Where(x =>
                    x.Pending && ids.Contains(x.ComponentId)
                ),
            x => x.ComponentId
        )
    {
    }
}