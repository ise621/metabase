using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect;

public sealed class OpenIdConnectApplicationType
    : ObjectType<OpenIddictEntityFrameworkCoreApplication>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreApplication> descriptor
    )
    {
        const string SuffixedName = nameof(OpenIdConnectApplicationType);
        descriptor.Name(SuffixedName.Remove(SuffixedName.Length - "Type".Length));
    }
}