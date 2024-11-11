using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentVariantOfConnection
    : Connection<Component, ComponentVariant, ComponentVariantOfByComponentIdDataLoader,
        ComponentVariantOfEdge>
{
    public ComponentVariantOfConnection(
        Component subject
    )
        : base(
            subject,
            x => new ComponentVariantOfEdge(x)
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
        return ComponentVariantAuthorization.IsAuthorizedToAdd(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}