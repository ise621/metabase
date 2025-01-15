using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;

namespace Metabase.Authorization;

public static class OpenIdConnectAuthorization
{
    public static async Task<bool> IsAuthorizedToViewApplications(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || await CommonAuthorization.IsOwner(user, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToManageApplications(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || await CommonAuthorization.IsOwner(user, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToManageApplication(
        Guid applicationId,
        OpenIddictApplicationManager<OpenIdApplication> applicationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        var institution = GetInstitutionByApplicationId(applicationId, context);
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || institution is not null && await CommonAuthorization.IsOwnerOfInstitution(user, institution.Id, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToDeleteAuthorization(
        Guid authorizationId,
        OpenIddictAuthorizationManager<OpenIdAuthorization> authorizationManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        var authorization = await authorizationManager.FindByIdAsync(authorizationId.ToString(), cancellationToken).ConfigureAwait(false);
        var institution = authorization != null && authorization.Application != null ? GetInstitutionByApplicationId(authorization.Application.Id, context) : null;
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || institution is not null && await CommonAuthorization.IsOwnerOfInstitution(user, institution.Id, context, cancellationToken));
    }

    public static async Task<bool> IsAuthorizedToRevokeToken(
        Guid tokenId,
        OpenIddictTokenManager<OpenIdToken> tokenManager,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        var token = await tokenManager.FindByIdAsync(tokenId.ToString(), cancellationToken).ConfigureAwait(false);
        var institution = token != null && token.Application != null ? GetInstitutionByApplicationId(token.Application.Id, context) : null;
        return user is not null
               && (await CommonAuthorization.IsAdministrator(user, userManager)
               || institution is not null && await CommonAuthorization.IsOwnerOfInstitution(user, institution.Id, context, cancellationToken));
    }

    private static Institution? GetInstitutionByApplicationId(Guid applicationId, ApplicationDbContext context)
    {
        var applicationInstitution = context.ApplicationInstitutions.Where(x => x.ApplicationId == applicationId).SingleOrDefault();
        return applicationInstitution?.Institution;
    }
}