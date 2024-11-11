using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Metabase.GraphQl.Components;
using Metabase.Tests.Integration.GraphQl.Institutions;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Metabase.Tests.Integration.GraphQl.Components;

[TestFixture]
public sealed class CreateComponentTests
    : ComponentIntegrationTests
{
    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task AnonymousUser_IsAuthenticationError()
    {
        // Act
        var response =
            await SuccessfullyQueryGraphQlContentAsString(
                File.ReadAllText("Integration/GraphQl/Components/CreateComponent.graphql"),
                variables: MinimalComponentInput
            ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task AnonymousUser_CannotCreateComponent()
    {
        // Act
        await SuccessfullyQueryGraphQlContentAsString(
            File.ReadAllText("Integration/GraphQl/Components/CreateComponent.graphql"),
            variables: MinimalComponentInput
        ).ConfigureAwait(false);
        var response = await GetComponents().ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }

    [TestCaseSource(nameof(EnumerateComponentInputs))]
    [Theory]
    [SuppressMessage("Naming", "CA1707")]
    public async Task LoggedInUser_IsSuccess(
        string key,
        CreateComponentInput input
    )
    {
        var testName = SnapshotFullNameHelper(typeof(CreateComponentTests), key);

        // Arrange
        var userId = await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        var institutionId = await InstitutionIntegrationTests.CreateAndVerifyInstitutionReturningUuid(
            HttpClient,
            AppSettings.BootstrapUserPassword,
            InstitutionIntegrationTests.PendingInstitutionInput with
            {
                OwnerIds = new[] { userId }
            }
        ).ConfigureAwait(false);
        // Act
        var response = await CreateComponent(
            input with
            {
                ManufacturerId = institutionId
            }
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            testName,
            matchOptions => matchOptions
                .Assert(fieldOptions =>
                    fieldOptions.Field<string>("data.createComponent.component.id").Should().NotBeNullOrWhiteSpace()
                )
                .Assert(fieldOptions =>
                    fieldOptions.Field<Guid>("data.createComponent.component.uuid").Should().NotBe(Guid.Empty)
                )
        );
    }

    [TestCaseSource(nameof(EnumerateComponentInputs))]
    [Theory]
    [SuppressMessage("Naming", "CA1707")]
    public async Task LoggedInUser_CreatesComponent(
        string key,
        CreateComponentInput input
    )
    {
        var testName = SnapshotFullNameHelper(typeof(CreateComponentTests), key);

        // Arrange
        var userId = await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        var institutionId = await InstitutionIntegrationTests.CreateAndVerifyInstitutionReturningUuid(
            HttpClient,
            AppSettings.BootstrapUserPassword,
            InstitutionIntegrationTests.PendingInstitutionInput with
            {
                OwnerIds = new[] { userId }
            }
        ).ConfigureAwait(false);
        // Act
        var (componentId, componentUuid) = await CreateComponentReturningIdAndUuid(
            input with
            {
                ManufacturerId = institutionId
            }
        ).ConfigureAwait(false);
        var response = await GetComponents().ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            testName,
            matchOptions => matchOptions
                .Assert(fieldOptions =>
                    fieldOptions.Field<string>("data.components.edges[*].node.id").Should().Be(componentId)
                )
                .Assert(fieldOptions =>
                    fieldOptions.Field<Guid>("data.components.edges[*].node.uuid").Should().Be(componentUuid)
                )
        );
    }
}