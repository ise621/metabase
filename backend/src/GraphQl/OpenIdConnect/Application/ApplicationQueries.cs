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

namespace Metabase.GraphQl.OpenIdConnect.Application;

[ExtendObjectType(nameof(Query))]
public sealed class ApplicationQueries
{
    // TODO In all queries, instead of returning nothing, report as authentication error to client.
    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreApplication>> GetApplications(
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        Data.ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
        {
            return Array.Empty<OpenIddictEntityFrameworkCoreApplication>();
        }

        //var applications = await applicationManager.ListAsync(cancellationToken: cancellationToken)
        //    .Select(async applicagtion => await OpenIdConnectAuthorization.)
        //    .ToListAsync(cancellationToken)
        //    .ConfigureAwait(false);

        // TODO Is there a more efficient way to return an `AsyncEnumerable` or `AsyncEnumerator` or
        // to turn such a thing into an `Enumerable` or `Enumerator`?
        return await applicationManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<OpenIddictEntityFrameworkCoreApplication?> GetApplication(
        Guid uuid,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        Data.ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToViewApplications(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false))
        {
            return null;
        }

        return await applicationManager.FindByIdAsync(uuid.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}