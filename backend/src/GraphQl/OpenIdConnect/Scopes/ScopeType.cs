using HotChocolate.Types;
using Metabase.Data;
using OpenIddict.Abstractions;

namespace Metabase.GraphQl.OpenIdConnect.Scopes;

public sealed class ScopeType
    : ObjectType<OpenIdScope>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIdScope> descriptor
    )
    {
        const string suffixedName = nameof(ScopeType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor.Field(t => t.Name).Type<NonNullType<StringType>>().Resolve(context =>
        {
            var scope = context.Parent<OpenIdScope>();
            return OpenIddictConstants.Permissions.Prefixes.Scope + scope.Name;
        });

        descriptor.Field(scope => scope.Properties).Ignore();
        descriptor.Field(scope => scope.ConcurrencyToken).Ignore();
        descriptor.Field(scope => scope.Resources).Ignore();
        descriptor.Field(scope => scope.Descriptions).Ignore();
        descriptor.Field(scope => scope.DisplayNames).Ignore();
    }
}