using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Metabase.Tests.Integration.GraphQl.Users;

[TestFixture]
public sealed class ChangeUserPasswordTests
    : UserIntegrationTests
{
    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task ValidData_ChangesUserPassword()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        const string newPassword = "new" + password;
        // Act
        var response = await ChangeUserPassword(
            password,
            newPassword
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser(
            email,
            newPassword
        ).ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task NonLoggedInUser_IsAuthenticationError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        const string newPassword = "new" + password;
        // Act
        var response = await SuccessfullyQueryGraphQlContentAsString(
            File.ReadAllText("Integration/GraphQl/Users/ChangeUserPassword.graphql"),
            variables: new Dictionary<string, object?>
            {
                ["currentPassword"] = password,
                ["newPassword"] = newPassword,
                ["newPasswordConfirmation"] = newPassword
            }
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task UnconfirmedUser_IsError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "new" + password
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions
                .Assert(fieldOptions =>
                    fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
                )
        );
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordConfirmationMismatch_IsUserError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "new" + password,
            "other" + password
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresDigit_IsUserError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "aabb@$CCDD"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresLower_IsUserError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "AABB@$567"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresNonAlphanumeric_IsUserError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "aaBBccDDeeFF123"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordRequiresUpper_IsUserError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "aabb@$567"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser().ConfigureAwait(false);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task PasswordTooShort_IsUserError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = "aaaAAA123$!@";
        await RegisterAndConfirmAndLoginUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await ChangeUserPassword(
            password,
            "aA@$567"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.changeUserPassword.user.id").Should().NotBeNullOrWhiteSpace()
            )
        );
        await LoginUser().ConfigureAwait(false);
    }
}