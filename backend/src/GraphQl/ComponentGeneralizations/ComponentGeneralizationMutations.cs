using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.ComponentGeneralizations;

[ExtendObjectType(nameof(Mutation))]
public sealed class ComponentGeneralizationMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddComponentGeneralizationPayload> AddComponentGeneralizationAsync(
        AddComponentGeneralizationInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentGeneralizationAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.ConcreteComponentId,
                input.GeneralComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddComponentGeneralizationPayload(
                new AddComponentGeneralizationError(
                    AddComponentGeneralizationErrorCode.UNAUTHORIZED,
                    "You are not authorized to add the component generalization.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<AddComponentGeneralizationError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.GeneralComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentGeneralizationError(
                    AddComponentGeneralizationErrorCode.UNKNOWN_GENERAL_COMPONENT,
                    "Unknown general component.",
                    new[] { nameof(input), nameof(input.GeneralComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.ConcreteComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentGeneralizationError(
                    AddComponentGeneralizationErrorCode.UNKNOWN_CONCRETE_COMPONENT,
                    "Unknown concrete component.",
                    new[] { nameof(input), nameof(input.ConcreteComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddComponentGeneralizationPayload(errors.AsReadOnly());

        if (await context.ComponentConcretizationAndGeneralizations.AsQueryable()
                .Where(a =>
                    a.GeneralComponentId == input.GeneralComponentId
                    && a.ConcreteComponentId == input.ConcreteComponentId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddComponentGeneralizationPayload(
                new AddComponentGeneralizationError(
                    AddComponentGeneralizationErrorCode.DUPLICATE,
                    "Component generalization already exists.",
                    new[] { nameof(input) }
                )
            );

        var componentGeneralization = new ComponentConcretizationAndGeneralization
        {
            GeneralComponentId = input.GeneralComponentId,
            ConcreteComponentId = input.ConcreteComponentId
        };
        context.ComponentConcretizationAndGeneralizations.Add(componentGeneralization);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddComponentGeneralizationPayload(componentGeneralization);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveComponentGeneralizationPayload> RemoveComponentGeneralizationAsync(
        RemoveComponentGeneralizationInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentGeneralizationAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.ConcreteComponentId,
                input.GeneralComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveComponentGeneralizationPayload(
                new RemoveComponentGeneralizationError(
                    RemoveComponentGeneralizationErrorCode.UNAUTHORIZED,
                    "You are not authorized to remove the component generalization.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<RemoveComponentGeneralizationError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.GeneralComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentGeneralizationError(
                    RemoveComponentGeneralizationErrorCode.UNKNOWN_GENERAL_COMPONENT,
                    "Unknown general component.",
                    new[] { nameof(input), nameof(input.GeneralComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.ConcreteComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentGeneralizationError(
                    RemoveComponentGeneralizationErrorCode.UNKNOWN_CONCRETE_COMPONENT,
                    "Unknown concrete component.",
                    new[] { nameof(input), nameof(input.ConcreteComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveComponentGeneralizationPayload(errors.AsReadOnly());

        var componentGeneralization =
            await context.ComponentConcretizationAndGeneralizations.AsQueryable()
                .Where(a =>
                    a.GeneralComponentId == input.GeneralComponentId
                    && a.ConcreteComponentId == input.ConcreteComponentId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (componentGeneralization is null)
            return new RemoveComponentGeneralizationPayload(
                new RemoveComponentGeneralizationError(
                    RemoveComponentGeneralizationErrorCode.UNKNOWN_GENERALIZATION,
                    "Unknown generalization.",
                    new[] { nameof(input) }
                )
            );

        context.ComponentConcretizationAndGeneralizations.Remove(componentGeneralization);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveComponentGeneralizationPayload(componentGeneralization);
    }
}