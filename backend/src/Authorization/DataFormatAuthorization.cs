using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Authorization;

public static class DataFormatAuthorization
{
    public static async Task<bool> IsAuthorizedToCreateDataFormatForInstitution(
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

    public static async Task<bool> IsAuthorizedToUpdate(
        ClaimsPrincipal claimsPrincipal,
        Guid dataFormatId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null &&
               await IsAtLeastAssistantOfVerifiedDataFormatManager(
                   user,
                   dataFormatId,
                   context,
                   cancellationToken
               );
    }

    private static async Task<bool> IsAtLeastAssistantOfVerifiedDataFormatManager(
        User user,
        Guid dataFormatId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var wrappedManagerId =
            await context.DataFormats.AsNoTracking()
                .Where(x => x.Id == dataFormatId)
                .Select(x => new { x.ManagerId })
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (wrappedManagerId is null) return false;

        return await CommonAuthorization.IsAtLeastAssistantOfVerifiedInstitution(
            user, wrappedManagerId.ManagerId, context, cancellationToken
        );
    }
}