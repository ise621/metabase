using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Authorization;

public static class DatabaseAuthorization
{
    public static async Task<bool> IsAuthorizedToCreateDatabaseForInstitution(
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
        Guid databaseId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null &&
               await IsAtLeastAssistantOfVerifiedDatabaseOperator(
                   user,
                   databaseId,
                   context,
                   cancellationToken
               );
    }

    public static async Task<bool> IsAuthorizedToVerify(
        ClaimsPrincipal claimsPrincipal,
        Guid databaseId,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null &&
               await IsAtLeastAssistantOfVerifiedDatabaseOperator(
                   user,
                   databaseId,
                   context,
                   cancellationToken
               );
    }

    private static async Task<bool> IsAtLeastAssistantOfVerifiedDatabaseOperator(
        User user,
        Guid databaseId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var wrappedOperatorId =
            await context.Databases.AsNoTracking()
                .Where(x => x.Id == databaseId)
                .Select(x => new { x.OperatorId })
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (wrappedOperatorId is null)
        {
            return false;
        }

        return await CommonAuthorization.IsAtLeastAssistantOfVerifiedInstitution(
            user, wrappedOperatorId.OperatorId, context, cancellationToken
        );
    }
}