using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Methods;

public sealed class UserMethodDeveloperEdge
    : Edge<User, UserByIdDataLoader>
{
    private readonly UserMethodDeveloper _association;

    public UserMethodDeveloperEdge(
        UserMethodDeveloper association
    )
        : base(association.UserId)
    {
        _association = association;
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserConfirmEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager
    )
    {
        return UserMethodDeveloperAuthorization.IsAuthorizedToConfirm(
            claimsPrincipal,
            _association.UserId,
            userManager
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
        return UserMethodDeveloperAuthorization.IsAuthorizedToRemove(
            claimsPrincipal,
            _association.MethodId,
            userManager,
            context,
            cancellationToken
        );
    }
}