using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect;

public sealed class OpenIdConnectScopeType
    : ObjectType<OpenIddictEntityFrameworkCoreScope>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreScope> descriptor
    )
    {
        const string SuffixedName = nameof(OpenIdConnectScopeType);
        descriptor.Name(SuffixedName.Remove(SuffixedName.Length - "Type".Length));
    }
}