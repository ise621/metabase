using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentVariantOfEdge
    : Edge<Component, ComponentByIdDataLoader>
{
    private readonly ComponentVariant _association;

    public ComponentVariantOfEdge(
        ComponentVariant association
    )
        : base(association.OfComponentId)
    {
        _association = association;
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserRemoveEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return ComponentAssemblyAuthorization.IsAuthorizedToManage(
            claimsPrincipal,
            _association.OfComponentId,
            _association.ToComponentId,
            userManager,
            context,
            cancellationToken
        );
    }
}