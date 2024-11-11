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

namespace Metabase.GraphQl.ComponentVariants;

[ExtendObjectType(nameof(Mutation))]
public sealed class ComponentVariantMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddComponentVariantPayload> AddComponentVariantAsync(
        AddComponentVariantInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentVariantAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.OtherComponentId,
                input.OneComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddComponentVariantPayload(
                new AddComponentVariantError(
                    AddComponentVariantErrorCode.UNAUTHORIZED,
                    "You are not authorized to add the component variant.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<AddComponentVariantError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.OneComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentVariantError(
                    AddComponentVariantErrorCode.UNKNOWN_ONE_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.OneComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.OtherComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentVariantError(
                    AddComponentVariantErrorCode.UNKNOWN_OTHER_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.OtherComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddComponentVariantPayload(errors.AsReadOnly());

        if (await context.ComponentVariants.AsQueryable()
                .Where(a =>
                    a.OfComponentId == input.OneComponentId
                    && a.ToComponentId == input.OtherComponentId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddComponentVariantPayload(
                new AddComponentVariantError(
                    AddComponentVariantErrorCode.DUPLICATE,
                    "Component variant already exists.",
                    new[] { nameof(input) }
                )
            );

        var componentVariant = new ComponentVariant
        {
            OfComponentId = input.OneComponentId,
            ToComponentId = input.OtherComponentId
        };
        var reverseComponentVariant = new ComponentVariant
        {
            OfComponentId = input.OtherComponentId,
            ToComponentId = input.OneComponentId
        };
        context.ComponentVariants.Add(componentVariant);
        context.ComponentVariants.Add(reverseComponentVariant);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddComponentVariantPayload(componentVariant, reverseComponentVariant);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveComponentVariantPayload> RemoveComponentVariantAsync(
        RemoveComponentVariantInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentVariantAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.OtherComponentId,
                input.OneComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveComponentVariantPayload(
                new RemoveComponentVariantError(
                    RemoveComponentVariantErrorCode.UNAUTHORIZED,
                    "You are not authorized to remove the component variant.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<RemoveComponentVariantError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.OneComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentVariantError(
                    RemoveComponentVariantErrorCode.UNKNOWN_ONE_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.OneComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.OtherComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentVariantError(
                    RemoveComponentVariantErrorCode.UNKNOWN_OTHER_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.OtherComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveComponentVariantPayload(errors.AsReadOnly());

        var componentVariant =
            await context.ComponentVariants.AsQueryable()
                .Where(a =>
                    a.OfComponentId == input.OneComponentId
                    && a.ToComponentId == input.OtherComponentId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        var reverseComponentVariant =
            await context.ComponentVariants.AsQueryable()
                .Where(a =>
                    a.OfComponentId == input.OtherComponentId
                    && a.ToComponentId == input.OneComponentId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        // Note that if the database is consistent, due to the reflivity of
        // the variant association, either both variants exist or none.
        if (componentVariant is null && reverseComponentVariant is null)
            return new RemoveComponentVariantPayload(
                new RemoveComponentVariantError(
                    RemoveComponentVariantErrorCode.UNKNOWN_VARIANT,
                    "Unknown variant.",
                    new[] { nameof(input) }
                )
            );

        if (componentVariant is not null) context.ComponentVariants.Remove(componentVariant);

        if (reverseComponentVariant is not null) context.ComponentVariants.Remove(reverseComponentVariant);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveComponentVariantPayload(input.OneComponentId, input.OtherComponentId);
    }
}