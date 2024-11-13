using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.OpenIdConnect;

[ExtendObjectType(nameof(Query))]
public sealed class OpendIdConnectQueries
{
    // TODO In all queries, instead of returning nothing, report as authentication error to client.
    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreApplication>> GetOpenIdConnectApplications(
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        // Data.ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToView(claimsPrincipal, userManager)
                .ConfigureAwait(false))
        {
            return Array.Empty<OpenIddictEntityFrameworkCoreApplication>();
        }

        // TODO Is there a more efficient way to return an `AsyncEnumerable` or `AsyncEnumerator` or to turn such a thing into an `Enumerable` or `Enumerator`?
        return await manager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreScope>> GetOpenIdConnectScopes(
        OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> manager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        // Data.ApplicationDbContext context // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToView(claimsPrincipal, userManager)
                .ConfigureAwait(false))
        {
            return Array.Empty<OpenIddictEntityFrameworkCoreScope>();
        }

        return await manager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreToken>> GetOpenIdConnectTokens(
        OpenIddictTokenManager<OpenIddictEntityFrameworkCoreToken> manager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        // [TokendService] Data.ApplicationDbContext context // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToView(claimsPrincipal, userManager)
                .ConfigureAwait(false))
        {
            return Array.Empty<OpenIddictEntityFrameworkCoreToken>();
        }

        return await manager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreAuthorization>> GetOpenIdConnectAuthorizations(
        OpenIddictAuthorizationManager<OpenIddictEntityFrameworkCoreAuthorization> manager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        // [AuthorizationdService] Data.ApplicationDbContext context // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToView(claimsPrincipal, userManager)
                .ConfigureAwait(false))
        {
            return Array.Empty<OpenIddictEntityFrameworkCoreAuthorization>();
        }

        return await manager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}