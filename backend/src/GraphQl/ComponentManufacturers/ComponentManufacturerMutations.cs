using System;
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

namespace Metabase.GraphQl.ComponentManufacturers;

[ExtendObjectType(nameof(Mutation))]
public sealed class ComponentManufacturerMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddComponentManufacturerPayload> AddComponentManufacturerAsync(
        AddComponentManufacturerInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentManufacturerAuthorization.IsAuthorizedToAdd(
                claimsPrincipal,
                input.ComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddComponentManufacturerPayload(
                new AddComponentManufacturerError(
                    AddComponentManufacturerErrorCode.UNAUTHORIZED,
                    "You are not authorized to add the component manufacturer.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<AddComponentManufacturerError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.ComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentManufacturerError(
                    AddComponentManufacturerErrorCode.UNKNOWN_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.ComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .Where(c => c.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentManufacturerError(
                    AddComponentManufacturerErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddComponentManufacturerPayload(errors.AsReadOnly());

        if (await context.ComponentManufacturers.AsQueryable()
                .Where(m =>
                    m.ComponentId == input.ComponentId
                    && m.InstitutionId == input.InstitutionId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddComponentManufacturerPayload(
                new AddComponentManufacturerError(
                    AddComponentManufacturerErrorCode.DUPLICATE,
                    "Component manufacturer already exists.",
                    new[] { nameof(input) }
                )
            );

        var componentManufacturer = new ComponentManufacturer
        {
            ComponentId = input.ComponentId,
            InstitutionId = input.InstitutionId,
            Pending = !await ComponentManufacturerAuthorization
                .IsAuthorizedToConfirm(claimsPrincipal, input.InstitutionId, userManager, context,
                    cancellationToken).ConfigureAwait(false)
        };
        context.ComponentManufacturers.Add(componentManufacturer);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddComponentManufacturerPayload(componentManufacturer);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<ConfirmComponentManufacturerPayload> ConfirmComponentManufacturerAsync(
        ConfirmComponentManufacturerInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentManufacturerAuthorization.IsAuthorizedToConfirm(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new ConfirmComponentManufacturerPayload(
                new ConfirmComponentManufacturerError(
                    ConfirmComponentManufacturerErrorCode.UNAUTHORIZED,
                    $"You are not authorized to confirm component manufacturer relation for institution ${input.InstitutionId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<ConfirmComponentManufacturerError>();
        if (!await context.Components.AsQueryable()
                .Where(u => u.Id == input.ComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmComponentManufacturerError(
                    ConfirmComponentManufacturerErrorCode.UNKNOWN_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.ComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmComponentManufacturerError(
                    ConfirmComponentManufacturerErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new ConfirmComponentManufacturerPayload(errors.AsReadOnly());

        var componentManufacturer =
            await context.ComponentManufacturers.AsQueryable()
                .Where(r =>
                    r.ComponentId == input.ComponentId
                    && r.InstitutionId == input.InstitutionId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (componentManufacturer is null)
            return new ConfirmComponentManufacturerPayload(
                new ConfirmComponentManufacturerError(
                    ConfirmComponentManufacturerErrorCode.UNKNOWN_MANUFACTURER,
                    "Unknown manufacturer.",
                    new[] { nameof(input) }
                )
            );

        componentManufacturer.Pending = false;
        var x = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new ConfirmComponentManufacturerPayload(componentManufacturer);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveComponentManufacturerPayload> RemoveComponentManufacturerAsync(
        RemoveComponentManufacturerInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentManufacturerAuthorization.IsAuthorizedToRemove(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveComponentManufacturerPayload(
                new RemoveComponentManufacturerError(
                    RemoveComponentManufacturerErrorCode.UNAUTHORIZED,
                    "You are not authorized to remove the component manufacturer.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<RemoveComponentManufacturerError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.ComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentManufacturerError(
                    RemoveComponentManufacturerErrorCode.UNKNOWN_COMPONENT,
                    "Unknown component.",
                    new[] { nameof(input), nameof(input.ComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .Where(c => c.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentManufacturerError(
                    RemoveComponentManufacturerErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveComponentManufacturerPayload(errors.AsReadOnly());

        var componentManufacturer =
            await context.ComponentManufacturers.AsQueryable()
                .Where(a =>
                    a.ComponentId == input.ComponentId
                    && a.InstitutionId == input.InstitutionId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (componentManufacturer is null)
            return new RemoveComponentManufacturerPayload(
                new RemoveComponentManufacturerError(
                    RemoveComponentManufacturerErrorCode.UNKNOWN_MANUFACTURER,
                    "Unknown manufacturer.",
                    new[] { nameof(input) }
                )
            );

        if (!await context.ComponentManufacturers.AsQueryable()
                .Where(a =>
                    a.ComponentId == input.ComponentId
                    && a.InstitutionId != input.InstitutionId
                    && !a.Pending
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false))
            return new RemoveComponentManufacturerPayload(
                new RemoveComponentManufacturerError(
                    RemoveComponentManufacturerErrorCode.LAST_MANUFACTURER,
                    "Cannot remove last manufacturer.",
                    new[] { nameof(input) }
                )
            );

        context.ComponentManufacturers.Remove(componentManufacturer);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveComponentManufacturerPayload(componentManufacturer);
    }
}