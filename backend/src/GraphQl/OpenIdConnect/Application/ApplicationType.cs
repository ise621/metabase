﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class ApplicationType
    : ObjectType<OpenIdApplication>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIdApplication> descriptor
    )
    {
        const string suffixedName = nameof(ApplicationType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor
            .Field(application => application.Permissions)
            .Type<NonNullType<ListType<StringType>>>()
            .Resolve(context =>
        {
            var application = context.Parent<OpenIdApplication>();
            var permissions = JsonSerializer.Deserialize<List<string>>(application.Permissions!);
            return permissions?.FindAll(permission => permission.Contains(AuthConfiguration.ScopePrefixApi)).ToList();
        });

        descriptor
            .Field(application => application.RedirectUris)
            .Name("redirectUri")
            .Type<StringType>()
            .Resolve(context =>
        {
            var application = context.Parent<OpenIdApplication>();
            if (string.IsNullOrEmpty(application.RedirectUris))
            {
                throw new GraphQLException("Redirect URI is null or empty");
            }
            else
            {
                return JsonSerializer.Deserialize<List<string>>(application.RedirectUris!)?[0];
            }
        });

        descriptor
            .Field(application => application.PostLogoutRedirectUris)
            .Name("postLogoutRedirectUri")
            .Type<StringType>()
            .Resolve(context =>
        {
            var application = context.Parent<OpenIdApplication>();
            if (string.IsNullOrEmpty(application.PostLogoutRedirectUris))
            {
                throw new GraphQLException("Post Logout Redirect URI is null or empty");
            }
            else
            {
                return JsonSerializer.Deserialize<List<string>>(application.PostLogoutRedirectUris!)?[0];
            }
        });
        descriptor
            .Field(application => application.Institutions)
            .Type<NonNullType<ObjectType<ApplicationInstitutionConnection>>>()
            .Resolve(context =>
                new ApplicationInstitutionConnection(
                    context.Parent<OpenIdApplication>()
                )
            );

        descriptor.Field(application => application.InstitutionEdges).Ignore();
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

        descriptor
            .Field("canCurrentUserManageApplication")
            .ResolveWith<ApplicationResolvers>(_ =>
                ApplicationResolvers.GetCanCurrentUserManageApplicationAsync(default!, default!, default!, default!, default!,
                    default!))
            .UseUserManager();
    }

    private sealed class ApplicationResolvers
    {
        public static Task<bool> GetCanCurrentUserManageApplicationAsync(
            [Parent] OpenIdApplication application,
            OpenIddictApplicationManager<OpenIdApplication> applicationManager,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return OpenIdConnectAuthorization.IsAuthorizedToManageApplication(application.Id, applicationManager, claimsPrincipal,
                userManager, context, cancellationToken);
        }
    }
}