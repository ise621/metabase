using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Metabase.Tests.Integration.GraphQl.Users;

[TestFixture]
public sealed class ResendUserEmailVerificationTests
    : UserIntegrationTests
{
    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task LoggedInUser_ResendsUserEmailVerification()
    {
        // Arrange
        const string name = "John Doe";
        const string email = "john.doe@ise.fraunhofer.de";
        await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        EmailSender.Clear();
        // Act
        var response = await ResendUserEmailVerification().ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions.Assert(fieldOptions =>
                fieldOptions.Field<string>("data.resendUserEmailVerification.user.id").Should()
                    .NotBeNullOrWhiteSpace()
            )
        );
        EmailsShouldContainSingle(
            (name, email),
            "Confirm your email",
            @"^Please confirm your email address by following the link https:\/\/local\.buildingenvelopedata\.org:4041\/users\/confirm-email\?email=john\.doe@ise\.fraunhofer\.de&confirmationCode=\w+$"
        );
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task NonLoggedInUser_IsAuthenticationError()
    {
        // Arrange
        const string email = "john.doe@ise.fraunhofer.de";
        const string password = DefaultPassword;
        await RegisterAndConfirmUser(
            email: email,
            password: password
        ).ConfigureAwait(false);
        // Act
        var response = await SuccessfullyQueryGraphQlContentAsString(
            File.ReadAllText("Integration/GraphQl/Users/ResendUserEmailVerification.graphql")
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }
}