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

namespace Metabase.GraphQl.OpenIdConnect.Authorizations;

[ExtendObjectType(nameof(Query))]
public sealed class AuthorizationQueries
{
    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreAuthorization>> GetAuthorizations(
        OpenIddictAuthorizationManager<OpenIddictEntityFrameworkCoreAuthorization> authorizationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return Array.Empty<OpenIddictEntityFrameworkCoreAuthorization>();

        var t = await authorizationManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return t;
    }

    [UseUserManager]
    public async Task<OpenIddictEntityFrameworkCoreAuthorization?> GetAuthorization(
        Guid uuid,
        OpenIddictAuthorizationManager<OpenIddictEntityFrameworkCoreAuthorization> authorizationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return null;

        return await authorizationManager.FindByIdAsync(uuid.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreAuthorization>> GetAuthorizationsByApplicationId(
        Guid applicationId,
        OpenIddictAuthorizationManager<OpenIddictEntityFrameworkCoreAuthorization> authorizationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
            return Array.Empty<OpenIddictEntityFrameworkCoreAuthorization>();

        var t = await authorizationManager.FindByApplicationIdAsync(applicationId.ToString(), cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return t;
    }
}