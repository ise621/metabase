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
    OpenIddictTokenManager<OpenIdToken> tokenManager,
    ApplicationDbContext context,
    CancellationToken cancellationToken
)
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToRevokeToken(
                input.TokenId,
                tokenManager,
                claimsPrincipal,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return new RevokeTokenPayload(
                new RevokeTokenError(
                    RevokeTokenErrorCode.UNAUTHORIZED,
                    "You are not authorized to revoke the token.",
                    new[] { nameof(input), nameof(input.TokenId).FirstCharToLower() }
                )
            );
        }
        if (input.TokenId != Guid.Empty)
        {
            return new RevokeTokenPayload(
                new RevokeTokenError(RevokeTokenErrorCode.UNKNOWN,
                "Empty Token Id",
                Array.Empty<string>()));
        }

        var Token = await tokenManager.FindByIdAsync(input.TokenId.ToString(), cancellationToken).ConfigureAwait(false);

        if (Token is null)
        {
            return new RevokeTokenPayload(
                new RevokeTokenError(
                    RevokeTokenErrorCode.UNKNOWN_TOKEN,
                    "Unknown Token.",
                    new[] { nameof(input), nameof(input.TokenId).FirstCharToLower() }
                )
            );
        }

        await tokenManager.TryRevokeAsync(Token, cancellationToken).ConfigureAwait(false);

        return new RevokeTokenPayload();
    }
}