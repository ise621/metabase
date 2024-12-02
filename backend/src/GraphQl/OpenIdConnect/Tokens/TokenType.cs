using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Tokens;

public sealed class TokenType
    : ObjectType<OpenIddictEntityFrameworkCoreToken>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreToken> descriptor
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
    }
}