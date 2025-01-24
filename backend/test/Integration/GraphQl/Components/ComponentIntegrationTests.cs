using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Metabase.Enumerations;
using Metabase.GraphQl.Common;
using Metabase.GraphQl.Components;

namespace Metabase.Tests.Integration.GraphQl.Components;

public abstract class ComponentIntegrationTests
    : IntegrationTests
{
    internal static CreateComponentInput MinimalComponentInput { get; } = new(
        "Component A",
        "C!A",
        "Best component ever!",
        null,
        [],
        null,
        null,
        null,
        Guid.Empty
    );

    internal static CreateComponentInput FromAndToRestrictedAvailabilityComponentInput { get; } = new(
        "Component B",
        "C!B",
        "Another component!",
        new OpenEndedDateTimeRangeInput(
            new DateTime(2020, 1, 1, 8, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 11, 5, 20, 0, 0, DateTimeKind.Utc)
        ),
        [
            ComponentCategory.MATERIAL,
            ComponentCategory.UNIT
        ],
        null,
        null,
        null,
        Guid.Empty
    );

    internal static CreateComponentInput ToRestrictedAvailabilityComponentInput { get; } = new(
        "Component C",
        "C!C",
        "Yet another component!",
        new OpenEndedDateTimeRangeInput(
            null,
            new DateTime(2021, 11, 5, 20, 0, 0, DateTimeKind.Utc)
        ),
        [
            ComponentCategory.UNIT
        ],
        null,
        null,
        null,
        Guid.Empty
    );

    internal static CreateComponentInput FromRestrictedAvailabilityComponentInput { get; } = new(
        "Component D",
        "C!D",
        "Whatever component!",
        new OpenEndedDateTimeRangeInput(
            new DateTime(2019, 4, 3, 0, 0, 0, DateTimeKind.Utc),
            null
        ),
        [
            ComponentCategory.LAYER,
            ComponentCategory.MATERIAL,
            ComponentCategory.UNIT
        ],
        null,
        null,
        null,
        Guid.Empty
    );

    internal static IEnumerable<CreateComponentInput> ComponentInputs
    {
        get
        {
            yield return MinimalComponentInput;
            yield return FromAndToRestrictedAvailabilityComponentInput;
            yield return ToRestrictedAvailabilityComponentInput;
            yield return FromRestrictedAvailabilityComponentInput;
        }
    }

    internal static IEnumerable<object[]> EnumerateComponentInputs()
    {
        yield return new object[] { nameof(MinimalComponentInput), MinimalComponentInput };
        yield return new object[]
        {
            nameof(FromAndToRestrictedAvailabilityComponentInput), FromAndToRestrictedAvailabilityComponentInput
        };
        yield return new object[]
            { nameof(ToRestrictedAvailabilityComponentInput), ToRestrictedAvailabilityComponentInput };
        yield return new object[]
            { nameof(FromRestrictedAvailabilityComponentInput), FromRestrictedAvailabilityComponentInput };
    }

    protected Task<string> GetComponents()
    {
        return GetComponents(HttpClient);
    }

    internal static Task<string> GetComponents(
        HttpClient httpClient
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Components/GetComponents.graphql")
        );
    }

    protected Task<string> GetComponent(
        string uuid
    )
    {
        return GetComponent(HttpClient, uuid);
    }

    internal static Task<string> GetComponent(
        HttpClient httpClient,
        string uuid
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Components/GetComponent.graphql"),
            variables: new Dictionary<string, object?>
            {
                ["uuid"] = uuid
            }
        );
    }

    protected Task<string> CreateComponent(
        CreateComponentInput input
    )
    {
        return CreateComponent(HttpClient, input);
    }

    internal static Task<string> CreateComponent(
        HttpClient httpClient,
        CreateComponentInput input
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Components/CreateComponent.graphql"),
            variables: input
        );
    }

    protected Task<JsonElement> CreateComponentAsJson(
        CreateComponentInput input
    )
    {
        return SuccessfullyQueryGraphQlContentAsJson(
            HttpClient,
            File.ReadAllText("Integration/GraphQl/Components/CreateComponent.graphql"),
            variables: input
        );
    }

    protected async Task<(string, string)> CreateComponentReturningIdAndUuid(
        CreateComponentInput input
    )
    {
        var response = await CreateComponentAsJson(input).ConfigureAwait(false);
        return (
            ExtractString(
                "$.data.createComponent.component.id",
                response
            ),
            ExtractString(
                "$.data.createComponent.component.uuid",
                response
            )
        );
    }
}