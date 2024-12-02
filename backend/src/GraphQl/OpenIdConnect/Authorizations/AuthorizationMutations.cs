using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Authorizations;

[ExtendObjectType(nameof(Mutation))]
public class AuthorizationMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<DeleteAuthorizationPayload> DeleteAuthorizationAsync(
        DeleteAuthorizationInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        OpenIddictAuthorizationManager<OpenIddictEntityFrameworkCoreAuthorization> manager,
        CancellationToken cancellationToken
    )
    {
        if (input.Id == null)
        {
            return new DeleteAuthorizationPayload(
                new DeleteAuthorizationError(DeleteAuthorizationErrorCode.UNKNOWN,
                "Empty Id",
                Array.Empty<string>()));
        }
        if (!await OpenIdConnectAuthorization.IsAuthorizedToDeleteAuthorization(
                claimsPrincipal,
                userManager
            ).ConfigureAwait(false))
        {
            return new DeleteAuthorizationPayload(
                new DeleteAuthorizationError(
                    DeleteAuthorizationErrorCode.UNAUTHORIZED,
                    "You are not authorized to delete the authorization.",
                    Array.Empty<string>()
                )
            );
        }

        var authorization = await manager.FindByIdAsync(input.Id, cancellationToken).AsTask().ConfigureAwait(false);

        if (authorization is null)
        {
            return new DeleteAuthorizationPayload(
                new DeleteAuthorizationError(
                    DeleteAuthorizationErrorCode.UNKNOWN_AUTHORIZATION,
                    "Unknown authorization.",
                    new[] { nameof(input), nameof(input.Id).FirstCharToLower() }
                )
            );
        }

        await manager.DeleteAsync(authorization, cancellationToken).ConfigureAwait(false);

        return new DeleteAuthorizationPayload();
    }
}