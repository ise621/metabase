using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;

namespace Metabase.GraphQl.Users;

[ExtendObjectType(nameof(Query))]
public sealed class UserQueries
{
    [UseUserManager]
    public async Task<User?> GetCurrentUserAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager
    )
    {
        return await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
    }

    [UsePaging]
    /* TODO [UseProjection] // fails without an explicit error message in the logs */
    /* TODO [UseFiltering(typeof(UserFilterType))] // wait for https://github.com/ChilliCream/hotchocolate/issues/2672 and https://github.com/ChilliCream/hotchocolate/issues/2666 */
    [UseSorting]
    public IQueryable<User> GetUsers(
        ApplicationDbContext context
    )
    {
        return context.Users.AsNoTracking();
    }

    public Task<User?> GetUserAsync(
        Guid uuid,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken
    )
    {
        return userById.LoadAsync(
            uuid,
            cancellationToken
        );
    }
}