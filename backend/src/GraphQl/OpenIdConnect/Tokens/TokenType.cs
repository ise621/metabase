using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;

namespace Metabase.GraphQl.OpenIdConnect.Tokens;

public sealed class TokenType
    : ObjectType<OpenIdToken>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIdToken> descriptor
    )
    {
        const string suffixedName = nameof(TokenType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor.Field(token => token.Application).Ignore();
        descriptor.Field(token => token.Authorization).Ignore();
        descriptor.Field(token => token.Properties).Ignore();
        descriptor.Field(token => token.ReferenceId).Ignore();
        descriptor.Field(token => token.Payload).Ignore();
        descriptor.Field(token => token.ConcurrencyToken).Ignore();

        descriptor
                .Field("canCurrentUserRevokeToken")
                .ResolveWith<TokenResolvers>(x =>
                    TokenResolvers.GetCanCurrentUserRevokeTokenAsync(default!, default!, default!, default!, default!,
                        default!))
                .UseUserManager();
    }

    private sealed class TokenResolvers
    {
        public static Task<bool> GetCanCurrentUserRevokeTokenAsync(
            [Parent] OpenIdToken token,
            OpenIddictTokenManager<OpenIdToken> tokenManager,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return OpenIdConnectAuthorization.IsAuthorizedToRevokeToken(token.Id, tokenManager, claimsPrincipal,
                userManager, context, cancellationToken);
        }
    }
}