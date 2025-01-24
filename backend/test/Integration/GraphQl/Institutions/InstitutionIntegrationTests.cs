using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Institutions;

namespace Metabase.Tests.Integration.GraphQl.Institutions;

public abstract class InstitutionIntegrationTests
    : IntegrationTests
{
    internal static CreateInstitutionInput PendingInstitutionInput { get; } = new(
        "Institution A",
        "I!A",
        "Best institution ever!",
        new Uri("https://institution-a.com", UriKind.Absolute),
        null,
        [],
        null
    );

    internal static IEnumerable<CreateInstitutionInput> InstitutionInputs
    {
        get { yield return PendingInstitutionInput; }
    }

    internal static IEnumerable<object[]> EnumerateInstitutionInputs()
    {
        yield return new object[] { nameof(PendingInstitutionInput), PendingInstitutionInput };
    }

    protected Task<string> GetInstitutions()
    {
        return GetInstitutions(HttpClient);
    }

    internal static Task<string> GetInstitutions(
        HttpClient httpClient
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Institutions/GetInstitutions.graphql")
        );
    }

    protected Task<string> GetPendingInstitutions()
    {
        return GetPendingInstitutions(HttpClient);
    }

    internal static Task<string> GetPendingInstitutions(
        HttpClient httpClient
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Institutions/GetPendingInstitutions.graphql")
        );
    }

    protected Task<string> GetInstitution(
        string uuid
    )
    {
        return GetInstitution(HttpClient, uuid);
    }

    internal static Task<string> GetInstitution(
        HttpClient httpClient,
        string uuid
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Institutions/GetInstitution.graphql"),
            variables: new Dictionary<string, object?>
            {
                ["uuid"] = uuid
            }
        );
    }

    protected Task<string> CreateInstitution(
        CreateInstitutionInput input
    )
    {
        return CreateInstitution(HttpClient, input);
    }

    internal static Task<string> CreateInstitution(
        HttpClient httpClient,
        CreateInstitutionInput input
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Institutions/CreateInstitution.graphql"),
            variables: input
        );
    }

    protected Task<JsonElement> CreateInstitutionAsJson(
        CreateInstitutionInput input
    )
    {
        return CreateInstitutionAsJson(HttpClient, input);
    }

    internal static Task<JsonElement> CreateInstitutionAsJson(
        HttpClient httpClient,
        CreateInstitutionInput input
    )
    {
        return SuccessfullyQueryGraphQlContentAsJson(
            httpClient,
            File.ReadAllText("Integration/GraphQl/Institutions/CreateInstitution.graphql"),
            variables: input
        );
    }

    protected Task<Guid> CreateInstitutionReturningUuid(
        CreateInstitutionInput input
    )
    {
        return CreateInstitutionReturningUuid(HttpClient, input);
    }

    internal static async Task<Guid> CreateInstitutionReturningUuid(
        HttpClient httpClient,
        CreateInstitutionInput input
    )
    {
        var response = await CreateInstitutionAsJson(httpClient, input).ConfigureAwait(false);
        return new Guid(
            ExtractString(
                "$.data.createInstitution.institution.uuid",
                response
            )
        );
    }

    internal static async Task<Guid> CreateAndVerifyInstitutionReturningUuid(
        HttpClient httpClient,
        string verifierPassword,
        CreateInstitutionInput input
    )
    {
        var uuid = await CreateInstitutionReturningUuid(httpClient, input).ConfigureAwait(false);
        await VerifyInstitutionByVerifierUser(httpClient, verifierPassword, uuid).ConfigureAwait(false);
        return uuid;
    }

    protected async Task<(string, string)> CreateInstitutionReturningIdAndUuid(
        CreateInstitutionInput input
    )
    {
        var response = await CreateInstitutionAsJson(input).ConfigureAwait(false);
        return (
            ExtractString(
                "$.data.createInstitution.institution.id",
                response
            ),
            ExtractString(
                "$.data.createInstitution.institution.uuid",
                response
            )
        );
    }

    internal static Task<string> VerifyInstitutionByVerifierUser(
        HttpClient httpClient,
        string verifierPassword,
        Guid institutionId
    )
    {
        return AsUser(
            httpClient,
            DbSeeder.VerifierUser.EmailAddress,
            verifierPassword,
            httpClient =>
            {
                return SuccessfullyQueryGraphQlContentAsString(
                    httpClient,
                    File.ReadAllText("Integration/GraphQl/Institutions/VerifyInstitution.graphql"),
                    variables: new Dictionary<string, object?>
                    {
                        ["institutionId"] = institutionId
                    }
                );
            }
        );
    }
}