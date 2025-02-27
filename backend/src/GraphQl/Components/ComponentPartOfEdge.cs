using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.Enumerations;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentPartOfEdge(
    ComponentAssembly association
    )
        : Edge<Component, ComponentByIdDataLoader>(association.AssembledComponentId)
{
    private readonly ComponentAssembly _association = association;

    public byte? Index => _association.Index;

    public PrimeSurface? PrimeSurface => _association.PrimeSurface;

    [UseUserManager]
    public Task<bool> CanCurrentUserUpdateEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return ComponentAssemblyAuthorization.IsAuthorizedToManage(
            claimsPrincipal,
            _association.AssembledComponentId,
            _association.PartComponentId,
            userManager,
            context,
            cancellationToken
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
        return ComponentAssemblyAuthorization.IsAuthorizedToManage(
            claimsPrincipal,
            _association.AssembledComponentId,
            _association.PartComponentId,
            userManager,
            context,
            cancellationToken
        );
    }
}