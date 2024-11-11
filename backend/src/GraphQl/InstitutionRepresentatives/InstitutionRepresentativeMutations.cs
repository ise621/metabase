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
using Metabase.Enumerations;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.InstitutionRepresentatives;

[ExtendObjectType(nameof(Mutation))]
public sealed class InstitutionRepresentativeMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddInstitutionRepresentativePayload> AddInstitutionRepresentativeAsync(
        AddInstitutionRepresentativeInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionRepresentativeAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddInstitutionRepresentativePayload(
                new AddInstitutionRepresentativeError(
                    AddInstitutionRepresentativeErrorCode.UNAUTHORIZED,
                    "You are not authorized to add institution representatives.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<AddInstitutionRepresentativeError>();
        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddInstitutionRepresentativeError(
                    AddInstitutionRepresentativeErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(u => u.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddInstitutionRepresentativeError(
                    AddInstitutionRepresentativeErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddInstitutionRepresentativePayload(errors.AsReadOnly());

        if (await context.InstitutionRepresentatives.AsQueryable()
                .Where(r =>
                    r.InstitutionId == input.InstitutionId
                    && r.UserId == input.UserId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddInstitutionRepresentativePayload(
                new AddInstitutionRepresentativeError(
                    AddInstitutionRepresentativeErrorCode.DUPLICATE,
                    "Institution representative already exists.",
                    new[] { nameof(input) }
                )
            );

        var institutionRepresentative = new InstitutionRepresentative
        {
            InstitutionId = input.InstitutionId,
            UserId = input.UserId,
            Role = input.Role,
            Pending = !await InstitutionRepresentativeAuthorization
                .IsAuthorizedToConfirm(claimsPrincipal, input.UserId, userManager).ConfigureAwait(false)
        };
        context.InstitutionRepresentatives.Add(institutionRepresentative);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddInstitutionRepresentativePayload(institutionRepresentative);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveInstitutionRepresentativePayload> RemoveInstitutionRepresentativeAsync(
        RemoveInstitutionRepresentativeInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionRepresentativeAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveInstitutionRepresentativePayload(
                new RemoveInstitutionRepresentativeError(
                    RemoveInstitutionRepresentativeErrorCode.UNAUTHORIZED,
                    "You are not authorized to remove institution representatives.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<RemoveInstitutionRepresentativeError>();
        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveInstitutionRepresentativeError(
                    RemoveInstitutionRepresentativeErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(u => u.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveInstitutionRepresentativeError(
                    RemoveInstitutionRepresentativeErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveInstitutionRepresentativePayload(errors.AsReadOnly());

        var institutionRepresentative =
            await context.InstitutionRepresentatives.AsQueryable()
                .Where(r =>
                    r.InstitutionId == input.InstitutionId
                    && r.UserId == input.UserId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institutionRepresentative is null)
            return new RemoveInstitutionRepresentativePayload(
                new RemoveInstitutionRepresentativeError(
                    RemoveInstitutionRepresentativeErrorCode.UNKNOWN_REPRESENTATIVE,
                    "Unknown representative.",
                    new[] { nameof(input) }
                )
            );

        if (institutionRepresentative.Role == InstitutionRepresentativeRole.OWNER
            && !await ExistsOtherInstitutionOwner(
                institutionRepresentative.InstitutionId,
                institutionRepresentative.UserId,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveInstitutionRepresentativePayload(
                new RemoveInstitutionRepresentativeError(
                    RemoveInstitutionRepresentativeErrorCode.LAST_OWNER,
                    "Cannot remove last owner.",
                    new[] { nameof(input) }
                )
            );

        context.InstitutionRepresentatives.Remove(institutionRepresentative);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveInstitutionRepresentativePayload(institutionRepresentative);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<ChangeInstitutionRepresentativeRolePayload> ChangeInstitutionRepresentativeRoleAsync(
        ChangeInstitutionRepresentativeRoleInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionRepresentativeAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new ChangeInstitutionRepresentativeRolePayload(
                new ChangeInstitutionRepresentativeRoleError(
                    ChangeInstitutionRepresentativeRoleErrorCode.UNAUTHORIZED,
                    "You are not authorized to change institution representative roles.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<ChangeInstitutionRepresentativeRoleError>();
        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ChangeInstitutionRepresentativeRoleError(
                    ChangeInstitutionRepresentativeRoleErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(u => u.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ChangeInstitutionRepresentativeRoleError(
                    ChangeInstitutionRepresentativeRoleErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new ChangeInstitutionRepresentativeRolePayload(errors.AsReadOnly());

        var institutionRepresentative =
            await context.InstitutionRepresentatives.AsQueryable()
                .Where(r =>
                    r.InstitutionId == input.InstitutionId
                    && r.UserId == input.UserId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institutionRepresentative is null)
            return new ChangeInstitutionRepresentativeRolePayload(
                new ChangeInstitutionRepresentativeRoleError(
                    ChangeInstitutionRepresentativeRoleErrorCode.UNKNOWN_REPRESENTATIVE,
                    "Unknown representative.",
                    new[] { nameof(input) }
                )
            );

        if (input.Role != InstitutionRepresentativeRole.OWNER
            && institutionRepresentative.Role == InstitutionRepresentativeRole.OWNER
            && !await ExistsOtherInstitutionOwner(
                institutionRepresentative.InstitutionId,
                institutionRepresentative.UserId,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new ChangeInstitutionRepresentativeRolePayload(
                new ChangeInstitutionRepresentativeRoleError(
                    ChangeInstitutionRepresentativeRoleErrorCode.LAST_OWNER,
                    "Cannot downgrade last owner.",
                    new[] { nameof(input) }
                )
            );

        institutionRepresentative.Role = input.Role;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new ChangeInstitutionRepresentativeRolePayload(institutionRepresentative);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<ConfirmInstitutionRepresentativePayload> ConfirmInstitutionRepresentativeAsync(
        ConfirmInstitutionRepresentativeInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionRepresentativeAuthorization.IsAuthorizedToConfirm(
                claimsPrincipal,
                input.UserId,
                userManager
            ).ConfigureAwait(false)
           )
            return new ConfirmInstitutionRepresentativePayload(
                new ConfirmInstitutionRepresentativeError(
                    ConfirmInstitutionRepresentativeErrorCode.UNAUTHORIZED,
                    $"You are not authorized to confirm representative relation for user ${input.UserId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<ConfirmInstitutionRepresentativeError>();
        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmInstitutionRepresentativeError(
                    ConfirmInstitutionRepresentativeErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(u => u.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmInstitutionRepresentativeError(
                    ConfirmInstitutionRepresentativeErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new ConfirmInstitutionRepresentativePayload(errors.AsReadOnly());

        var institutionRepresentative =
            await context.InstitutionRepresentatives.AsQueryable()
                .Where(r =>
                    r.InstitutionId == input.InstitutionId
                    && r.UserId == input.UserId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institutionRepresentative is null)
            return new ConfirmInstitutionRepresentativePayload(
                new ConfirmInstitutionRepresentativeError(
                    ConfirmInstitutionRepresentativeErrorCode.UNKNOWN_REPRESENTATIVE,
                    "Unknown representative.",
                    new[] { nameof(input) }
                )
            );

        institutionRepresentative.Pending = false;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new ConfirmInstitutionRepresentativePayload(institutionRepresentative);
    }

    private static async Task<bool> ExistsOtherInstitutionOwner(
        Guid institutionId,
        Guid userId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await context.InstitutionRepresentatives.AsQueryable()
            .Where(r =>
                r.InstitutionId == institutionId
                && r.UserId != userId
                && r.Role == InstitutionRepresentativeRole.OWNER
            )
            .AnyAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}