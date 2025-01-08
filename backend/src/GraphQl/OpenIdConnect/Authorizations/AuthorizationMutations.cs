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
        OpenIddictAuthorizationManager<OpenIdAuthorization> authorizationManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToDeleteAuthorization(
                input.AuthorizationId,
                authorizationManager,
                claimsPrincipal,
                userManager,
                context,
                cancellationToken).ConfigureAwait(false))
        {
            return new DeleteAuthorizationPayload(
                new DeleteAuthorizationError(
                    DeleteAuthorizationErrorCode.UNAUTHORIZED,
                    "You are not authorized to delete the authorization.",
                    new[] { nameof(input), nameof(input.AuthorizationId).FirstCharToLower() }
                )
            );
        }
        if (input.AuthorizationId != Guid.Empty)
        {
            return new DeleteAuthorizationPayload(
                new DeleteAuthorizationError(DeleteAuthorizationErrorCode.UNKNOWN,
                "Empty Authorization Id",
                Array.Empty<string>()));
        }

        var authorization = await authorizationManager.FindByIdAsync(input.AuthorizationId.ToString(), cancellationToken).ConfigureAwait(false);

        if (authorization is null)
        {
            return new DeleteAuthorizationPayload(
                new DeleteAuthorizationError(
                    DeleteAuthorizationErrorCode.UNKNOWN_AUTHORIZATION,
                    "Unknown authorization.",
                    new[] { nameof(input), nameof(input.AuthorizationId).FirstCharToLower() }
                )
            );
        }

        await authorizationManager.DeleteAsync(authorization, cancellationToken).ConfigureAwait(false);

        return new DeleteAuthorizationPayload();
    }
}