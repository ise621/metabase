using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Metabase.Tests.Integration.GraphQl.Institutions;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Metabase.Tests.Integration.GraphQl.Components;

[TestFixture]
public sealed class GetComponentTests
    : ComponentIntegrationTests
{
    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task NoComponent_Fails()
    {
        // Act
        var response = await GetComponent(
            "68ccd42538d8490095051f4d0beb2837"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task UnknownId_Fails()
    {
        // Arrange
        var userId = await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        var institutionId = await InstitutionIntegrationTests.CreateAndVerifyInstitutionReturningUuid(
            HttpClient,
            AppSettings.BootstrapUserPassword,
            InstitutionIntegrationTests.PendingInstitutionInput with
            {
                OwnerIds = [userId]
            }
        ).ConfigureAwait(false);
        await CreateComponentReturningIdAndUuid(
            MinimalComponentInput with
            {
                ManufacturerId = institutionId
            }
        ).ConfigureAwait(false);
        await LogoutUser().ConfigureAwait(false);
        // Act
        // There is some tiny probability that the hard-coded identifier is
        // the one of the component in which case this test fails.
        var response = await GetComponent(
            "68ccd42538d8490095051f4d0beb2837"
        ).ConfigureAwait(false);
        // Assert
        Snapshot.Match(response);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task KnownId_Succeeds()
    {
        // Arrange
        var userId = await RegisterAndConfirmAndLoginUser().ConfigureAwait(false);
        var institutionId = await InstitutionIntegrationTests.CreateAndVerifyInstitutionReturningUuid(
            HttpClient,
            AppSettings.BootstrapUserPassword,
            InstitutionIntegrationTests.PendingInstitutionInput with
            {
                OwnerIds = [userId]
            }
        ).ConfigureAwait(false);
        var componentIdsAndUuids = new List<(string, string)>();
        foreach (var input in ComponentInputs)
        {
            componentIdsAndUuids.Add(
                await CreateComponentReturningIdAndUuid(
                    input with
                    {
                        ManufacturerId = institutionId
                    }
                ).ConfigureAwait(false)
            );
        }

        await LogoutUser().ConfigureAwait(false);
        // Act
        var response = await GetComponent(componentIdsAndUuids[1].Item2).ConfigureAwait(false);
        // Assert
        Snapshot.Match(
            response,
            matchOptions => matchOptions
                .Assert(fieldOptions =>
                    fieldOptions.Field<string>("data.component.id").Should().Be(componentIdsAndUuids[1].Item1)
                )
                .Assert(fieldOptions =>
                    fieldOptions.Field<Guid>("data.component.uuid").Should().Be(componentIdsAndUuids[1].Item2)
                )
        );
    }
}