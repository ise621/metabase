using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionDevelopedMethodConnection(
    Institution institution,
    bool pending
    )
        : ForkingConnection<Institution, InstitutionMethodDeveloper,
        PendingInstitutionDevelopedMethodsByInstitutionIdDataLoader,
        InstitutionDevelopedMethodsByInstitutionIdDataLoader, InstitutionDevelopedMethodEdge>(
        institution,
        pending,
        x => new InstitutionDevelopedMethodEdge(x)
        )
{
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
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}