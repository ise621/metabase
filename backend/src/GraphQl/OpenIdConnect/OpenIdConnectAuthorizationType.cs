using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect;

public sealed class OpenIdConnectAuthorizationType
    : ObjectType<OpenIddictEntityFrameworkCoreAuthorization>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreAuthorization> descriptor
    )
    {
        const string SuffixedName = nameof(OpenIdConnectAuthorizationType);
        descriptor.Name(SuffixedName.Remove(SuffixedName.Length - "Type".Length));
    }
}