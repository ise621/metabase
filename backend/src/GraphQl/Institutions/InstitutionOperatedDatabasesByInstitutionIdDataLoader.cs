using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionOperatedDatabasesByInstitutionIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<Database>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.Databases.AsNoTracking().Where(x =>
                    ids.Contains(x.OperatorId)
                ),
        x => x.OperatorId
        )
{
}