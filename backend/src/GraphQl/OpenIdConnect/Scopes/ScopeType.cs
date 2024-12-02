using System.Collections.Generic;
using System.Text.Json;
using HotChocolate.Types;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Scopes;

public sealed class ScopeType
    : ObjectType<OpenIddictEntityFrameworkCoreScope>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreScope> descriptor
    )
    {
        const string suffixedName = nameof(ScopeType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor.Field(t => t.Name).Type<NonNullType<StringType>>().Resolve(context =>
        {
            var scope = context.Parent<OpenIddictEntityFrameworkCoreScope>();
            return OpenIddictConstants.Permissions.Prefixes.Scope + scope.Name;
        });

        descriptor.Field(scope => scope.Properties).Ignore();
        descriptor.Field(scope => scope.ConcurrencyToken).Ignore();
        descriptor.Field(scope => scope.Resources).Ignore();
        descriptor.Field(scope => scope.Descriptions).Ignore();
        descriptor.Field(scope => scope.DisplayNames).Ignore();
    }
}