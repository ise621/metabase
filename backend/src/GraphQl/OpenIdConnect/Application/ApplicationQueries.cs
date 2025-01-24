using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;

namespace Metabase.GraphQl.OpenIdConnect.Application;

[ExtendObjectType(nameof(Query))]
public sealed class ApplicationQueries
{
    // TODO In all queries, instead of returning nothing, report as authentication error to client.
    // TODO Make the application manager use the scoped database context.
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.ReadPolicy)]
    public async Task<IList<OpenIdApplication>> GetApplications(
        OpenIddictApplicationManager<OpenIdApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        Data.ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
        {
            return Array.Empty<OpenIdApplication>();
        }

        // TODO Is there a more efficient way to return an `AsyncEnumerable` or `AsyncEnumerator` or to turn such a thing into an `Enumerable` or `Enumerator`?
        return await applicationManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.ReadPolicy)]
    public async Task<OpenIdApplication?> GetApplication(
        Guid applicationId,
        OpenIddictApplicationManager<OpenIdApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        Data.ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
        {
            return null;
        }

        return await applicationManager.FindByIdAsync(applicationId.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}