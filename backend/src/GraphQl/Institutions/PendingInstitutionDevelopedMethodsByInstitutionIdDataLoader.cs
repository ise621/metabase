using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions;

public sealed class PendingInstitutionDevelopedMethodsByInstitutionIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<InstitutionMethodDeveloper>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.InstitutionMethodDevelopers.AsNoTracking().Where(x =>
                    x.Pending && ids.Contains(x.InstitutionId)
                ),
        x => x.InstitutionId
        )
{
}