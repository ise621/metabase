using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Methods;

public sealed class InstitutionMethodDeveloperEdge(
    InstitutionMethodDeveloper association
    )
        : Edge<Institution, InstitutionByIdDataLoader>(association.InstitutionId)
{
    private readonly InstitutionMethodDeveloper _association = association;

    [UseUserManager]
    public Task<bool> CanCurrentUserConfirmEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return InstitutionMethodDeveloperAuthorization.IsAuthorizedToConfirm(
            claimsPrincipal,
            _association.InstitutionId,
            userManager,
            context,
            cancellationToken
        );
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserRemoveEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return InstitutionMethodDeveloperAuthorization.IsAuthorizedToRemove(
            claimsPrincipal,
            _association.MethodId,
            userManager,
            context,
            cancellationToken
        );
    }
}