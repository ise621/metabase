using HotChocolate;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System.Linq;

namespace Metabase.GraphQl.OpenIdConnect.Application;

[ExtendObjectType(nameof(Query))]
public sealed class ApplicationQueries
{
    // TODO In all queries, instead of returning nothing, report as authentication error to client.
    [UseUserManager]
    public async Task<IList<OpenIddictEntityFrameworkCoreApplication>> GetOpenIdConnectApplications(
        [Service] OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager,
        ClaimsPrincipal claimsPrincipal,
        [Service(ServiceKind.Resolver)] UserManager<User> userManager,
        // Data.ApplicationDbContext context, // TODO Make the application manager use the scoped
        // database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToView(claimsPrincipal, userManager)
                .ConfigureAwait(false))
            return Array.Empty<OpenIddictEntityFrameworkCoreApplication>();

        // TODO Is there a more efficient way to return an `AsyncEnumerable` or `AsyncEnumerator` or
        // to turn such a thing into an `Enumerable` or `Enumerator`?
        return await manager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [UseUserManager]
    public async Task<OpenIddictEntityFrameworkCoreApplication?> GetOpenIdConnectApplication(
        Guid uuid,
        [Service] OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager,
        ClaimsPrincipal claimsPrincipal,
        [Service(ServiceKind.Resolver)] UserManager<User> userManager,
        //Data.ApplicationDbContext context, // TODO Make the application manager use the scoped database context.
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToView(claimsPrincipal, userManager)
                .ConfigureAwait(false))
            return null;

        return await manager.FindByIdAsync(uuid.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}