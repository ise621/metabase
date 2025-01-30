using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;

namespace Metabase.Authorization;

public class ApprovalAuthorization
{
    public static async Task<bool> IsAuthorizedToAddApprovals(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && user.DataSigningPermission == Enumerations.DataSigningPermission.GRANTED;
    }
}