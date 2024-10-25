using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Authorization;

public static class CommonMethodAuthorization
{
    internal static async Task<bool> IsAtLeastAssistantOfVerifiedMethodManager(
        User user,
        Guid methodId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var wrappedManagerId =
            await context.Methods.AsNoTracking()
                .Where(x => x.Id == methodId)
                .Select(x => new { x.ManagerId })
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (wrappedManagerId is null) return false;

        return await CommonAuthorization.IsAtLeastAssistantOfVerifiedInstitution(
            user, wrappedManagerId.ManagerId, context, cancellationToken
        );
    }
}