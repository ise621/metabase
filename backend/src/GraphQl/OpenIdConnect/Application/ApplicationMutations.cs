using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Metabase.GraphQl.OpenIdConnect.Application;

[ExtendObjectType(nameof(Mutation))]
public sealed class ApplicationMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<CreateApplicationPayload> CreateApplicationAsync(
        CreateApplicationInput input,
        OpenIddictApplicationManager<OpenIdApplication> applicationManager,
        InstitutionByIdDataLoader institutionById,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToManageApplications(
                claimsPrincipal,
                userManager,
                context, cancellationToken).ConfigureAwait(false))
        {
            return new CreateApplicationPayload(
                new CreateApplicationError(
                    CreateApplicationErrorCode.UNAUTHORIZED,
                    "You are not authorized to create applications.",
                    new[] { nameof(input), nameof(input.ClientId).FirstCharToLower() }
                )
            );
        }

        var institution = await institutionById.LoadAsync(input.AssociatedInstitutionId, cancellationToken).ConfigureAwait(false);
        if (institution == null)
        {
            return new CreateApplicationPayload(
                new CreateApplicationError(
                    CreateApplicationErrorCode.UNKNOWN_INSTITUTION,
                    "No institution with this id found.",
                    new[] { nameof(input), nameof(input.AssociatedInstitutionId).FirstCharToLower() }
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

        foreach (var permission in input.Permissions)
        {
            descriptor.Permissions.Add(permission);
        }
        var application = await applicationManager.CreateAsync(descriptor, cancellationToken).ConfigureAwait(false);
        context.ApplicationInstitutions.Add(new InstitutionApplication
        {
            ApplicationId = application.Id,
            InstitutionId = institution.Id
        });
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new CreateApplicationPayload(application);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateApplicationPayload> UpdateApplicationAsync(
        UpdateApplicationInput input,
        OpenIddictApplicationManager<OpenIdApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToManageApplication(
                input.ApplicationId,
                applicationManager,
                claimsPrincipal,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return new UpdateApplicationPayload(
                new UpdateApplicationError(
                    UpdateApplicationErrorCode.UNAUTHORIZED,
                    "You are not authorized to update the application.",
                    new[] { nameof(input), nameof(input.ApplicationId).FirstCharToLower() }
                )
            );
        }
        if (input.ApplicationId == Guid.Empty)
        {
            return new UpdateApplicationPayload(
                new UpdateApplicationError(UpdateApplicationErrorCode.UNKNOWN,
                "Empty Application Id",
                new[] { nameof(input), nameof(input.ApplicationId).FirstCharToLower() }));
        }

        var application = await applicationManager.FindByIdAsync(input.ApplicationId.ToString(), cancellationToken).ConfigureAwait(false);

        if (application is null)
        {
            return new UpdateApplicationPayload(
                new UpdateApplicationError(
                    UpdateApplicationErrorCode.UNKNOWN_APPLICATION,
                    "Unknown application.",
                    new[] { nameof(input), nameof(input.ApplicationId).FirstCharToLower() }
                )
            );
        }

        var descriptor = new OpenIddictApplicationDescriptor();
        await applicationManager.PopulateAsync(descriptor, application, cancellationToken).ConfigureAwait(false);
        UpdateApplicationDescriptor(input, descriptor);
        await applicationManager.UpdateAsync(application, descriptor, cancellationToken).ConfigureAwait(false);

        return new UpdateApplicationPayload(application);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<DeleteApplicationPayload> DeleteApplicationAsync(
        DeleteApplicationInput input,
        OpenIddictApplicationManager<OpenIdApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await OpenIdConnectAuthorization.IsAuthorizedToManageApplication(
                input.ApplicationId,
                applicationManager,
                claimsPrincipal,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return new DeleteApplicationPayload(
                new DeleteApplicationError(
                    DeleteApplicationErrorCode.UNAUTHORIZED,
                    "You are not authorized to delete the application.",
                    new[] { nameof(input), nameof(input.ApplicationId).FirstCharToLower() }
                )
            );
        }
        if (input.ApplicationId == Guid.Empty)
        {
            return new DeleteApplicationPayload(
                new DeleteApplicationError(DeleteApplicationErrorCode.UNKNOWN,
                    "Empty Application Id",
                    new[] { nameof(input), nameof(input.ApplicationId).FirstCharToLower() }));
        }

        var application = await applicationManager.FindByIdAsync(input.ApplicationId.ToString(), cancellationToken).ConfigureAwait(false);

        if (application is null)
        {
            return new DeleteApplicationPayload(
                new DeleteApplicationError(
                    DeleteApplicationErrorCode.UNKNOWN_APPLICATION,
                    "Unknown application.",
                    new[] { nameof(input), nameof(input.ApplicationId).FirstCharToLower() }
                )
            );
        }

        await applicationManager.DeleteAsync(application, cancellationToken).ConfigureAwait(false);

        return new DeleteApplicationPayload();
    }

    private static void UpdateApplicationDescriptor(UpdateApplicationInput input, OpenIddictApplicationDescriptor descriptor)
    {
        descriptor.ClientId = input.ClientId;
        descriptor.DisplayName = input.DisplayName;
        descriptor.RedirectUris.Clear();
        descriptor.PostLogoutRedirectUris.Clear();
        descriptor.RedirectUris.Add(new Uri(input.RedirectUri, UriKind.Absolute));
        descriptor.PostLogoutRedirectUris.Add(new Uri(input.PostLogoutRedirectUri, UriKind.Absolute));
        descriptor.Permissions.RemoveWhere(permission => permission.Contains(AuthConfiguration.ScopePrefixApi + ":"));
        foreach (var permission in input.Permissions)
        {
            descriptor.Permissions.Add(permission);
        }
    }
}