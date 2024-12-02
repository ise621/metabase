using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Authorizations;

public sealed class AuthorizationType
    : ObjectType<OpenIddictEntityFrameworkCoreAuthorization>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreAuthorization> descriptor
    )
    {
        const string suffixedName = nameof(AuthorizationType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor.Field(authorization => authorization.Tokens).Ignore();
        descriptor.Field(authorization => authorization.Properties).Ignore();
        descriptor.Field(authorization => authorization.Application).Ignore();
        descriptor.Field(authorization => authorization.ConcurrencyToken).Ignore();
        descriptor.Field(authorization => authorization.Scopes).Ignore();
    }
}