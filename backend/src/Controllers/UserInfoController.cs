using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Metabase.Controllers;

// Inspired by https://github.com/openiddict/openiddict-core/blob/rel/6.0.0/sandbox/OpenIddict.Sandbox.AspNetCore.Server/Controllers/UserinfoController.cs
//
// Keep in sync with `PersonalUserDataController`.
public sealed class UserInfoController(UserManager<User> userManager) : Controller
{
    private readonly UserManager<User> _userManager = userManager;
    private bool _disposed;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!_disposed)
        {
            // Dispose of resources held by this instance.
            _userManager.Dispose();
            _disposed = true;
        }
    }

    // Disposable types implement a finalizer.
    ~UserInfoController()
    {
        Dispose(false);
    }

    // GET: /connect/userinfo
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo")]
    [HttpPost("~/connect/userinfo")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> UserInfo()
    {
        var subject = User.GetClaim(Claims.Subject);
        if (subject is null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The subject claim is missing."
                }));
        }

        var user = await _userManager.FindByIdAsync(subject).ConfigureAwait(false);
        if (user is null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }));
        }

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [Claims.Subject] = await _userManager.GetUserIdAsync(user).ConfigureAwait(false)
        };
        if (User.HasScope(Scopes.Address))
        {
            // https://openid.net/specs/openid-connect-basic-1_0.html#AddressClaim
            if (user.PostalAddress is not null)
            {
                // Alternatively, we could use the [raw string literal](https://devblogs.microsoft.com/dotnet/csharp-11-preview-updates/#raw-string-literals)
                // $$"""{ "formatted": "{{user.PostalAddress}}" }""";
                claims[Claims.Address] = JsonSerializer.Serialize(
                    new Address(user.PostalAddress)
                );
            }
        }

        if (User.HasScope(Scopes.Email))
        {
            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            if (email is not null)
            {
                claims[Claims.Email] = email;
            }

            claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
        }

        if (User.HasScope(Scopes.Phone))
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
            if (phoneNumber is not null)
            {
                claims[Claims.PhoneNumber] = phoneNumber;
            }

            claims[Claims.PhoneNumberVerified] =
                await _userManager.IsPhoneNumberConfirmedAsync(user).ConfigureAwait(false);
        }

        if (User.HasScope(Scopes.Profile))
        {
            // https://openid.net/specs/openid-connect-basic-1_0.html#Scopes
            claims[Claims.Name] = user.Name;
            // claims[Claims.UpdatedAt] = ...;
            if (user.WebsiteLocator is not null)
            {
                claims[Claims.Website] = user.WebsiteLocator;
            }
        }

        if (User.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        }

        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        return Ok(claims);
    }

    private readonly record struct Address(string Formatted);
}