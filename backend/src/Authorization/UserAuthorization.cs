using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;
using UserRole = Metabase.Enumerations.UserRole;

namespace Metabase.Authorization;

public static class UserAuthorization
{
    public static async Task<bool> IsAuthorizedToDeleteUsers(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsAdministrator(user, userManager);
    }

    public static async Task<bool> IsAuthorizedToManageUser(
        ClaimsPrincipal claimsPrincipal,
        Guid userId,
        UserManager<User> userManager
    )
    {
        var loggedInUser = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (loggedInUser is null)
        {
            return false;
        }

        if (loggedInUser.Id == userId)
        {
            return true;
        }

        if (await userManager.IsInRoleAsync(
                loggedInUser,
                Role.EnumToName(UserRole.ADMINISTRATOR)
            ).ConfigureAwait(false))
        {
            return true;
        }

        return false;
    }

    public static async Task<bool> IsAuthorizedToAddOrRemoveRole(
        ClaimsPrincipal claimsPrincipal,
        UserRole role,
        UserManager<User> userManager
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
        {
            return false;
        }

        if (await CommonAuthorization.IsAdministrator(user, userManager).ConfigureAwait(false))
        {
            return true;
        }

        return role switch
        {
            UserRole.ADMINISTRATOR =>
                await CommonAuthorization.IsAdministrator(user, userManager).ConfigureAwait(false),
            UserRole.VERIFIER =>
                await CommonAuthorization.IsVerifier(user, userManager).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(role), $"Unknown role `{role}.`")
        };
    }

    public static async Task<bool> IsAuthorizedToManageSigningPermission(
        Guid institutionId,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);

        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || await CommonAuthorization.IsOwnerOfInstitution(user, institutionId, context, cancellationToken));
    }
}