using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Authorization;

public static class CommonComponentAuthorization
{
    internal static async Task<bool> IsAtLeastAssistantOfOneVerifiedManufacturerOfComponent(
        User user,
        Guid componentId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var manufacturerIds =
            await context.Institutions.AsNoTracking()
                .Where(i => i.ManufacturedComponents.Any(c => c.Id == componentId))
                .Select(i => i.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        foreach (var manufacturerId in manufacturerIds)
            if (await CommonAuthorization.IsAtLeastAssistantOfVerifiedInstitution(
                    user,
                    manufacturerId,
                    context,
                    cancellationToken
                ).ConfigureAwait(false)
                &&
                await CommonAuthorization.IsVerifiedManufacturerOfComponent(
                    manufacturerId,
                    componentId,
                    context,
                    cancellationToken
                ).ConfigureAwait(false)
               )
                return true;

        return false;
    }
}