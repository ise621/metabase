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

namespace Metabase.GraphQl.UserMethodDevelopers;

[ExtendObjectType(nameof(Mutation))]
public sealed class UserMethodDeveloperMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddUserMethodDeveloperPayload> AddUserMethodDeveloperAsync(
        AddUserMethodDeveloperInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await UserMethodDeveloperAuthorization.IsAuthorizedToAdd(
                claimsPrincipal,
                input.MethodId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddUserMethodDeveloperPayload(
                new AddUserMethodDeveloperError(
                    AddUserMethodDeveloperErrorCode.UNAUTHORIZED,
                    $"You are not authorized to add method developer relation for method ${input.MethodId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<AddUserMethodDeveloperError>();
        if (!await context.Methods.AsQueryable()
                .Where(u => u.Id == input.MethodId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddUserMethodDeveloperError(
                    AddUserMethodDeveloperErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    new[] { nameof(input), nameof(input.MethodId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(i => i.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddUserMethodDeveloperError(
                    AddUserMethodDeveloperErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddUserMethodDeveloperPayload(errors.AsReadOnly());

        if (await context.UserMethodDevelopers.AsQueryable()
                .Where(m =>
                    m.MethodId == input.MethodId
                    && m.UserId == input.UserId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddUserMethodDeveloperPayload(
                new AddUserMethodDeveloperError(
                    AddUserMethodDeveloperErrorCode.DUPLICATE,
                    "User method developer already exists.",
                    new[] { nameof(input) }
                )
            );

        var userMethodDeveloper = new UserMethodDeveloper
        {
            MethodId = input.MethodId,
            UserId = input.UserId,
            Pending = !await UserMethodDeveloperAuthorization
                .IsAuthorizedToConfirm(claimsPrincipal, input.UserId, userManager).ConfigureAwait(false)
        };
        context.UserMethodDevelopers.Add(userMethodDeveloper);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddUserMethodDeveloperPayload(userMethodDeveloper);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<ConfirmUserMethodDeveloperPayload> ConfirmUserMethodDeveloperAsync(
        ConfirmUserMethodDeveloperInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await UserMethodDeveloperAuthorization.IsAuthorizedToConfirm(
                claimsPrincipal,
                input.UserId,
                userManager
            ).ConfigureAwait(false)
           )
            return new ConfirmUserMethodDeveloperPayload(
                new ConfirmUserMethodDeveloperError(
                    ConfirmUserMethodDeveloperErrorCode.UNAUTHORIZED,
                    $"You are not authorized to confirm method developer relation for user ${input.UserId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<ConfirmUserMethodDeveloperError>();
        if (!await context.Methods.AsQueryable()
                .Where(u => u.Id == input.MethodId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmUserMethodDeveloperError(
                    ConfirmUserMethodDeveloperErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    new[] { nameof(input), nameof(input.MethodId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(i => i.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new ConfirmUserMethodDeveloperError(
                    ConfirmUserMethodDeveloperErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new ConfirmUserMethodDeveloperPayload(errors.AsReadOnly());

        var userMethodDeveloper =
            await context.UserMethodDevelopers.AsQueryable()
                .Where(r =>
                    r.MethodId == input.MethodId
                    && r.UserId == input.UserId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (userMethodDeveloper is null)
            return new ConfirmUserMethodDeveloperPayload(
                new ConfirmUserMethodDeveloperError(
                    ConfirmUserMethodDeveloperErrorCode.UNKNOWN_DEVELOPER,
                    "Unknown developer.",
                    new[] { nameof(input) }
                )
            );

        userMethodDeveloper.Pending = false;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new ConfirmUserMethodDeveloperPayload(userMethodDeveloper);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveUserMethodDeveloperPayload> RemoveUserMethodDeveloperAsync(
        RemoveUserMethodDeveloperInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await UserMethodDeveloperAuthorization.IsAuthorizedToRemove(
                claimsPrincipal,
                input.MethodId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveUserMethodDeveloperPayload(
                new RemoveUserMethodDeveloperError(
                    RemoveUserMethodDeveloperErrorCode.UNAUTHORIZED,
                    $"You are not authorized to remove method developer relation for method ${input.MethodId}.",
                    Array.Empty<string>()
                )
            );

        var errors = new List<RemoveUserMethodDeveloperError>();
        if (!await context.Methods.AsQueryable()
                .Where(u => u.Id == input.MethodId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveUserMethodDeveloperError(
                    RemoveUserMethodDeveloperErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    new[] { nameof(input), nameof(input.MethodId).FirstCharToLower() }
                )
            );

        if (!await context.Users.AsQueryable()
                .Where(i => i.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveUserMethodDeveloperError(
                    RemoveUserMethodDeveloperErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveUserMethodDeveloperPayload(errors.AsReadOnly());

        var userMethodDeveloper =
            await context.UserMethodDevelopers.AsQueryable()
                .Where(r =>
                    r.MethodId == input.MethodId
                    && r.UserId == input.UserId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (userMethodDeveloper is null)
            return new RemoveUserMethodDeveloperPayload(
                new RemoveUserMethodDeveloperError(
                    RemoveUserMethodDeveloperErrorCode.UNKNOWN_DEVELOPER,
                    "Unknown developer.",
                    new[] { nameof(input) }
                )
            );

        context.UserMethodDevelopers.Remove(userMethodDeveloper);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveUserMethodDeveloperPayload(userMethodDeveloper);
    }
}