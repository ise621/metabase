using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;

namespace Metabase.Authorization;

public static class OpenIdConnectAuthorization
{
    public static async Task<bool> IsAuthorizedToViewApplications(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || await CommonAuthorization.IsOwner(user, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToManageApplications(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || await CommonAuthorization.IsOwner(user, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToManageApplication(
        Guid applicationId,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || await CommonAuthorization.IsOwner(user, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToDeleteAuthorization(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsAdministrator(
                   user,
                   userManager
               );
    }

    public static async Task<bool> IsAuthorizedToRevokeToken(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsAdministrator(
                   user,
                   userManager
               );
    }
}