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

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

[ExtendObjectType(nameof(Mutation))]
public sealed class InstitutionMethodDeveloperMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddInstitutionMethodDeveloperPayload> AddInstitutionMethodDeveloperAsync(
        AddInstitutionMethodDeveloperInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionMethodDeveloperAuthorization.IsAuthorizedToAdd(
                claimsPrincipal,
                input.MethodId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddInstitutionMethodDeveloperPayload(
                new AddInstitutionMethodDeveloperError(
                    AddInstitutionMethodDeveloperErrorCode.UNAUTHORIZED,
                    $"You are not authorized to add method developer relation for method ${input.MethodId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<AddInstitutionMethodDeveloperError>();
        if (!await context.Methods.AsQueryable()
                .Where(u => u.Id == input.MethodId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddInstitutionMethodDeveloperError(
                    AddInstitutionMethodDeveloperErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    new[] { nameof(input), nameof(input.MethodId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddInstitutionMethodDeveloperError(
                    AddInstitutionMethodDeveloperErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddInstitutionMethodDeveloperPayload(errors.AsReadOnly());

        if (await context.InstitutionMethodDevelopers.AsQueryable()
                .Where(m =>
                    m.MethodId == input.MethodId
                    && m.InstitutionId == input.InstitutionId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddInstitutionMethodDeveloperPayload(
                new AddInstitutionMethodDeveloperError(
                    AddInstitutionMethodDeveloperErrorCode.DUPLICATE,
                    "Institution method developer already exists.",
                    new[] { nameof(input) }
                )
            );

        var institutionMethodDeveloper = new InstitutionMethodDeveloper
        {
            MethodId = input.MethodId,
            InstitutionId = input.InstitutionId,
            Pending = !await InstitutionMethodDeveloperAuthorization
                .IsAuthorizedToConfirm(claimsPrincipal, input.InstitutionId, userManager, context,
                    cancellationToken).ConfigureAwait(false)
        };
        context.InstitutionMethodDevelopers.Add(institutionMethodDeveloper);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddInstitutionMethodDeveloperPayload(institutionMethodDeveloper);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<ConfirmInstitutionMethodDeveloperPayload> ConfirmInstitutionMethodDeveloperAsync(
        ConfirmInstitutionMethodDeveloperInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionMethodDeveloperAuthorization.IsAuthorizedToConfirm(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new ConfirmInstitutionMethodDeveloperPayload(
                new ConfirmInstitutionMethodDeveloperError(
                    ConfirmInstitutionMethodDeveloperErrorCode.UNAUTHORIZED,
                    $"You are not authorized to confirm method developer relation for institution ${input.InstitutionId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<ConfirmInstitutionMethodDeveloperError>();
        if (!await context.Methods.AsQueryable()
                .Where(u => u.Id == input.MethodId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmInstitutionMethodDeveloperError(
                    ConfirmInstitutionMethodDeveloperErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    new[] { nameof(input), nameof(input.MethodId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmInstitutionMethodDeveloperError(
                    ConfirmInstitutionMethodDeveloperErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new ConfirmInstitutionMethodDeveloperPayload(errors.AsReadOnly());

        var institutionMethodDeveloper =
            await context.InstitutionMethodDevelopers.AsQueryable()
                .Where(r =>
                    r.MethodId == input.MethodId
                    && r.InstitutionId == input.InstitutionId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institutionMethodDeveloper is null)
            return new ConfirmInstitutionMethodDeveloperPayload(
                new ConfirmInstitutionMethodDeveloperError(
                    ConfirmInstitutionMethodDeveloperErrorCode.UNKNOWN_DEVELOPER,
                    "Unknown developer.",
                    new[] { nameof(input) }
                )
            );

        institutionMethodDeveloper.Pending = false;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new ConfirmInstitutionMethodDeveloperPayload(institutionMethodDeveloper);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveInstitutionMethodDeveloperPayload> RemoveInstitutionMethodDeveloperAsync(
        RemoveInstitutionMethodDeveloperInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionMethodDeveloperAuthorization.IsAuthorizedToRemove(
                claimsPrincipal,
                input.MethodId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveInstitutionMethodDeveloperPayload(
                new RemoveInstitutionMethodDeveloperError(
                    RemoveInstitutionMethodDeveloperErrorCode.UNAUTHORIZED,
                    $"You are not authorized to remove method developer relation for method ${input.MethodId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<RemoveInstitutionMethodDeveloperError>();
        if (!await context.Methods.AsQueryable()
                .Where(u => u.Id == input.MethodId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveInstitutionMethodDeveloperError(
                    RemoveInstitutionMethodDeveloperErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    new[] { nameof(input), nameof(input.MethodId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveInstitutionMethodDeveloperError(
                    RemoveInstitutionMethodDeveloperErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveInstitutionMethodDeveloperPayload(errors.AsReadOnly());

        var institutionMethodDeveloper =
            await context.InstitutionMethodDevelopers.AsQueryable()
                .Where(r =>
                    r.MethodId == input.MethodId
                    && r.InstitutionId == input.InstitutionId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institutionMethodDeveloper is null)
            return new RemoveInstitutionMethodDeveloperPayload(
                new RemoveInstitutionMethodDeveloperError(
                    RemoveInstitutionMethodDeveloperErrorCode.UNKNOWN_DEVELOPER,
                    "Unknown developer.",
                    new[] { nameof(input) }
                )
            );

        context.InstitutionMethodDevelopers.Remove(institutionMethodDeveloper);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveInstitutionMethodDeveloperPayload(institutionMethodDeveloper);
    }
}