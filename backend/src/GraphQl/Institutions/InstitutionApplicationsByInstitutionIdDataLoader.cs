using System.Linq;
using GreenDonut;
using Metabase.Data;
using Metabase.GraphQl.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Institutions
{
    public sealed class InstitutionApplicationsByInstitutionIdDataLoader :
        AssociationsByAssociateIdDataLoader<InstitutionApplication>
    {
        public InstitutionApplicationsByInstitutionIdDataLoader(
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
                        ids.Contains(x.InstitutionId)
                    ),
                x => x.InstitutionId
            )
        {
        }
    }
}