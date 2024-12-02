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

namespace Metabase.GraphQl.OpenIdConnect.Tokens;

[ExtendObjectType(nameof(Query))]
public sealed class TokenQueries
{
    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreToken>> GetTokens(
        OpenIddictTokenManager<OpenIddictEntityFrameworkCoreToken> tokenManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return Array.Empty<OpenIddictEntityFrameworkCoreToken>();

        return await tokenManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<OpenIddictEntityFrameworkCoreToken?> GetToken(
        Guid uuid,
        OpenIddictTokenManager<OpenIddictEntityFrameworkCoreToken> tokenManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return null;

        return await tokenManager.FindByIdAsync(uuid.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreToken>> GetTokensByApplicationId(
        Guid applicationId,
        OpenIddictTokenManager<OpenIddictEntityFrameworkCoreToken> tokenManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return Array.Empty<OpenIddictEntityFrameworkCoreToken>();

        return await tokenManager.FindByApplicationIdAsync(applicationId.ToString(), cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}