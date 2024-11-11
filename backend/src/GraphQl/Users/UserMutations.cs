using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

// Note that `SignInManager` relies on cookies, see https://github.com/aspnet/Identity/issues/1421. For its source code see https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/SignInManager.cs
namespace Metabase.GraphQl.Users;

[ExtendObjectType(nameof(Mutation))]
public sealed class UserMutations
{
    // Key Uri Format https://github.com/google/google-authenticator/wiki/Key-Uri-Format
    private static readonly CompositeFormat AuthenticatorUriFormat =
        CompositeFormat.Parse("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6");

    private static async Task ValidateAntiforgeryTokenAsync(
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new AntiforgeryValidationException(
                "Cannot access the HTTP context to validate the antiforgery token.");

        await antiforgeryService.ValidateRequestAsync(httpContext).ConfigureAwait(false);
    }

    /////////////////////
    // LOGGED-OUT USER //
    /////////////////////

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.ConfirmEmail.cs.cshtml
    // and https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.RegisterConfirmation.cs.cshtml
    // Despite its name, it is also used to confirm registrations. TODO Should we add another mutation for that?
    [UseUserManager]
    public async Task<ConfirmUserEmailPayload> ConfirmUserEmailAsync(
        ConfirmUserEmailInput input,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
        if (user is null)
            return new ConfirmUserEmailPayload(
                new ConfirmUserEmailError(
                    ConfirmUserEmailErrorCode.UNKNOWN_USER,
                    $"Failed to load user with email address {input.Email}.",
                    new[] { nameof(input), nameof(input.Email).FirstCharToLower() }
                )
            );

        var confirmationToken = DecodeCode(input.ConfirmationCode);
        var identityResult = await userManager.ConfirmEmailAsync(user, confirmationToken).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<ConfirmUserEmailError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L140
                    error.Code switch
                    {
                        "InvalidToken" =>
                            new ConfirmUserEmailError(
                                ConfirmUserEmailErrorCode.INVALID_CONFIRMATION_CODE,
                                error.Description,
                                new[] { nameof(input), nameof(input.ConfirmationCode).FirstCharToLower() }
                            ),
                        _ =>
                            new ConfirmUserEmailError(
                                ConfirmUserEmailErrorCode.UNKNOWN,
                                error.Description,
                                new[] { nameof(input) }
                            )
                    }
                );

            return new ConfirmUserEmailPayload(errors);
        }

        return new ConfirmUserEmailPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.ConfirmEmailChange.cs.cshtml
    [UseUserManager]
    [UseSignInManager]
    public async Task<ConfirmUserEmailChangePayload> ConfirmUserEmailChangeAsync(
        ConfirmUserEmailChangeInput input,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        // TODO This public endpoint can be used to test whether there is a user for the given email address. Is this a problem? In other endpoints like `ResetUserPasswordAsync` do not do that on purpose. Why exactly?
        var user = await userManager.FindByEmailAsync(input.CurrentEmail).ConfigureAwait(false);
        if (user is null)
            return new ConfirmUserEmailChangePayload(
                new ConfirmUserEmailChangeError(
                    ConfirmUserEmailChangeErrorCode.UNKNOWN_USER,
                    $"Failed to load user with email address {input.CurrentEmail}.",
                    new[] { nameof(input), nameof(input.CurrentEmail).FirstCharToLower() }
                )
            );

        var changeEmailIdentityResult =
            await userManager.ChangeEmailAsync(
                user,
                input.NewEmail,
                DecodeCode(input.ConfirmationCode)
            ).ConfigureAwait(false);
        // For us email and user name are one and the same, so when we
        // update the email we need to update the user name.
        var setUserNameIdentityResult =
            await userManager.SetUserNameAsync(user, input.NewEmail).ConfigureAwait(false);
        if (!(changeEmailIdentityResult.Succeeded && setUserNameIdentityResult.Succeeded))
        {
            var errors = new List<ConfirmUserEmailChangeError>();
            foreach (var error in changeEmailIdentityResult.Errors)
                errors.Add(
                    // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L140
                    error.Code switch
                    {
                        "DuplicateEmail" =>
                            new ConfirmUserEmailChangeError(
                                ConfirmUserEmailChangeErrorCode.DUPLICATE_EMAIL,
                                error.Description,
                                new[] { nameof(input), nameof(input.NewEmail).FirstCharToLower() }
                            ),
                        "InvalidToken" =>
                            new ConfirmUserEmailChangeError(
                                ConfirmUserEmailChangeErrorCode.INVALID_CONFIRMATION_CODE,
                                error.Description,
                                new[] { nameof(input), nameof(input.ConfirmationCode).FirstCharToLower() }
                            ),
                        _ =>
                            new ConfirmUserEmailChangeError(
                                ConfirmUserEmailChangeErrorCode.UNKNOWN,
                                error.Description,
                                new[] { nameof(input) }
                            )
                    }
                );

            foreach (var error in setUserNameIdentityResult.Errors)
                // Ignore `*UserName` errors that have corresponding `*Email` errors because we use `Email` as `UserName`.
                if (error.Code != "DuplicateUserName")
                    errors.Add(
                        // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L140
                        error.Code switch
                        {
                            _ =>
                                new ConfirmUserEmailChangeError(
                                    ConfirmUserEmailChangeErrorCode.UNKNOWN,
                                    error.Description,
                                    new[] { nameof(input) }
                                )
                        }
                    );

            return new ConfirmUserEmailChangePayload(errors);
        }

        await signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
        return new ConfirmUserEmailChangePayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.Login.cs.cshtml
    [UseUserManager]
    [UseSignInManager]
    public async Task<LoginUserPayload> LoginUserAsync(
        LoginUserInput input,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var signInResult = await signInManager.PasswordSignInAsync(
            input.Email,
            input.Password,
            false,
            true
        ).ConfigureAwait(false);
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.signinresult?view=aspnetcore-5.0
        if (signInResult.IsLockedOut)
            return new LoginUserPayload(
                new LoginUserError(
                    LoginUserErrorCode.LOCKED_OUT,
                    "User is locked out.",
                    new[] { nameof(input) }
                )
            );

        if (signInResult.IsNotAllowed)
            return new LoginUserPayload(
                new LoginUserError(
                    LoginUserErrorCode.NOT_ALLOWED,
                    "User is not allowed to login.",
                    new[] { nameof(input) }
                )
            );

        if (!signInResult.Succeeded && !signInResult.RequiresTwoFactor)
            return new LoginUserPayload(
                new LoginUserError(
                    LoginUserErrorCode.INVALID,
                    "Invalid login attempt.",
                    new[] { nameof(input) }
                )
            );

        // TODO Only load the user if requested in the GraphQl query. Use resolver in payload and just pass email address.
        var user = await userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
        if (user is null)
            return new LoginUserPayload(
                new LoginUserError(
                    LoginUserErrorCode.UNKNOWN,
                    "Failed to fetch user.",
                    new[] { nameof(input) }
                )
            );

        // We could set
        // HttpContext.User = await _signInManager.CreateUserPrincipalAsync(user);
        // and add tokens as cookies as is done in the antiforgery
        // controller (XSRF). However, the GraphQL endpoint allows CORS
        // whereas the antiforgery controller does not making the latter
        // more secure because the antiforgery token should only be
        // requestable from the same domain.
        if (signInResult.RequiresTwoFactor) return new LoginUserPayload(user, true);

        return new LoginUserPayload(user, false);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.LoginWith2fa.cs.cshtml
    [UseUserManager]
    [UseSignInManager]
    public async Task<LoginUserWithTwoFactorCodePayload> LoginUserWithTwoFactorCodeAsync(
        LoginUserWithTwoFactorCodeInput input,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
        if (user is null)
            return new LoginUserWithTwoFactorCodePayload(
                new LoginUserWithTwoFactorCodeError(
                    LoginUserWithTwoFactorCodeErrorCode.UNKNOWN_USER,
                    "Failed to load two-factor authentication user.",
                    Array.Empty<string>()
                )
            );

        var authenticatorCode =
            input.AuthenticatorCode
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);
        var signInResult =
            await signInManager.TwoFactorAuthenticatorSignInAsync(
                authenticatorCode,
                false,
                input.RememberMachine
            ).ConfigureAwait(false);
        if (signInResult.IsLockedOut)
            return new LoginUserWithTwoFactorCodePayload(
                new LoginUserWithTwoFactorCodeError(
                    LoginUserWithTwoFactorCodeErrorCode.LOCKED_OUT,
                    "User is locked out.",
                    new[] { nameof(input) }
                )
            );

        if (signInResult.IsNotAllowed)
            return new LoginUserWithTwoFactorCodePayload(
                new LoginUserWithTwoFactorCodeError(
                    LoginUserWithTwoFactorCodeErrorCode.NOT_ALLOWED,
                    "User is not allowed to login.",
                    new[] { nameof(input) }
                )
            );

        if (!signInResult.Succeeded)
            return new LoginUserWithTwoFactorCodePayload(
                new LoginUserWithTwoFactorCodeError(
                    LoginUserWithTwoFactorCodeErrorCode.INVALID_AUTHENTICATOR_CODE,
                    "Invalid authenticator code.",
                    new[] { nameof(input), nameof(input.AuthenticatorCode).FirstCharToLower() }
                )
            );

        return new LoginUserWithTwoFactorCodePayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.LoginWithRecoveryCode.cs.cshtml
    [UseUserManager]
    [UseSignInManager]
    public async Task<LoginUserWithRecoveryCodePayload> LoginUserWithRecoveryCodeAsync(
        LoginUserWithRecoveryCodeInput input,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
        if (user is null)
            return new LoginUserWithRecoveryCodePayload(
                new LoginUserWithRecoveryCodeError(
                    LoginUserWithRecoveryCodeErrorCode.UNKNOWN_USER,
                    "Failed to load two-factor authentication user.",
                    Array.Empty<string>()
                )
            );

        var recoveryCode =
            input.RecoveryCode
                .Replace(" ", string.Empty);
        var signInResult =
            await signInManager.TwoFactorRecoveryCodeSignInAsync(
                recoveryCode
            ).ConfigureAwait(false);
        if (signInResult.IsLockedOut)
            return new LoginUserWithRecoveryCodePayload(
                new LoginUserWithRecoveryCodeError(
                    LoginUserWithRecoveryCodeErrorCode.LOCKED_OUT,
                    "User is locked out.",
                    new[] { nameof(input) }
                )
            );

        if (signInResult.IsNotAllowed)
            return new LoginUserWithRecoveryCodePayload(
                new LoginUserWithRecoveryCodeError(
                    LoginUserWithRecoveryCodeErrorCode.NOT_ALLOWED,
                    "User is not allowed to login.",
                    new[] { nameof(input) }
                )
            );

        if (!signInResult.Succeeded)
            return new LoginUserWithRecoveryCodePayload(
                new LoginUserWithRecoveryCodeError(
                    LoginUserWithRecoveryCodeErrorCode.INVALID_RECOVERY_CODE,
                    "Invalid recovery code.",
                    new[] { nameof(input), nameof(input.RecoveryCode).FirstCharToLower() }
                )
            );

        return new LoginUserWithRecoveryCodePayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.Register.cs.cshtml
    [UseUserManager]
    public async Task<RegisterUserPayload> RegisterUserAsync(
        RegisterUserInput input,
        UserManager<User> userManager,
        IEmailSender emailSender,
        UrlEncoder urlEncoder,
        AppSettings appSettings,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = new User(
            input.Name,
            input.Email,
            null,
            null
        );
        if (input.Password != input.PasswordConfirmation)
            return new RegisterUserPayload(
                new RegisterUserError(
                    RegisterUserErrorCode.PASSWORD_CONFIRMATION_MISMATCH,
                    "Password and confirmation password do not match.",
                    new[] { nameof(input), nameof(input.PasswordConfirmation).FirstCharToLower() }
                )
            );

        var identityResult =
            await userManager.CreateAsync(
                user,
                input.Password
            ).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<RegisterUserError>();
            foreach (var error in identityResult.Errors)
                // Ignore `*UserName` errors that have corresponding `*Email` errors because we use `Email` as `UserName`.
                if (error.Code != "DuplicateUserName"
                    && error.Code != "InvalidUserName")
                    errors.Add(
                        // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L120
                        error.Code switch
                        {
                            "DuplicateEmail" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.DUPLICATE_EMAIL,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Email).FirstCharToLower() }
                                ),
                            "InvalidEmail" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.INVALID_EMAIL,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Email).FirstCharToLower() }
                                ),
                            "PasswordRequiresDigit" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.PASSWORD_REQUIRES_DIGIT,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordRequiresLower" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.PASSWORD_REQUIRES_LOWER,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordRequiresNonAlphanumeric" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.PASSWORD_REQUIRES_NON_ALPHANUMERIC,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordRequiresUpper" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.PASSWORD_REQUIRES_UPPER,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordTooShort" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.PASSWORD_TOO_SHORT,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PropertyTooShort" =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.NULL_OR_EMPTY_EMAIL,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Email).FirstCharToLower() }
                                ),
                            _ =>
                                new RegisterUserError(
                                    RegisterUserErrorCode.UNKNOWN,
                                    $"{error.Description} (error code `{error.Code}`)",
                                    new[] { nameof(input) }
                                )
                        }
                    );

            return new RegisterUserPayload(errors);
        }

        // TODO The confirmation also confirms the registration/account. Should we use another email text then?
        await SendUserEmailConfirmation(
            (user.Name, input.Email),
            await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false),
            emailSender,
            appSettings.Host,
            input.ReturnTo,
            urlEncoder
        ).ConfigureAwait(false);
        return new RegisterUserPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.ResendEmailConfirmation.cs.cshtml
    [UseUserManager]
    public async Task<ResendUserEmailConfirmationPayload> ResendUserEmailConfirmationAsync(
        ResendUserEmailConfirmationInput input,
        UserManager<User> userManager,
        IEmailSender emailSender,
        UrlEncoder urlEncoder,
        AppSettings appSettings,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
        // Don't reveal that the user does not exist.
        if (user is not null)
            await SendUserEmailConfirmation(
                (user.Name, input.Email),
                await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false),
                emailSender,
                appSettings.Host,
                null,
                urlEncoder
            ).ConfigureAwait(false);

        return new ResendUserEmailConfirmationPayload();
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.ForgotPassword.cs.cshtml
    [UseUserManager]
    public async Task<RequestUserPasswordResetPayload> RequestUserPasswordResetAsync(
        RequestUserPasswordResetInput input,
        UserManager<User> userManager,
        IEmailSender emailSender,
        UrlEncoder urlEncoder,
        AppSettings appSettings,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
        // Don't reveal that the user does not exist or is not confirmed
        if (user is not null && await userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
        {
            // For more information on how to enable account confirmation and password reset please
            // visit https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-5.0
            var resetCode = EncodeToken(
                await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false)
            );
            await emailSender.SendAsync(
                (user.Name, input.Email),
                "Reset password",
                $"Please reset your password by following the link {appSettings.Host}/users/reset-password?resetCode={resetCode}" +
                (input.ReturnTo is null ? "" : $"&returnTo={urlEncoder.Encode(input.ReturnTo.OriginalString)}")
            ).ConfigureAwait(false);
        }

        return new RequestUserPasswordResetPayload();
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.ResetPassword.cs.cshtml
    [UseUserManager]
    public async Task<ResetUserPasswordPayload> ResetUserPasswordAsync(
        ResetUserPasswordInput input,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
        if (input.Password != input.PasswordConfirmation)
            return new ResetUserPasswordPayload(
                new ResetUserPasswordError(
                    ResetUserPasswordErrorCode.PASSWORD_CONFIRMATION_MISMATCH,
                    "Password and confirmation password do not match.",
                    new[] { nameof(input), nameof(input.PasswordConfirmation).FirstCharToLower() }
                )
            );

        // Don't reveal that the user does not exist
        // TODO As said above, do not reveal that the user does or does not exist. However, right now we reveal whether the user exists or not because errors with the password are only reported when the user exists and not otherwise.
        if (user is not null)
        {
            var identityResult = await userManager.ResetPasswordAsync(
                user,
                DecodeCode(input.ResetCode),
                input.Password
            ).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                var errors = new List<ResetUserPasswordError>();
                foreach (var error in identityResult.Errors)
                    errors.Add(
                        // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L120
                        error.Code switch
                        {
                            "InvalidToken" =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.INVALID_RESET_CODE,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.ResetCode).FirstCharToLower() }
                                ),
                            "PasswordRequiresDigit" =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.PASSWORD_REQUIRES_DIGIT,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordRequiresLower" =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.PASSWORD_REQUIRES_LOWER,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordRequiresNonAlphanumeric" =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.PASSWORD_REQUIRES_NON_ALPHANUMERIC,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordRequiresUpper" =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.PASSWORD_REQUIRES_UPPER,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            "PasswordTooShort" =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.PASSWORD_TOO_SHORT,
                                    error.Description,
                                    new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                                ),
                            _ =>
                                new ResetUserPasswordError(
                                    ResetUserPasswordErrorCode.UNKNOWN,
                                    $"{error.Description} (error code `{error.Code}`)",
                                    new[] { nameof(input) }
                                )
                        }
                    );

                return new ResetUserPasswordPayload(errors);
            }
        }

        return new ResetUserPasswordPayload();
    }

    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<DeleteUserPayload> DeleteUserAsync(
        DeleteUserInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        if (!await UserAuthorization.IsAuthorizedToDeleteUsers(
                claimsPrincipal,
                userManager
            ).ConfigureAwait(false)
           )
            return new DeleteUserPayload(
                new DeleteUserError(
                    DeleteUserErrorCode.UNAUTHORIZED,
                    $"You are not authorized to delete user with identifier {input.UserId}.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        var user =
            await userManager.Users.SingleOrDefaultAsync(_ =>
                _.Id == input.UserId
            ).ConfigureAwait(false);
        if (user is null)
            return new DeleteUserPayload(
                new DeleteUserError(
                    DeleteUserErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {input.UserId}.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        var identityResult = await userManager.DeleteAsync(user).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<DeleteUserError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // TODO Which errors can occur here?
                    error.Code switch
                    {
                        _ =>
                            new DeleteUserError(
                                DeleteUserErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new DeleteUserPayload(user, errors);
        }

        return new DeleteUserPayload(user);
    }

    ////////////////////
    // LOGGED-IN USER //
    ////////////////////

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Account.Logout.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<LogoutUserPayload> LogoutUserAsync(
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        await signInManager.SignOutAsync().ConfigureAwait(false);
        return new LogoutUserPayload();
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.ChangePassword.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<ChangeUserPasswordPayload> ChangeUserPasswordAsync(
        ChangeUserPasswordInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new ChangeUserPasswordPayload(
                new ChangeUserPasswordError(
                    ChangeUserPasswordErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        if (!await userManager.HasPasswordAsync(user).ConfigureAwait(false))
            return new ChangeUserPasswordPayload(
                user,
                new ChangeUserPasswordError(
                    ChangeUserPasswordErrorCode.NO_PASSWORD,
                    "You do not have a password yet.",
                    Array.Empty<string>()
                )
            );

        if (input.NewPassword != input.NewPasswordConfirmation)
            return new ChangeUserPasswordPayload(
                user,
                new ChangeUserPasswordError(
                    ChangeUserPasswordErrorCode.PASSWORD_CONFIRMATION_MISMATCH,
                    "Password and confirmation password do not match.",
                    new[] { nameof(input), nameof(input.NewPasswordConfirmation).FirstCharToLower() }
                )
            );

        var identityResult = await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword)
            .ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<ChangeUserPasswordError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L120
                    error.Code switch
                    {
                        "PasswordRequiresDigit" =>
                            new ChangeUserPasswordError(
                                ChangeUserPasswordErrorCode.PASSWORD_REQUIRES_DIGIT,
                                error.Description,
                                new[] { nameof(input), nameof(input.NewPassword).FirstCharToLower() }
                            ),
                        "PasswordRequiresLower" =>
                            new ChangeUserPasswordError(
                                ChangeUserPasswordErrorCode.PASSWORD_REQUIRES_LOWER,
                                error.Description,
                                new[] { nameof(input), nameof(input.NewPassword).FirstCharToLower() }
                            ),
                        "PasswordRequiresNonAlphanumeric" =>
                            new ChangeUserPasswordError(
                                ChangeUserPasswordErrorCode.PASSWORD_REQUIRES_NON_ALPHANUMERIC,
                                error.Description,
                                new[] { nameof(input), nameof(input.NewPassword).FirstCharToLower() }
                            ),
                        "PasswordRequiresUpper" =>
                            new ChangeUserPasswordError(
                                ChangeUserPasswordErrorCode.PASSWORD_REQUIRES_UPPER,
                                error.Description,
                                new[] { nameof(input), nameof(input.NewPassword).FirstCharToLower() }
                            ),
                        "PasswordTooShort" =>
                            new ChangeUserPasswordError(
                                ChangeUserPasswordErrorCode.PASSWORD_TOO_SHORT,
                                error.Description,
                                new[] { nameof(input), nameof(input.NewPassword).FirstCharToLower() }
                            ),
                        _ =>
                            new ChangeUserPasswordError(
                                ChangeUserPasswordErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new ChangeUserPasswordPayload(user, errors);
        }

        await signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
        return new ChangeUserPasswordPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.DeletePersonalData.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<DeletePersonalUserDataPayload> DeletePersonalUserDataAsync(
        DeletePersonalUserDataInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new DeletePersonalUserDataPayload(
                new DeletePersonalUserDataError(
                    DeletePersonalUserDataErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        if (await userManager.HasPasswordAsync(user).ConfigureAwait(false))
        {
            if (input.Password is null)
                return new DeletePersonalUserDataPayload(
                    user,
                    new DeletePersonalUserDataError(
                        DeletePersonalUserDataErrorCode.MISSING_PASSWORD,
                        "Missing password.",
                        new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                    )
                );

            if (!await userManager.CheckPasswordAsync(user, input.Password).ConfigureAwait(false))
                return new DeletePersonalUserDataPayload(
                    user,
                    new DeletePersonalUserDataError(
                        DeletePersonalUserDataErrorCode.INCORRECT_PASSWORD,
                        "Incorrect password.",
                        new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                    )
                );
        }

        var identityResult = await userManager.DeleteAsync(user).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<DeletePersonalUserDataError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // TODO Which errors can occur here?
                    error.Code switch
                    {
                        _ =>
                            new DeletePersonalUserDataError(
                                DeletePersonalUserDataErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new DeletePersonalUserDataPayload(user, errors);
        }

        await signInManager.SignOutAsync().ConfigureAwait(false);
        return new DeletePersonalUserDataPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.Disable2fa.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<DisableUserTwoFactorAuthenticationPayload> DisableUserTwoFactorAuthenticationAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new DisableUserTwoFactorAuthenticationPayload(
                new DisableUserTwoFactorAuthenticationError(
                    DisableUserTwoFactorAuthenticationErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        var disableResult = await userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
        if (!disableResult.Succeeded)
            return new DisableUserTwoFactorAuthenticationPayload(
                new DisableUserTwoFactorAuthenticationError(
                    DisableUserTwoFactorAuthenticationErrorCode.UNKNOWN,
                    "Unknown error.",
                    Array.Empty<string>()
                )
            );

        return new DisableUserTwoFactorAuthenticationPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.TwoFactorAuthentication.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<ForgetUserTwoFactorAuthenticationClientPayload> ForgetUserTwoFactorAuthenticationClientAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new ForgetUserTwoFactorAuthenticationClientPayload(
                new ForgetUserTwoFactorAuthenticationClientError(
                    ForgetUserTwoFactorAuthenticationClientErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        await signInManager.ForgetTwoFactorClientAsync().ConfigureAwait(false);
        return new ForgetUserTwoFactorAuthenticationClientPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.EnableAuthenticator.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload>
        GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriAsync(
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            UrlEncoder urlEncoder,
            IAntiforgery antiforgeryService,
            IHttpContextAccessor httpContextAccessor
        )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload(
                new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriError(
                    GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        return await LoadSharedKeyAndQrCodeUriAsync(userManager, urlEncoder, user).ConfigureAwait(false) switch
        {
            LoadSharedKeyAndQrCodeUriPayload.Success(var sharedKey, var authenticatorUri) =>
                new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload(
                    user,
                    sharedKey,
                    authenticatorUri
                ),
            LoadSharedKeyAndQrCodeUriPayload.ResettingAuthenticatorKeyFailure =>
                new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload(
                    new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriError(
                        GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode
                            .RESETTING_AUTHENTICATOR_KEY_FAILED,
                        "Failed to reset authenticator key.",
                        Array.Empty<string>()
                    )
                ),
            LoadSharedKeyAndQrCodeUriPayload.GettingAuthenticatorKeyFailure =>
                new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload(
                    new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriError(
                        GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode
                            .GETTING_AUTHENTICATOR_KEY_FAILED,
                        "Failed to get authenticator key.",
                        Array.Empty<string>()
                    )
                ),
            LoadSharedKeyAndQrCodeUriPayload.GettingEmailFailure =>
                new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload(
                    new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriError(
                        GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode.GETTING_EMAIL_FAILED,
                        "Failed to get email.",
                        Array.Empty<string>()
                    )
                ),
            _ =>
                new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriPayload(
                    new GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriError(
                        GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode.UNKNOWN,
                        "Failed to load shared key and QR code URI.",
                        Array.Empty<string>()
                    )
                )
        };
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.EnableAuthenticator.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<EnableUserTwoFactorAuthenticatorPayload> EnableUserTwoFactorAuthenticatorAsync(
        EnableUserTwoFactorAuthenticatorInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        UrlEncoder urlEncoder,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new EnableUserTwoFactorAuthenticatorPayload(
                new EnableUserTwoFactorAuthenticatorError(
                    EnableUserTwoFactorAuthenticatorErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        var verificationToken =
            input.VerificationCode
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);
        var isTokenValid =
            await userManager.VerifyTwoFactorTokenAsync(
                    user,
                    userManager.Options.Tokens.AuthenticatorTokenProvider,
                    verificationToken
                )
                .ConfigureAwait(false);
        if (!isTokenValid)
            return await LoadSharedKeyAndQrCodeUriAsync(userManager, urlEncoder, user).ConfigureAwait(false) switch
            {
                LoadSharedKeyAndQrCodeUriPayload.Success(var sharedKey, var authenticatorUri) =>
                    new EnableUserTwoFactorAuthenticatorPayload(
                        new EnableUserTwoFactorAuthenticatorError(
                            EnableUserTwoFactorAuthenticatorErrorCode.INVALID_VERIFICATION_CODE,
                            "Verification code is invalid.",
                            new[] { nameof(input), nameof(input.VerificationCode).FirstCharToLower() }
                        ),
                        sharedKey,
                        authenticatorUri
                    ),
                _ =>
                    new EnableUserTwoFactorAuthenticatorPayload(
                        new EnableUserTwoFactorAuthenticatorError(
                            EnableUserTwoFactorAuthenticatorErrorCode.INVALID_VERIFICATION_CODE,
                            "Verification code is invalid.",
                            new[] { nameof(input), nameof(input.VerificationCode).FirstCharToLower() }
                        )
                    )
            };

        var enableResult = await userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
        if (!enableResult.Succeeded)
            return await LoadSharedKeyAndQrCodeUriAsync(userManager, urlEncoder, user).ConfigureAwait(false) switch
            {
                LoadSharedKeyAndQrCodeUriPayload.Success(var sharedKey, var authenticatorUri) =>
                    new EnableUserTwoFactorAuthenticatorPayload(
                        new EnableUserTwoFactorAuthenticatorError(
                            EnableUserTwoFactorAuthenticatorErrorCode.ENABLING_FAILED,
                            "Unknown error enabling.",
                            Array.Empty<string>()
                        ),
                        sharedKey,
                        authenticatorUri
                    ),
                _ =>
                    new EnableUserTwoFactorAuthenticatorPayload(
                        new EnableUserTwoFactorAuthenticatorError(
                            EnableUserTwoFactorAuthenticatorErrorCode.ENABLING_FAILED,
                            "Unknown error enabling.",
                            Array.Empty<string>()
                        )
                    )
            };

        if (await userManager.CountRecoveryCodesAsync(user).ConfigureAwait(false) == 0)
        {
            var recoveryCodes =
                await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);
            if (recoveryCodes is null) return new EnableUserTwoFactorAuthenticatorPayload(user, Array.Empty<string>());
            // TODO Inform user that recovery code generation failed and the he/she has none left.
            // return new EnableUserTwoFactorAuthenticatorPayload(
            //     new EnableUserTwoFactorAuthenticatorError(
            //       EnableUserTwoFactorAuthenticatorErrorCode.GENERATING_RECOVERY_CODES_FAILED,
            //       "Generating recovery codes failed.",
            //       Array.Empty<string>()
            //       )
            //     );
            return new EnableUserTwoFactorAuthenticatorPayload(user, recoveryCodes.ToList().AsReadOnly());
        }

        return new EnableUserTwoFactorAuthenticatorPayload(user);
    }

    private static async Task<LoadSharedKeyAndQrCodeUriPayload> LoadSharedKeyAndQrCodeUriAsync(
        UserManager<User> userManager, UrlEncoder urlEncoder, User user)
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            var identityResult = await userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            if (!identityResult.Succeeded)
                return new LoadSharedKeyAndQrCodeUriPayload.ResettingAuthenticatorKeyFailure();

            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            if (string.IsNullOrEmpty(unformattedKey))
                return new LoadSharedKeyAndQrCodeUriPayload.GettingAuthenticatorKeyFailure();
        }

        var sharedKey = FormatKey(unformattedKey);
        var email = await userManager.GetEmailAsync(user).ConfigureAwait(false);
        if (email is null) return new LoadSharedKeyAndQrCodeUriPayload.GettingEmailFailure();

        var authenticatorUri = GenerateQrCodeUri(urlEncoder, email, unformattedKey);
        return new LoadSharedKeyAndQrCodeUriPayload.Success(sharedKey, authenticatorUri);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey, currentPosition, 4).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
            result.Append(unformattedKey, currentPosition, unformattedKey.Length - currentPosition);

        return result.ToString().ToLowerInvariant();
    }

    private static string GenerateQrCodeUri(UrlEncoder urlEncoder, string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            urlEncoder.Encode("buildingenvelopedata.org"), // issuer
            urlEncoder.Encode(email), // account name
            unformattedKey // secret
        );
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.ResetAuthenticator.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<ResetUserTwoFactorAuthenticatorPayload> ResetUserTwoFactorAuthenticatorAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new ResetUserTwoFactorAuthenticatorPayload(
                new ResetUserTwoFactorAuthenticatorError(
                    ResetUserTwoFactorAuthenticatorErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        var disableResult = await userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
        if (!disableResult.Succeeded)
            return new ResetUserTwoFactorAuthenticatorPayload(
                new ResetUserTwoFactorAuthenticatorError(
                    ResetUserTwoFactorAuthenticatorErrorCode.DISABLING_FAILED,
                    "Unknown error disabling.",
                    Array.Empty<string>()
                )
            );

        var resetResult = await userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        if (!resetResult.Succeeded)
            return new ResetUserTwoFactorAuthenticatorPayload(
                new ResetUserTwoFactorAuthenticatorError(
                    ResetUserTwoFactorAuthenticatorErrorCode.RESETTING_FAILED,
                    "Unknown error resetting.",
                    Array.Empty<string>()
                )
            );

        await signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
        return new ResetUserTwoFactorAuthenticatorPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.Email.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<ChangeUserEmailPayload> ChangeUserEmailAsync(
        ChangeUserEmailInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IEmailSender emailSender,
        UrlEncoder urlEncoder,
        AppSettings appSettings,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new ChangeUserEmailPayload(
                new ChangeUserEmailError(
                    ChangeUserEmailErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        var currentEmail = await userManager.GetEmailAsync(user).ConfigureAwait(false);
        if (currentEmail is null)
            return new ChangeUserEmailPayload(
                new ChangeUserEmailError(
                    ChangeUserEmailErrorCode.UNKNOWN_CURRENT_EMAIL,
                    "Failed to load current email address.",
                    Array.Empty<string>()
                )
            );

        if (currentEmail == input.NewEmail)
            return new ChangeUserEmailPayload(
                user,
                new ChangeUserEmailError(
                    ChangeUserEmailErrorCode.UNCHANGED_EMAIL,
                    "Your email is unchanged.",
                    new[] { nameof(input), nameof(input.NewEmail).FirstCharToLower() }
                )
            );

        // TODO Check validity of `input.NewEmail` (use error code `INVALID_EMAIL`)
        await SendChangeUserEmailConfirmation(
            user.Name,
            currentEmail,
            input.NewEmail,
            await userManager.GenerateChangeEmailTokenAsync(user, input.NewEmail).ConfigureAwait(false),
            emailSender,
            appSettings.Host,
            urlEncoder
        ).ConfigureAwait(false);
        return new ChangeUserEmailPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.Email.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<ResendUserEmailVerificationPayload> ResendUserEmailVerificationAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IEmailSender emailSender,
        UrlEncoder urlEncoder,
        AppSettings appSettings,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new ResendUserEmailVerificationPayload(
                new ResendUserEmailVerificationError(
                    ResendUserEmailVerificationErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        var email = await userManager.GetEmailAsync(user).ConfigureAwait(false);
        if (email is null)
            return new ResendUserEmailVerificationPayload(
                new ResendUserEmailVerificationError(
                    ResendUserEmailVerificationErrorCode.UNKNOWN_EMAIL,
                    "Failed to load email address.",
                    Array.Empty<string>()
                )
            );

        await SendUserEmailConfirmation(
            (user.Name, email),
            await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false),
            emailSender,
            appSettings.Host,
            null,
            urlEncoder
        ).ConfigureAwait(false);
        return new ResendUserEmailVerificationPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.GenerateRecoveryCodes.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<GenerateUserTwoFactorRecoveryCodesPayload> GenerateUserTwoFactorRecoveryCodesAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new GenerateUserTwoFactorRecoveryCodesPayload(
                new GenerateUserTwoFactorRecoveryCodesError(
                    GenerateUserTwoFactorRecoveryCodesErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        if (!await userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false))
            return new GenerateUserTwoFactorRecoveryCodesPayload(
                user,
                new GenerateUserTwoFactorRecoveryCodesError(
                    GenerateUserTwoFactorRecoveryCodesErrorCode.TWO_FACTOR_AUTHENTICATION_DISABLED,
                    "You have disabled two-factor authentication.",
                    Array.Empty<string>()
                )
            );

        var recoveryCodes =
            await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);
        if (recoveryCodes is null)
            return new GenerateUserTwoFactorRecoveryCodesPayload(
                user,
                new GenerateUserTwoFactorRecoveryCodesError(
                    GenerateUserTwoFactorRecoveryCodesErrorCode.CODE_GENERATION_FAILED,
                    "Code generation failed.",
                    Array.Empty<string>()
                )
            );

        return new GenerateUserTwoFactorRecoveryCodesPayload(
            user,
            recoveryCodes.ToList().AsReadOnly()
        );
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.Index.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<SetUserPhoneNumberPayload> SetUserPhoneNumberAsync(
        SetUserPhoneNumberInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new SetUserPhoneNumberPayload(
                new SetUserPhoneNumberError(
                    SetUserPhoneNumberErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        var currentPhoneNumber = await userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
        if (currentPhoneNumber == input.PhoneNumber)
            return new SetUserPhoneNumberPayload(
                user,
                new SetUserPhoneNumberError(
                    SetUserPhoneNumberErrorCode.UNCHANGED_PHONE_NUMBER,
                    "Your phone number is unchanged.",
                    new[] { nameof(input), nameof(input.PhoneNumber).FirstCharToLower() }
                )
            );

        var identityResult = await userManager.SetPhoneNumberAsync(user, input.PhoneNumber).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<SetUserPhoneNumberError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // TODO Which errors can occur?
                    error.Code switch
                    {
                        _ =>
                            new SetUserPhoneNumberError(
                                SetUserPhoneNumberErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new SetUserPhoneNumberPayload(user, errors);
        }

        await signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
        return new SetUserPhoneNumberPayload(user);
    }

    // Inspired by https://github.com/dotnet/Scaffolding/blob/main/src/Scaffolding/VS.Web.CG.Mvc/Templates/Identity/Bootstrap4/Pages/Account/Manage/Account.Manage.SetPassword.cs.cshtml
    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    [UseSignInManager]
    public async Task<SetUserPasswordPayload> SetUserPasswordAsync(
        SetUserPasswordInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new SetUserPasswordPayload(
                new SetUserPasswordError(
                    SetUserPasswordErrorCode.UNKNOWN_USER,
                    $"Failed to load user with identifier {userManager.GetUserId(claimsPrincipal)}.",
                    Array.Empty<string>()
                )
            );

        if (await userManager.HasPasswordAsync(user).ConfigureAwait(false))
            return new SetUserPasswordPayload(
                user,
                new SetUserPasswordError(
                    SetUserPasswordErrorCode.EXISTING_PASSWORD,
                    "You already have a password.",
                    Array.Empty<string>()
                )
            );

        if (input.Password != input.PasswordConfirmation)
            return new SetUserPasswordPayload(
                user,
                new SetUserPasswordError(
                    SetUserPasswordErrorCode.PASSWORD_CONFIRMATION_MISMATCH,
                    "Password and confirmation password do not match.",
                    new[] { nameof(input), nameof(input.PasswordConfirmation).FirstCharToLower() }
                )
            );

        var identityResult = await userManager.AddPasswordAsync(user, input.Password).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<SetUserPasswordError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // List of codes from https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/Resources.resx#L120
                    error.Code switch
                    {
                        "PasswordRequiresDigit" =>
                            new SetUserPasswordError(
                                SetUserPasswordErrorCode.PASSWORD_REQUIRES_DIGIT,
                                error.Description,
                                new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                            ),
                        "PasswordRequiresLower" =>
                            new SetUserPasswordError(
                                SetUserPasswordErrorCode.PASSWORD_REQUIRES_LOWER,
                                error.Description,
                                new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                            ),
                        "PasswordRequiresNonAlphanumeric" =>
                            new SetUserPasswordError(
                                SetUserPasswordErrorCode.PASSWORD_REQUIRES_NON_ALPHANUMERIC,
                                error.Description,
                                new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                            ),
                        "PasswordRequiresUpper" =>
                            new SetUserPasswordError(
                                SetUserPasswordErrorCode.PASSWORD_REQUIRES_UPPER,
                                error.Description,
                                new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                            ),
                        "PasswordTooShort" =>
                            new SetUserPasswordError(
                                SetUserPasswordErrorCode.PASSWORD_TOO_SHORT,
                                error.Description,
                                new[] { nameof(input), nameof(input.Password).FirstCharToLower() }
                            ),
                        _ =>
                            new SetUserPasswordError(
                                SetUserPasswordErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new SetUserPasswordPayload(user, errors);
        }

        await signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
        return new SetUserPasswordPayload(user);
    }

    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<AddUserRolePayload> AddUserRoleAsync(
        AddUserRoleInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        if (!await UserAuthorization.IsAuthorizedToAddOrRemoveRole(claimsPrincipal, input.Role, userManager)
                .ConfigureAwait(false))
            return new AddUserRolePayload(
                new AddUserRoleError(
                    AddUserRoleErrorCode.UNAUTHORIZED,
                    $"You are not authorized to add role {input.Role}.",
                    Array.Empty<string>()
                )
            );

        var user = await context.Users.AsQueryable()
            .SingleOrDefaultAsync(
                x => x.Id == input.UserId,
                cancellationToken
            ).ConfigureAwait(false);
        if (user is null)
            return new AddUserRolePayload(
                new AddUserRoleError(
                    AddUserRoleErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        var identityResult = await userManager.AddToRoleAsync(user, Role.EnumToName(input.Role))
            .ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<AddUserRoleError>();
            foreach (var error in identityResult.Errors)
                errors.Add(
                    // TODO Which error codes occur here? When known, translate the properly into `AddUserRoleErrorCode`.
                    error.Code switch
                    {
                        _ =>
                            new AddUserRoleError(
                                AddUserRoleErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new AddUserRolePayload(user, errors);
        }

        return new AddUserRolePayload(user);
    }

    [Authorize(Policy = AuthConfiguration.ManageUserPolicy)]
    [UseUserManager]
    public async Task<RemoveUserRolePayload> RemoveUserRoleAsync(
        RemoveUserRoleInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        IAntiforgery antiforgeryService,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        await ValidateAntiforgeryTokenAsync(antiforgeryService, httpContextAccessor).ConfigureAwait(false);
        if (!await UserAuthorization.IsAuthorizedToAddOrRemoveRole(claimsPrincipal, input.Role, userManager)
                .ConfigureAwait(false))
            return new RemoveUserRolePayload(
                new RemoveUserRoleError(
                    RemoveUserRoleErrorCode.UNAUTHORIZED,
                    $"You are not authorized to remove role {input.Role}.",
                    Array.Empty<string>()
                )
            );

        var user = await context.Users.AsQueryable()
            .SingleOrDefaultAsync(
                x => x.Id == input.UserId,
                cancellationToken
            ).ConfigureAwait(false);
        if (user is null)
            return new RemoveUserRolePayload(
                new RemoveUserRoleError(
                    RemoveUserRoleErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    new[] { nameof(input), nameof(input.UserId).FirstCharToLower() }
                )
            );

        var identityResult = await userManager.RemoveFromRoleAsync(user, Role.EnumToName(input.Role))
            .ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            var errors = new List<RemoveUserRoleError>();
            foreach (var error in identityResult.Errors)
                errors.Remove(
                    // TODO Which error codes occur here? When known, translate the properly into `RemoveUserRoleErrorCode`.
                    error.Code switch
                    {
                        _ =>
                            new RemoveUserRoleError(
                                RemoveUserRoleErrorCode.UNKNOWN,
                                $"{error.Description} (error code `{error.Code}`)",
                                new[] { nameof(input) }
                            )
                    }
                );

            return new RemoveUserRolePayload(user, errors);
        }

        return new RemoveUserRolePayload(user);
    }

    private static async Task SendUserEmailConfirmation(
        (string name, string address) recipient,
        string confirmationToken,
        IEmailSender emailSender,
        string host,
        Uri? returnTo,
        UrlEncoder urlEncoder
    )
    {
        var confirmationCode = EncodeToken(confirmationToken);
        await emailSender.SendAsync(
                recipient,
                "Confirm your email",
                $"Please confirm your email address by following the link {host}/users/confirm-email?email={urlEncoder.Encode(recipient.address)}&confirmationCode={urlEncoder.Encode(confirmationCode)}" +
                (returnTo is null ? "" : $"&returnTo={urlEncoder.Encode(returnTo.OriginalString)}"))
            .ConfigureAwait(false);
    }

    private static async Task SendChangeUserEmailConfirmation(
        string name,
        string currentEmail,
        string newEmail,
        string confirmationToken,
        IEmailSender emailSender,
        string host,
        UrlEncoder urlEncoder
    )
    {
        var confirmationCode = EncodeToken(confirmationToken);
        await emailSender.SendAsync(
                (name, newEmail),
                "Confirm your email change",
                $"Please confirm your email address change by following the link {host}/users/confirm-email-change?currentEmail={urlEncoder.Encode(currentEmail)}&newEmail={urlEncoder.Encode(newEmail)}&confirmationCode={confirmationCode}")
            .ConfigureAwait(false);
    }

    private static string EncodeToken(string token)
    {
        return
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(
                    token
                )
            );
    }

    private static string DecodeCode(string code)
    {
        return
            Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(
                    code
                )
            );
    }

    public abstract record LoadSharedKeyAndQrCodeUriPayload
    {
        private LoadSharedKeyAndQrCodeUriPayload()
        {
        }

        public sealed record Success(string SharedKey, string AuthenticatorUri) : LoadSharedKeyAndQrCodeUriPayload;

        public sealed record ResettingAuthenticatorKeyFailure : LoadSharedKeyAndQrCodeUriPayload;

        public sealed record GettingAuthenticatorKeyFailure : LoadSharedKeyAndQrCodeUriPayload;

        public sealed record GettingEmailFailure : LoadSharedKeyAndQrCodeUriPayload;
    }
}