using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Metabase.GraphQl.Institutions;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Metabase.Tests.Integration.GraphQl.Institutions;

[TestFixture]
public sealed class CreateInstitutionTests
    : InstitutionIntegrationTests
{
    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task AnonymousUser_IsAuthenticationError()
    {
        // Act
        var response =
            await SuccessfullyQueryGraphQlContentAsString(
                File.ReadAllText("Integration/GraphQl/Institutions/CreateInstitution.graphql"),
                variables: PendingInstitutionInput
            ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task AnonymousUser_CannotCreateInstitution()
    {
        // Act
        await SuccessfullyQueryGraphQlContentAsString(
            File.ReadAllText("Integration/GraphQl/Institutions/CreateInstitution.graphql"),
            variables: PendingInstitutionInput
        ).ConfigureAwait(false);
        var response = await GetInstitutions().ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }

    [TestCaseSource(nameof(EnumerateInstitutionInputs))]
    [Theory]
    [SuppressMessage("Naming", "CA1707")]
    public async Task LoggedInUser_IsSuccess(
        string key,
        CreateInstitutionInput input
    )
    {
        var testName = SnapshotFullNameHelper(typeof(CreateInstitutionTests), key);

        // Arrange
        var userId = await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        // Act
        var response = await CreateInstitution(
            input with
            {
                OwnerIds = new[] { userId }
            }
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            testName,
            matchOptions => matchOptions
                .Assert(fieldOptions =>
                    fieldOptions.Field<string>("data.createInstitution.institution.id").Should()
                        .NotBeNullOrWhiteSpace()
                )
                .Assert(fieldOptions =>
                    fieldOptions.Field<Guid>("data.createInstitution.institution.uuid").Should().NotBe(Guid.Empty)
                )
        );
    }

    [TestCaseSource(nameof(EnumerateInstitutionInputs))]
    [Theory]
    [SuppressMessage("Naming", "CA1707")]
    public async Task LoggedInUser_CreatesInstitution(
        string key,
        CreateInstitutionInput input
    )
    {
        var testName = SnapshotFullNameHelper(typeof(CreateInstitutionTests), key);

        // Arrange
        var userId = await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        // Act
        var (institutionId, institutionUuid) = await CreateInstitutionReturningIdAndUuid(
            input with
            {
                OwnerIds = new[] { userId }
            }
        ).ConfigureAwait(false);
        var response = await GetPendingInstitutions().ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            testName,
            matchOptions => matchOptions
                .Assert(fieldOptions =>
                    fieldOptions.Field<string>("data.pendingInstitutions.edges[*].node.id").Should().Be(institutionId)
                )
                .Assert(fieldOptions =>
                    fieldOptions.Field<Guid>("data.pendingInstitutions.edges[*].node.uuid").Should().Be(institutionUuid)
                )
        );
    }
}