using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedDataFormatsByInstitutionIdDataLoader
    : AssociationsByAssociateIdDataLoader<DataFormat>
{
    public InstitutionManagedDataFormatsByInstitutionIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            (dbContext, ids) =>
                dbContext.DataFormats.AsNoTracking().Where(x =>
                    ids.Contains(x.ManagerId)
                ),
            x => x.ManagerId
        )
    {
    }
}