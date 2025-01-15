using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Metabase.Controllers;

// Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.DownloadPersonalData.cs.cshtml
//
// Keep in sync with `UserinfoController`.
public sealed class PersonalUserDataController(
    UserManager<User> userManager
    ) : Controller
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
    ~PersonalUserDataController()
    {
        Dispose(false);
    }

    [HttpGet("~/personal-user-data")]
    public async Task<IActionResult> GetAsync()
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var personalData = new Dictionary<string, object>();
        if (User.HasScope(Scopes.Address))
        {
            if (user.PostalAddress is not null)
            {
                personalData[Claims.Address] = user.PostalAddress;
            }
        }

        if (User.HasScope(Scopes.Email))
        {
            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            if (email is not null)
            {
                personalData[Claims.Email] = email;
            }

            personalData[Claims.EmailVerified] =
                await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
        }

        if (User.HasScope(Scopes.Phone))
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
            if (phoneNumber is not null)
            {
                personalData[Claims.PhoneNumber] = phoneNumber;
            }

            personalData[Claims.PhoneNumberVerified] =
                await _userManager.IsPhoneNumberConfirmedAsync(user).ConfigureAwait(false);
        }

        if (User.HasScope(Scopes.Profile))
        {
            // https://openid.net/specs/openid-connect-basic-1_0.html#Scopes
            personalData[Claims.Name] = user.Name;
            // personalData[Claims.UpdatedAt] = ...;
            if (user.WebsiteLocator is not null)
            {
                personalData[Claims.Website] = user.WebsiteLocator;
            }

            var logins = await _userManager.GetLoginsAsync(user).ConfigureAwait(false);
            foreach (var login in logins)
            {
                personalData.Add($"{login.LoginProvider} external login provider key", login.ProviderKey);
            }
        }

        if (User.HasScope(Scopes.Roles))
        {
            personalData[Claims.Role] = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        }

        Response.Headers.Append("Content-Disposition", "attachment; filename=PersonalUserData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}