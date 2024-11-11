using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Application;

[ExtendObjectType(nameof(Mutation))]
public sealed class ApplicationMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<OpenIddictEntityFrameworkCoreApplication> CreateApplicationAsync(
        ApplicationDto application,
        ClaimsPrincipal claimsPrincipal,
        [Service(ServiceKind.Resolver)] UserManager<User> userManager,
        ApplicationDbContext context,
        [Service] OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager,
        CancellationToken cancellationToken
    )
    {
        //if (!await ApplicationAuthorization.IsAuthorizedToCreateComponentForInstitution(
        //        claimsPrincipal,
        //        input.ManufacturerId,
        //        userManager,
        //        context,
        //        cancellationToken).ConfigureAwait(false))
        //{
        //    return new CreateApplicationPayload(
        //        new CreateApplicationError(
        //            CreateApplicationErrorCode.UNAUTHORIZED,
        //            "You are not authorized to create Applications for the institution.",
        //            new[] { nameof(input), nameof(input.ManufacturerId).FirstCharToLower() }
        //        )
        //    );
        //}

        var task = manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = application.ClientId,
            ClientSecret = application.ClientSecret,
            DisplayName = application.DisplayName,
            Permissions =
            {
                // TODO use passed permissions
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Logout,
                OpenIddictConstants.Permissions.Endpoints.Token
            }
        }, cancellationToken).AsTask().ConfigureAwait(false);

        return await task;
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<OpenIddictEntityFrameworkCoreApplication> UpdateApplicationAsync(
        ApplicationDto applicationDto,
        ClaimsPrincipal claimsPrincipal,
        [Service(ServiceKind.Resolver)] UserManager<User> userManager,
        [Service] OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        //if (!await ApplicationAuthorization.IsAuthorizedToUpdate(
        //        claimsPrincipal,
        //        input.ApplicationId,
        //        userManager,
        //        context,
        //        cancellationToken
        //    ).ConfigureAwait(false)
        //   )
        //    return new UpdateApplicationPayload(
        //        new UpdateApplicationError(
        //            UpdateApplicationErrorCode.UNAUTHORIZED,
        //            "You are not authorized to update the Application.",
        //            Array.Empty<string>()
        //        )
        //    );

        var application = await manager.FindByIdAsync(applicationDto.Id, cancellationToken).AsTask().ConfigureAwait(false) ??
            throw new InvalidOperationException("Client application cannot be found.");
        application.ClientId = applicationDto.ClientId;
        application.ClientSecret = applicationDto.ClientSecret;
        application.DisplayName = applicationDto.DisplayName;
        application.Permissions = applicationDto.Permissions;

        await manager.UpdateAsync(application, cancellationToken).ConfigureAwait(false);

        return application;
    }
}