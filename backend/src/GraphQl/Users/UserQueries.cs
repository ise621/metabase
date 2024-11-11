using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;
using Metabase.GraphQl.Extensions;

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
    /* [UseProjection] // fails without an explicit error message in the logs */
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<User>();
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