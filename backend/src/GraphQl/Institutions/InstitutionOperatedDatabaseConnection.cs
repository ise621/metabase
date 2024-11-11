using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionOperatedDatabaseConnection
    : Connection<Institution, Database, InstitutionOperatedDatabasesByInstitutionIdDataLoader,
        InstitutionOperatedDatabaseEdge>
{
    public InstitutionOperatedDatabaseConnection(
        Institution institution
    )
        : base(
            institution,
            x => new InstitutionOperatedDatabaseEdge(x)
        )
    {
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserAddEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return DatabaseAuthorization.IsAuthorizedToCreateDatabaseForInstitution(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}