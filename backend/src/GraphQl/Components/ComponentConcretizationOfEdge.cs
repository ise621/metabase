using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentConcretizationOfEdge(
    ComponentConcretizationAndGeneralization association
    )
        : Edge<Component, ComponentByIdDataLoader>(association.GeneralComponentId)
{
    private readonly ComponentConcretizationAndGeneralization _association = association;

    [UseUserManager]
    public Task<bool> CanCurrentUserRemoveEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return ComponentGeneralizationAuthorization.IsAuthorizedToManage(
            claimsPrincipal,
            _association.GeneralComponentId,
            _association.ConcreteComponentId,
            userManager,
            context,
            cancellationToken
        );
    }
}