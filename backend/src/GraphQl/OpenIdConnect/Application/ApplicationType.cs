using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HotChocolate.Types;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class ApplicationType
    : ObjectType<OpenIddictEntityFrameworkCoreApplication>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIddictEntityFrameworkCoreApplication> descriptor
    )
    {
        const string suffixedName = nameof(ApplicationType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor.Field(t => t.Permissions).Type<NonNullType<ListType<StringType>>>().Resolve(context =>
        {
            var application = context.Parent<OpenIddictEntityFrameworkCoreApplication>();
            var permissions = JsonSerializer.Deserialize<List<string>>(application.Permissions!);
            return permissions?.FindAll(permission => permission.Contains("api")).ToList();
        });

        descriptor.Field(t => t.RedirectUris).Type<NonNullType<StringType>>().Resolve(context =>
        {
            var application = context.Parent<OpenIddictEntityFrameworkCoreApplication>();
            if (string.IsNullOrEmpty(application.RedirectUris))
            {
                return string.Empty;
            }
            else
            {
                return JsonSerializer.Deserialize<List<string>>(application.RedirectUris!)?[0];
            }
        });

        descriptor.Field(t => t.PostLogoutRedirectUris).Type<NonNullType<StringType>>().Resolve(context =>
        {
            var application = context.Parent<OpenIddictEntityFrameworkCoreApplication>();
            if (string.IsNullOrEmpty(application.PostLogoutRedirectUris))
            {
                return string.Empty;
            }
            else
            {
                return JsonSerializer.Deserialize<List<string>>(application.PostLogoutRedirectUris!)?[0];
            }
        });

        descriptor.Field(application => application.DisplayNames).Ignore();
        descriptor.Field(application => application.JsonWebKeySet).Ignore();
        descriptor.Field(application => application.Tokens).Ignore();
        descriptor.Field(application => application.Requirements).Ignore();
        descriptor.Field(application => application.Properties).Ignore();
        descriptor.Field(application => application.ApplicationType).Ignore();
        descriptor.Field(application => application.Authorizations).Ignore();
        descriptor.Field(application => application.Settings).Ignore();
        descriptor.Field(application => application.ClientType).Ignore();
        descriptor.Field(application => application.ConsentType).Ignore();
        descriptor.Field(application => application.ConcurrencyToken).Ignore();
    }
}