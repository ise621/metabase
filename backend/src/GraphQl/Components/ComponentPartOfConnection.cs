using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentPartOfConnection(
    Component subject
    )
        : Connection<Component, ComponentAssembly, ComponentPartOfByComponentIdDataLoader,
        ComponentPartOfEdge>(
        subject,
        x => new ComponentPartOfEdge(x)
        )
{
    [UseUserManager]
    public Task<bool> CanCurrentUserAddEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return ComponentAssemblyAuthorization.IsAuthorizedToAdd(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}