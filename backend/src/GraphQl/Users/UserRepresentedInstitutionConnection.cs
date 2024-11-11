using System.Security.Claims;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Users;

public sealed class UserRepresentedInstitutionConnection
    : ForkingConnection<User, InstitutionRepresentative,
        PendingUserRepresentedInstitutionsByUserIdDataLoader, UserRepresentedInstitutionsByUserIdDataLoader,
        UserRepresentedInstitutionEdge>
{
    public UserRepresentedInstitutionConnection(
        User subject,
        bool pending
    )
        : base(
            subject,
            pending,
            x => new UserRepresentedInstitutionEdge(x)
        )
    {
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserConfirmEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager
    )
    {
        return InstitutionRepresentativeAuthorization.IsAuthorizedToConfirm(
            claimsPrincipal,
            Subject.Id,
            userManager
        );
    }
}