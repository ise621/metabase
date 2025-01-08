using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class ApplicationInstitutionsByApplicationIdDataLoader :
    AssociationsByAssociateIdDataLoader<InstitutionApplication>
{
    public ApplicationInstitutionsByApplicationIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            (dbContext, ids) =>
                dbContext.ApplicationInstitutions.AsNoTracking().Where(x =>
                    ids.Contains(x.ApplicationId)
                ),
            x => x.ApplicationId
        )
    {
    }
}