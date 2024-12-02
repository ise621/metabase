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

namespace Metabase.GraphQl.OpenIdConnect.Tokens;

[ExtendObjectType(nameof(Mutation))]
public sealed class TokenMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RevokeTokenPayload> RevokeTokenAsync(
    RevokeTokenInput input,
    ClaimsPrincipal claimsPrincipal,
    UserManager<User> userManager,
    OpenIddictTokenManager<OpenIddictEntityFrameworkCoreToken> manager,
    CancellationToken cancellationToken
)
    {
        if (input.Id == null)
        {
            return new RevokeTokenPayload(
                new RevokeTokenError(RevokeTokenErrorCode.UNKNOWN,
                "Empty Id",
                Array.Empty<string>()));
        }
        if (!await OpenIdConnectAuthorization.IsAuthorizedToRevokeToken(
                claimsPrincipal,
                userManager
            ).ConfigureAwait(false))
        {
            return new RevokeTokenPayload(
                new RevokeTokenError(
                    RevokeTokenErrorCode.UNAUTHORIZED,
                    "You are not authorized to revoke the token.",
                    Array.Empty<string>()
                )
            );
        }

        var Token = await manager.FindByIdAsync(input.Id, cancellationToken).AsTask().ConfigureAwait(false);

        if (Token is null)
        {
            return new RevokeTokenPayload(
                new RevokeTokenError(
                    RevokeTokenErrorCode.UNKNOWN_TOKEN,
                    "Unknown Token.",
                    new[] { nameof(input), nameof(input.Id).FirstCharToLower() }
                )
            );
        }

        await manager.TryRevokeAsync(Token, cancellationToken).ConfigureAwait(false);

        return new RevokeTokenPayload();
    }
}