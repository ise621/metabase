using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentConcretizationOfConnection(
    Component subject
    )
        : Connection<Component, ComponentConcretizationAndGeneralization,
        ComponentGeneralizationsByComponentIdDataLoader, ComponentConcretizationOfEdge>(
        subject,
        x => new ComponentConcretizationOfEdge(x)
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
        return ComponentGeneralizationAuthorization.IsAuthorizedToAdd(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}