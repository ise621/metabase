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

namespace Metabase.GraphQl.OpenIdConnect.Authorizations;

public sealed class AuthorizationType
    : ObjectType<OpenIdAuthorization>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIdAuthorization> descriptor
    )
    {
        const string suffixedName = nameof(AuthorizationType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor.Field(authorization => authorization.Tokens).Ignore();
        descriptor.Field(authorization => authorization.Properties).Ignore();
        descriptor.Field(authorization => authorization.Application).Ignore();
        descriptor.Field(authorization => authorization.ConcurrencyToken).Ignore();
        descriptor.Field(authorization => authorization.Scopes).Ignore();

        descriptor
                .Field("canCurrentUserDeleteAuthorization")
                .ResolveWith<AuthorizationResolvers>(x =>
                    AuthorizationResolvers.GetCanCurrentUserDeleteAuthorizationAsync(default!, default!, default!, default!, default!,
                        default!))
                .UseUserManager();
    }

    private sealed class AuthorizationResolvers
    {
        public static Task<bool> GetCanCurrentUserDeleteAuthorizationAsync(
            [Parent] OpenIdAuthorization authorization,
            OpenIddictAuthorizationManager<OpenIdAuthorization> authorizationManager,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return OpenIdConnectAuthorization.IsAuthorizedToDeleteAuthorization(authorization.Id, authorizationManager, claimsPrincipal,
                userManager, context, cancellationToken);
        }
    }
}