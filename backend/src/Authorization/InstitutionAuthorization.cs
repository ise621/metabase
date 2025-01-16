using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;

namespace Metabase.Authorization;

public static class InstitutionAuthorization
{
    internal static async Task<bool> IsAuthorizedToUpdateInstitution(
        ClaimsPrincipal claimsPrincipal,
        Guid institutionId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsAtLeastAssistant(
                   user,
                   institutionId,
                   context,
                   cancellationToken
               );
    }

    internal static async Task<bool> IsAuthorizedToDeleteInstitution(
        ClaimsPrincipal claimsPrincipal,
        Guid institutionId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsOwnerOfInstitution(
                   user,
                   institutionId,
                   context,
                   cancellationToken
               );
    }

    internal static async Task<bool> IsAuthorizedToCreateInstitutionManagedByInstitution(
        ClaimsPrincipal claimsPrincipal,
        Guid institutionId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsAtLeastAssistantOfVerifiedInstitution(
                   user,
                   institutionId,
                   context,
                   cancellationToken
               );
    }

    internal static async Task<bool> IsAuthorizedToVerifyInstitution(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsVerifier(
                   user,
                   userManager
               );
    }

    internal static async Task<bool> IsAuthorizedToSwitchInstitutionOperatingState(
        ClaimsPrincipal claimsPrincipal,
        Guid institutionId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsOwnerOfInstitution(
                   user,
                   institutionId,
                   context,
                   cancellationToken
               );
    }
}