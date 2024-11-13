using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Methods;

public sealed class UserMethodDevelopersByMethodIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<UserMethodDeveloper>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.UserMethodDevelopers.AsNoTracking().Where(x =>
                    !x.Pending && ids.Contains(x.MethodId)
                ),
        x => x.MethodId
        )
{
}