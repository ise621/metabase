using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Metabase.Tests.Integration.GraphQl.Users;

[TestFixture]
public sealed class ResetUserPasswordTests
    : UserIntegrationTests
{
    private async Task<string> RegisterAndConfirmUserAndRequestPasswordReset(
        string email,
        string password
    )
    {
        await RegisterAndConfirmUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        EmailSender.Clear();
        await RequestUserPasswordReset(
            email
        ).ConfigureAwait(false);
        return ExtractResetCodeFromEmail();
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task ValidData_ResetsUserPassword()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        const string NewPassword = "new" + Password;
        // Act
        var response = await ResetUserPassword(
            Email,
            NewPassword,
            resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser(
            Email,
            NewPassword
        ).ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task InvalidResetCode_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "new" + Password,
            "invalid" + resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordConfirmationMismatch_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "new" + Password,
            resetCode,
            "other" + Password).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresDigit_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "aabb@$CCDD",
            resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresLower_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "AABB@$567",
            resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresNonAlphanumeric_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "aaBBccDDeeFF123",
            resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresUpper_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "aabb@$567",
            resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordTooShort_IsUserError()
    {
        // Arrange
        const string Email = "john.doe@ise.fraunhofer.de";
        const string Password = "aaaAAA123$!@";
        var resetCode = await RegisterAndConfirmUserAndRequestPasswordReset(
            Email,
            Password
        ).ConfigureAwait(false);
        // Act
        var response = await ResetUserPassword(
            Email,
            "aA@$567",
            resetCode
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }
}