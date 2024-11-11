using System.Security.Claims;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Users;

public sealed class UserDevelopedMethodConnection
    : ForkingConnection<User, UserMethodDeveloper, PendingUserDevelopedMethodsByUserIdDataLoader,
        UserDevelopedMethodsByUserIdDataLoader, UserDevelopedMethodEdge>
{
    public UserDevelopedMethodConnection(
        User subject,
        bool pending
    )
        : base(
            subject,
            pending,
            x => new UserDevelopedMethodEdge(x)
        )
    {
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserConfirmEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager
    )
    {
        return UserMethodDeveloperAuthorization.IsAuthorizedToConfirm(
            claimsPrincipal,
            Subject.Id,
            userManager
        );
    }
}