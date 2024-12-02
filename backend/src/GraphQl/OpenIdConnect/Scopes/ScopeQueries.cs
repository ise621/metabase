using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Scopes;

[ExtendObjectType(nameof(Query))]
public sealed class ScopeQueries
{
    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreScope>> GetScopes(
        OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> manager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return Array.Empty<OpenIddictEntityFrameworkCoreScope>();

        return await manager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<OpenIddictEntityFrameworkCoreScope?> GetScope(
        Guid uuid,
        OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> manager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return null;

        return await manager.FindByIdAsync(uuid.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}