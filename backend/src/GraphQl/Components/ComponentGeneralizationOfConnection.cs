using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentGeneralizationOfConnection
    : Connection<Component, ComponentConcretizationAndGeneralization,
        ComponentConcretizationsByComponentIdDataLoader, ComponentGeneralizationOfEdge>
{
    public ComponentGeneralizationOfConnection(
        Component subject
    )
        : base(
            subject,
            x => new ComponentGeneralizationOfEdge(x)
        )
    {
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserAddEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        [Service] UserManager<User> userManager,
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