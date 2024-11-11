using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedInstitutionConnection
    : Connection<Institution, Institution, InstitutionManagedInstitutionsByInstitutionIdDataLoader,
        InstitutionManagedInstitutionEdge>
{
    public InstitutionManagedInstitutionConnection(
        Institution institution
    )
        : base(
            institution,
            x => new InstitutionManagedInstitutionEdge(x)
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
        return InstitutionAuthorization.IsAuthorizedToCreateInstitutionManagedByInstitution(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}