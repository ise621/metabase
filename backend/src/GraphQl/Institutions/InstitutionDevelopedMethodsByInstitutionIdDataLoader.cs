using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionDevelopedMethodsByInstitutionIdDataLoader
    : AssociationsByAssociateIdDataLoader<InstitutionMethodDeveloper>
{
    public InstitutionDevelopedMethodsByInstitutionIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            (dbContext, ids) =>
                dbContext.InstitutionMethodDevelopers.AsQueryable().Where(x =>
                    !x.Pending && ids.Contains(x.InstitutionId)
                ),
            x => x.InstitutionId
        )
    {
    }
}