using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentManufacturerConnection
    : ForkingConnection<Component, ComponentManufacturer,
        PendingComponentManufacturersByComponentIdDataLoader, ComponentManufacturersByComponentIdDataLoader,
        ComponentManufacturerEdge>
{
    public ComponentManufacturerConnection(
        Component subject,
        bool pending
    )
        : base(
            subject,
            pending,
            x => new ComponentManufacturerEdge(x)
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
        return ComponentManufacturerAuthorization.IsAuthorizedToAdd(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}