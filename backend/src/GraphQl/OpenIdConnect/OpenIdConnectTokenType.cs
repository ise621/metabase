using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect;

public sealed class OpenIdConnectTokenType
    : ObjectType<OpenIddictEntityFrameworkCoreToken>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreToken> descriptor
    )
    {
        const string SuffixedName = nameof(OpenIdConnectTokenType);
        descriptor.Name(SuffixedName.Remove(SuffixedName.Length - "Type".Length));
    }
}