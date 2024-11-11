using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedMethodConnection
    : Connection<Institution, Method, InstitutionManagedMethodsByInstitutionIdDataLoader,
        InstitutionManagedMethodEdge>
{
    public InstitutionManagedMethodConnection(
        Institution institution
    )
        : base(
            institution,
            x => new InstitutionManagedMethodEdge(x)
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
        return MethodAuthorization.IsAuthorizedToCreateMethodManagedByInstitution(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}