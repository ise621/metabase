using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentManufacturerEdge(
    ComponentManufacturer association
    )
        : Edge<Institution, InstitutionByIdDataLoader>(association.InstitutionId)
{
    private readonly ComponentManufacturer _association = association;

    [UseUserManager]
    public Task<bool> CanCurrentUserConfirmEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return ComponentManufacturerAuthorization.IsAuthorizedToConfirm(
            claimsPrincipal,
            _association.InstitutionId,
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
        return ComponentManufacturerAuthorization.IsAuthorizedToRemove(
            claimsPrincipal,
            _association.InstitutionId,
            userManager,
            context,
            cancellationToken
        );
    }
}