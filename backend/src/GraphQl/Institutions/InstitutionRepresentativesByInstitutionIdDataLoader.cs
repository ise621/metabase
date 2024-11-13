using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionRepresentativesByInstitutionIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<InstitutionRepresentative>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.InstitutionRepresentatives.AsNoTracking().Where(x =>
                    !x.Pending && ids.Contains(x.InstitutionId)
                ),
        x => x.InstitutionId
        )
{
}