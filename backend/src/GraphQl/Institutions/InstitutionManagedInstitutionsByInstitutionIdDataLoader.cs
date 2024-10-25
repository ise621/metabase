using System;
using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedInstitutionsByInstitutionIdDataLoader
    : AssociationsByAssociateIdDataLoader<Institution>
{
    public InstitutionManagedInstitutionsByInstitutionIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            (dbContext, ids) =>
                dbContext.Institutions.AsNoTracking().Where(x =>
                    ids.Contains(x.ManagerId ?? Guid.Empty)
                ),
            x => x.ManagerId ?? Guid.Empty
        )
    {
    }
}