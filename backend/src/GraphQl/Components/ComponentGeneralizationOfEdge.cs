using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Components;

public sealed class ComponentGeneralizationOfEdge
    : Edge<Component, ComponentByIdDataLoader>
{
    private readonly ComponentConcretizationAndGeneralization _association;

    public ComponentGeneralizationOfEdge(
        ComponentConcretizationAndGeneralization association
    )
        : base(association.ConcreteComponentId)
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