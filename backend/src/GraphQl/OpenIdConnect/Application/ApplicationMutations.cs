using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Application;

[ExtendObjectType(nameof(Mutation))]
public sealed class ApplicationMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<CreateApplicationPayload> CreateApplicationAsync(
        CreateApplicationInput input,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IWebHostEnvironment environment,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToManageApplications(
                claimsPrincipal,
                userManager).ConfigureAwait(false))
        {
            return new CreateApplicationPayload(
                new CreateApplicationError(
                    CreateApplicationErrorCode.UNAUTHORIZED,
                    "You are not authorized to create applications.",
                    new[] { nameof(input), nameof(input.ClientId).FirstCharToLower() }
                )
            );
        }
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = input.ClientId,
            ClientSecret = "application.ClientSecret",
            DisplayName = input.DisplayName,
            ConsentType = environment.IsEnvironment(Program.TestEnvironment)
                        ? OpenIddictConstants.ConsentTypes.Systematic
                        : OpenIddictConstants.ConsentTypes.Explicit,
            RedirectUris =
            {
                new Uri(environment.IsEnvironment(Program.TestEnvironment)
                    ? "urn:test"
                    : input.RedirectUri,
                    UriKind.Absolute)
            },
            PostLogoutRedirectUris =
            {
                new Uri(environment.IsEnvironment(Program.TestEnvironment)
                    ? "urn:test"
                    : input.PostLogoutRedirectUri,
                    UriKind.Absolute)
            },
            Permissions =
            {
                // Add default permissions
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.Logout,
                OpenIddictConstants.Permissions.Endpoints.Revocation,
                OpenIddictConstants.Permissions.Endpoints.Token,
                environment.IsEnvironment(Program.TestEnvironment)
                    ? OpenIddictConstants.Permissions.GrantTypes.Password
                    : OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                environment.IsEnvironment(Program.TestEnvironment)
                    ? OpenIddictConstants.Permissions.ResponseTypes.Token
                    : OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        var permissions = JsonSerializer.Deserialize<List<string>>(input.Permissions);
        permissions?.ForEach(permission => descriptor.Permissions.Add(permission));

        return new CreateApplicationPayload(await applicationManager.CreateAsync(descriptor, cancellationToken).AsTask().ConfigureAwait(false));
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateApplicationPayload> UpdateApplicationAsync(
        UpdateApplicationInput input,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        CancellationToken cancellationToken
    )
    {
        if (input.Id == null)
        {
            return new UpdateApplicationPayload(
                new UpdateApplicationError(UpdateApplicationErrorCode.UNKNOWN,
                "Empty Id",
                Array.Empty<string>()));
        }
        if (!await OpenIdConnectAuthorization.IsAuthorizedToManageApplications(
                claimsPrincipal,
                userManager
            ).ConfigureAwait(false))
        {
            return new UpdateApplicationPayload(
                new UpdateApplicationError(
                    UpdateApplicationErrorCode.UNAUTHORIZED,
                    "You are not authorized to update the application.",
                    Array.Empty<string>()
                )
            );
        }

        var application = await applicationManager.FindByIdAsync(input.Id, cancellationToken).AsTask().ConfigureAwait(false);

        if (application is null)
        {
            return new UpdateApplicationPayload(
                new UpdateApplicationError(
                    UpdateApplicationErrorCode.UNKNOWN_APPLICATION,
                    "Unknown application.",
                    new[] { nameof(input), nameof(input.Id).FirstCharToLower() }
                )
            );
        }

        var descriptor = new OpenIddictApplicationDescriptor();

        await applicationManager.PopulateAsync(descriptor, application, cancellationToken).ConfigureAwait(false);

        descriptor.ClientId = input.ClientId;
        descriptor.DisplayName = input.DisplayName;
        descriptor.RedirectUris.Clear();
        descriptor.PostLogoutRedirectUris.Clear();
        descriptor.RedirectUris.Add(new Uri(input.RedirectUri, UriKind.Absolute));
        descriptor.PostLogoutRedirectUris.Add(new Uri(input.PostLogoutRedirectUri, UriKind.Absolute));
        descriptor.Permissions.RemoveWhere(permission => permission.Contains("api"));
        var permissions = JsonSerializer.Deserialize<List<string>>(input.Permissions);
        permissions?.ForEach(permission => descriptor.Permissions.Add(permission));

        await applicationManager.UpdateAsync(application, descriptor, cancellationToken).ConfigureAwait(false);

        return new UpdateApplicationPayload(application);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<DeleteApplicationPayload> DeleteApplicationAsync(
        DeleteApplicationInput input,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        CancellationToken cancellationToken
    )
    {
        if (input.Id == null)
        {
            return new DeleteApplicationPayload(
                new DeleteApplicationError(DeleteApplicationErrorCode.UNKNOWN,
                "Empty Id",
                Array.Empty<string>()));
        }
        if (!await OpenIdConnectAuthorization.IsAuthorizedToManageApplications(
                claimsPrincipal,
                userManager
            ).ConfigureAwait(false))
        {
            return new DeleteApplicationPayload(
                new DeleteApplicationError(
                    DeleteApplicationErrorCode.UNAUTHORIZED,
                    "You are not authorized to delete the application.",
                    Array.Empty<string>()
                )
            );
        }

        var application = await applicationManager.FindByIdAsync(input.Id, cancellationToken).AsTask().ConfigureAwait(false);

        if (application is null)
        {
            return new DeleteApplicationPayload(
                new DeleteApplicationError(
                    DeleteApplicationErrorCode.UNKNOWN_APPLICATION,
                    "Unknown application.",
                    new[] { nameof(input), nameof(input.Id).FirstCharToLower() }
                )
            );
        }

        await applicationManager.DeleteAsync(application, cancellationToken).ConfigureAwait(false);

        return new DeleteApplicationPayload();
    }
}