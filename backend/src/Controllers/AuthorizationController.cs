// Inspired by https://github.com/openiddict/openiddict-core/blob/b898e2d21f30c75d04206870e27bde31c500491f/samples/Mvc.Server/Controllers/AuthorizationController.cs

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.ViewModels.Authorization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Metabase.Controllers;

public sealed class AuthorizationController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<User> signInManager,
        UserManager<User> userManager)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    private async Task<AuthenticateResult> AuthenticateAsync(
        string scheme
    )
    {
        var result = await HttpContext.AuthenticateAsync(scheme).ConfigureAwait(false);
        if (result.Principal is not null) HttpContext.User = result.Principal;

        return result;
    }

    private async Task<ClaimsPrincipal> CreateUserPrincipalAsync(
        User user,
        ImmutableArray<string> scopes,
        Func<ClaimsPrincipal, Task>? extend = null
    )
    {
        var principal = await _signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);
        // Use `user.Name` instead of the default value `user.UserName` for
        // the claim `Claims.Name`.
        principal.SetClaim(Claims.Name, user.Name);
        principal.SetClaim(Claims.Email, user.Email);
        principal.SetScopes(scopes);
        // Resources are used as audiences of issued access tokens. The
        // audience of the identity token though is always the client (and
        // not the resource server).
        principal.SetResources(
            await _scopeManager.ListResourcesAsync(
                    principal.GetScopes()
                )
                .ToListAsync()
                .ConfigureAwait(false)
        );
        if (extend is not null) await extend(principal).ConfigureAwait(false);

        // Set claim destinations when the respective scopes are granted.
        // For details see
        // https://documentation.openiddict.com/configuration/claim-destinations.html
        foreach (var claim in principal.Claims)
            claim.SetDestinations(
                GetDestinations(claim, principal)
            );

        return principal;
    }

    private async Task CreatePermanentAuthorization(
        ClaimsPrincipal principal,
        User user,
        List<object> authorizations,
        string applicationId
    )
    {
        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        authorization ??= await _authorizationManager.CreateAsync(
                principal,
                await _userManager.GetUserIdAsync(user).ConfigureAwait(false),
                applicationId,
                AuthorizationTypes.Permanent,
                principal.GetScopes()
            )
            .ConfigureAwait(false);
        principal.SetAuthorizationId(
            await _authorizationManager.GetIdAsync(authorization).ConfigureAwait(false)
        );
    }

    #region Password, authorization code, device and refresh token flows

    [HttpPost("~/connect/token")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsPasswordGrantType())
        {
            var user = request.Username is null
                ? null
                : await _userManager.FindByNameAsync(request.Username).ConfigureAwait(false);
            if (user is null)
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The username/password couple is invalid."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = request.Password is null
                ? null
                : await _signInManager.CheckPasswordSignInAsync(user, request.Password, true)
                    .ConfigureAwait(false);
            if (result is null || !result.Succeeded)
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The username/password couple is invalid."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Note: in this sample, the granted scopes match the requested scope
            // but you may want to allow the user to uncheck specific scopes.
            // For that, simply restrict the list of scopes.
            var principal = await CreateUserPrincipalAsync(
                user,
                request.GetScopes()
            ).ConfigureAwait(false);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsAuthorizationCodeGrantType() || request.IsDeviceCodeGrantType() ||
            request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the authorization code/device code/refresh token.
            var principal = (await AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
                .ConfigureAwait(false)).Principal;
            if (principal is null)
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The token is no longer valid."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // Note: if you want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change, use the following line instead:
            // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
            var user = await _userManager.GetUserAsync(principal).ConfigureAwait(false);
            if (user is null)
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The token is no longer valid."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user).ConfigureAwait(false))
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is no longer allowed to sign in."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            foreach (var claim in principal.Claims) claim.SetDestinations(GetDestinations(claim, principal));

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    #endregion

    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            // https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            // Note that the information for the respective scopes can also be fetched from the userinfo endpoint.
            case Claims.Name:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }

    #region Authorization code, implicit and hybrid flows

    public async Task<IActionResult> DoSignIn(
        ClaimsPrincipal claimsPrincipal
    )
    {
        // Remove the `Identity.Application` cookie as it was only needed to authenticate the user.
        await _signInManager.SignOutAsync().ConfigureAwait(false);
        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the user principal stored in the authentication cookie.
        // If a max_age parameter was provided, ensure that the cookie is not too old.
        // If the user principal can't be extracted or the cookie is too old, redirect the user to the login page.
        var result = await AuthenticateAsync(AuthConfiguration.IdentityConstantsApplicationScheme)
            .ConfigureAwait(false);
        if (result?.Succeeded != true || (
                request.MaxAge != null
                && result.Properties?.IssuedUtc != null
                && DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value)
            )
           )
        {
            // If the client application requested promptless authentication,
            // return an error indicating that the user is not logged in.
            if (request.HasPrompt(Prompts.None))
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is not logged in."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );

            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                },
                AuthConfiguration.IdentityConstantsApplicationScheme
            );
        }

        // If prompt=login was specified by the client application,
        // immediately return the user agent to the login page.
        if (request.HasPrompt(Prompts.Login))
        {
            // To avoid endless login -> authorization redirects, the prompt=login flag
            // is removed from the authorization request payload before redirecting the user.
            var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));

            var parameters = Request.HasFormContentType
                ? Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
                : Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
                },
                AuthConfiguration.IdentityConstantsApplicationScheme);
        }

        // Retrieve the profile of the logged in user.
        var user = (
                       result.Principal is null
                           ? null
                           : await _userManager.GetUserAsync(result.Principal).ConfigureAwait(false)
                   )
                   ?? throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the application details from the database.
        var application = (
                              request.ClientId is null
                                  ? null
                                  : await _applicationManager.FindByClientIdAsync(request.ClientId)
                                      .ConfigureAwait(false)
                          )
                          ?? throw new InvalidOperationException(
                              "Details concerning the calling client application cannot be found.");
        // Can't we not just use `request.ClientId`?
        var applicationId =
            await _applicationManager.GetIdAsync(application).ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                "Details concerning the calling client application cannot be found.");
        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await _authorizationManager.FindAsync(
                await _userManager.GetUserIdAsync(user).ConfigureAwait(false),
                applicationId,
                Statuses.Valid,
                AuthorizationTypes.Permanent,
                request.GetScopes()
            )
            .ToListAsync()
            .ConfigureAwait(false);

        switch (await _applicationManager.GetConsentTypeAsync(application).ConfigureAwait(false))
        {
            // If the consent is external (e.g when authorizations are granted by a sysadmin),
            // immediately return an error if no authorization can be found in the database.
            case ConsentTypes.External when authorizations.Count == 0:
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // If the consent is implicit or if an authorization was found,
            // return an authorization response without displaying the consent form.
            case ConsentTypes.Implicit:
            case ConsentTypes.External when authorizations.Count > 0:
            case ConsentTypes.Explicit when authorizations.Count > 0 && !request.HasPrompt(Prompts.Consent):
                // Note: The granted scopes match the requested scope
                // but we may want to allow the user to uncheck specific scopes.
                // For that, simply restrict the list of scopes.
                var principal = await CreateUserPrincipalAsync(
                        user,
                        request.GetScopes(),
                        async principal =>
                            await CreatePermanentAuthorization(
                                    principal,
                                    user,
                                    authorizations,
                                    applicationId
                                )
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false);
                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return await DoSignIn(principal).ConfigureAwait(false);

            // At this point, no authorization was found in the database and an error must be returned
            // if the client application specified prompt=none in the authorization request.
            case ConsentTypes.Explicit when request.HasPrompt(Prompts.None):
            case ConsentTypes.Systematic when request.HasPrompt(Prompts.None):
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Interactive user consent is required."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // In every other case, render the consent form.
            default:
                return View(new AuthorizeViewModel
                {
                    ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application)
                        .ConfigureAwait(false),
                    Scope = request.Scope
                });
        }
    }

    [Authorize(AuthenticationSchemes = AuthConfiguration.IdentityConstantsApplicationScheme)]
    [FormValueRequired("submit.Accept")]
    [HttpPost("~/connect/authorize")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the profile of the logged in user.
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false) ??
                   throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the application details from the database.
        var application = (
                              request.ClientId is null
                                  ? null
                                  : await _applicationManager.FindByClientIdAsync(request.ClientId)
                                      .ConfigureAwait(false)
                          )
                          ?? throw new InvalidOperationException(
                              "Details concerning the calling client application cannot be found.");
        // Can't we just use `request.ClientId`?
        var applicationId =
            await _applicationManager.GetIdAsync(application).ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                "Details concerning the calling client application cannot be found.");

        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await _authorizationManager.FindAsync(
                await _userManager.GetUserIdAsync(user).ConfigureAwait(false),
                applicationId,
                Statuses.Valid,
                AuthorizationTypes.Permanent,
                request.GetScopes()
            )
            .ToListAsync()
            .ConfigureAwait(false);

        // Note: the same check is already made in the other action but is repeated
        // here to ensure a malicious user can't abuse this POST-only endpoint and
        // force it to return a valid response without the external authorization.
        if (authorizations.Count == 0 && await _applicationManager
                .HasConsentTypeAsync(application, ConsentTypes.External).ConfigureAwait(false))
            return Forbid(
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The logged in user is not allowed to access this client application."
                }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, simply restrict the list of scopes.
        var principal = await CreateUserPrincipalAsync(
                user,
                request.GetScopes(),
                async principal =>
                    await CreatePermanentAuthorization(
                            principal,
                            user,
                            authorizations,
                            applicationId
                        )
                        .ConfigureAwait(false)
            )
            .ConfigureAwait(false);

        return await DoSignIn(principal).ConfigureAwait(false);
    }

    [Authorize(AuthenticationSchemes = AuthConfiguration.IdentityConstantsApplicationScheme)]
    [FormValueRequired("submit.Deny")]
    [HttpPost("~/connect/authorize")]
    [ValidateAntiForgeryToken]
    // Notify OpenIddict that the authorization grant has been denied by the resource owner
    // to redirect the user agent to the client application using the appropriate response_mode.
    public async Task<IActionResult> Deny()
    {
        // Remove the `Identity.Application` cookie as it was only needed to authenticate the user.
        await _signInManager.SignOutAsync().ConfigureAwait(false);
        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    #endregion

    #region Device flow

    [Authorize(AuthenticationSchemes = AuthConfiguration.IdentityConstantsApplicationScheme)]
    [HttpGet("~/connect/verify")]
    public async Task<IActionResult> Verify()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // If the user code was not specified in the query string (e.g as part of the verification_uri_complete),
        // render a form to ask the user to enter the user code manually (non-digit chars are automatically ignored).
        if (string.IsNullOrEmpty(request.UserCode)) return View(new VerifyViewModel());

        // Retrieve the claims principal associated with the user code.
        var result = await AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
            .ConfigureAwait(false);
        if (result.Succeeded)
        {
            var clientId = result.Principal?.GetClaim(Claims.ClientId);
            var scopes = result.Principal?.GetScopes();
            if (clientId is not null && scopes is not null)
            {
                // Retrieve the application details from the database using the client_id stored in the principal.
                var application = await _applicationManager.FindByClientIdAsync(clientId).ConfigureAwait(false) ??
                                  throw new InvalidOperationException(
                                      "Details concerning the calling client application cannot be found.");

                // Render a form asking the user to confirm the authorization demand.
                return View(new VerifyViewModel
                {
                    ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application)
                        .ConfigureAwait(false),
                    Scope = string.Join(" ", scopes),
                    UserCode = request.UserCode
                });
            }
        }

        // Redisplay the form when the user code is not valid.
        return View(new VerifyViewModel
        {
            Error = Errors.InvalidToken,
            ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
        });
    }

    [Authorize(AuthenticationSchemes = AuthConfiguration.IdentityConstantsApplicationScheme)]
    [FormValueRequired("submit.Accept")]
    [HttpPost("~/connect/verify")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyAccept()
    {
        // Retrieve the profile of the logged in user.
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false) ??
                   throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the claims principal associated with the user code.
        var result = await AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
            .ConfigureAwait(false);
        if (result.Succeeded)
        {
            // Note: in this sample, the granted scopes match the requested scope
            // but you may want to allow the user to uncheck specific scopes.
            // For that, simply restrict the list of scopes.
            var principal = await CreateUserPrincipalAsync(
                user,
                result.Principal?.GetScopes() ??
                throw new InvalidOperationException("The scopes cannot be retrieved.")
            ).ConfigureAwait(false);
            var properties = new AuthenticationProperties
            {
                // This property points to the address OpenIddict will automatically
                // redirect the user to after validating the authorization demand.
                RedirectUri = "/"
            };
            return await DoSignIn(principal).ConfigureAwait(false);
        }

        // Redisplay the form when the user code is not valid.
        return View(new VerifyViewModel
        {
            Error = Errors.InvalidToken,
            ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
        });
    }

    [Authorize(AuthenticationSchemes = AuthConfiguration.IdentityConstantsApplicationScheme)]
    [FormValueRequired("submit.Deny")]
    [HttpPost("~/connect/verify")]
    [ValidateAntiForgeryToken]
    // Notify OpenIddict that the authorization grant has been denied by the resource owner.
    public async Task<IActionResult> VerifyDeny()
    {
        // Remove the `Identity.Application` cookie as it was only needed to authenticate the user.
        await _signInManager.SignOutAsync().ConfigureAwait(false);
        return Forbid(
            new AuthenticationProperties
            {
                // This property points to the address OpenIddict will automatically
                // redirect the user to after rejecting the authorization demand.
                RedirectUri = "/"
            },
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
    }

    #endregion

    #region Logout support for interactive flows like code and implicit

    [HttpGet("~/connect/logout")]
    public IActionResult Logout()
    {
        return View();
    }

    // [Authorize(AuthenticationSchemes = AuthConfiguration.IdentityConstantsApplicationScheme)]
    [ActionName(nameof(Logout))]
    [HttpPost("~/connect/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await _signInManager.SignOutAsync().ConfigureAwait(false);

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = "/"
            },
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    #endregion
}

public static class AsyncEnumerableExtensions
{
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source) list.Add(element);

            return list;
        }
    }
}

public sealed class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
    private readonly string _name;

    public FormValueRequiredAttribute(string name)
    {
        _name = name;
    }

    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    {
        if (string.Equals(routeContext.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase))
            return false;

        if (string.IsNullOrEmpty(routeContext.HttpContext.Request.ContentType)) return false;

        if (!routeContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded",
                StringComparison.OrdinalIgnoreCase))
            return false;

        return !string.IsNullOrEmpty(routeContext.HttpContext.Request.Form[_name]);
    }
}