using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Serializer.SystemTextJson;
using IdentityModel;
using IdentityModel.Client;
using Metabase.Data;
using Metabase.GraphQl.DataX;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenIddict.Client.AspNetCore;

namespace Metabase.GraphQl.Databases;

public sealed class QueryingDatabases
{
    public const string DatabaseHttpClient = "Database";

    // Inspired by https://learn.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support#use-datetimeoffsetparse-as-a-fallback
    private sealed class DateTimeConverterUsingDateTimeParseAsFallback : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            if (!reader.TryGetDateTime(out DateTime value))
            {
                value = DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture);
            }
            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // For information on the format specifier `o`, see
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
            writer.WriteStringValue(value.ToString("o", CultureInfo.InvariantCulture));
        }
    }

    public static readonly JsonSerializerOptions SerializerOptions =
        new()
        {
            Converters =
            {
                new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false),
                new DateTimeConverterUsingDateTimeParseAsFallback()
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            NumberHandling = JsonNumberHandling.Strict,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            // RespectNullableAnnotations = true,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            WriteIndented = false
        }; //.SetupImmutableConverter();

    public static async Task<string> ConstructQuery(
        string[] fileNames
    )
    {
        return string.Join(
            Environment.NewLine,
            await Task.WhenAll(
                fileNames.Select(fileName =>
                    File.ReadAllTextAsync($"GraphQl/Databases/Queries/{fileName}")
                )
            ).ConfigureAwait(false)
        );
    }

    private static async Task<string?> ExtractBearerToken(
        IHttpContextAccessor httpContextAccessor
    )
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        // Extract bearer token stored in cookie (used by Metabase Web
        // frontend)
        var cookieBearerToken = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken
        ).ConfigureAwait(false);
        if (cookieBearerToken is not null)
        {
            return cookieBearerToken;
        }

        // Extract bearer token given in authorization header (used by
        // third-party frontends)
        var bearerTokenPrefix = $"{OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer} ";
        return httpContextAccessor.HttpContext.Request?.Headers?.Authorization
            .FirstOrDefault(
                x => x is not null
                     && x.TrimStart().StartsWith(bearerTokenPrefix, StringComparison.Ordinal))
            ?.TrimStart()
            ?.Replace(bearerTokenPrefix, "");
    }

    public static async
        Task<GraphQLResponse<TGraphQlResponse>>
        QueryDatabase<TGraphQlResponse>(
            Database database,
            GraphQLRequest request,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            CancellationToken cancellationToken,
            string? apiToken = null
        )
        where TGraphQlResponse : class
    {
        // https://github.com/graphql-dotnet/graphql-client/blob/47b4abfbfda507a91b5c62a18a9789bd3a8079c7/src/GraphQL.Client/GraphQLHttpResponse.cs
        // var response =
        //   (
        //    await CreateGraphQlClient(database)
        //    .SendQueryAsync<TGraphQlResponse>(
        //      request,
        //      cancellationToken
        //      )
        //    .ConfigureAwait(false)
        //    )
        //   .AsGraphQLHttpResponse();
        using var httpClient = httpClientFactory.CreateClient(DatabaseHttpClient);
        // Set the authorization header to a given API token or the bearer token
        // from the original HTTP request. Note that we cannot pass the API
        // token as well as the bearer token in one request, neither with
        // multiple HTTP authorization headers nor with one header with
        // a comma-separated list of authentication scheme and value pairs.
        // Both go against RFC 7230/7235, even though some web servers accept
        // multiple schemes. For details see
        // https://stackoverflow.com/questions/29282578/multiple-http-authorization-headers
        if (apiToken is not null)
        {
            httpClient.SetToken("Token", apiToken);
        }
        else
        {
            // We extract and set the bearer token below. Alternatively, we could
            // add a named client to the factory and set the bearer token there as
            // detailed in
            // https://stackoverflow.com/questions/51358870/configure-httpclientfactory-to-use-data-from-the-current-request-context/51460160#51460160
            var bearerToken = await ExtractBearerToken(httpContextAccessor).ConfigureAwait(false);
            if (bearerToken is not null)
            {
                httpClient.SetBearerToken(bearerToken);
            }
        }

        // For some reason `httpClient.PostAsJsonAsync` without `MakeJsonHttpContent` but with `SerializerOptions` results in `BadRequest` status code. It has to do with `JsonContent.Create` used within `PostAsJsonAsync` --- we also cannot use `JsonContent.Create` in `MakeJsonHttpContent`. What is happening here?
        using var jsonHttpContent = MakeJsonHttpContent(request);
        using var httpResponseMessage =
            await httpClient.PostAsync(
                database.Locator,
                jsonHttpContent,
                cancellationToken
            ).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"The status code is not {HttpStatusCode.OK} but {httpResponseMessage.StatusCode}.", null,
                httpResponseMessage.StatusCode);
        }

        // We could use `httpResponseMessage.Content.ReadFromJsonAsync<GraphQL.GraphQLResponse<TGraphQlResponse>>` which would make debugging more difficult though, https://docs.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync?view=net-5.0#System_Net_Http_Json_HttpContentJsonExtensions_ReadFromJsonAsync__1_System_Net_Http_HttpContent_System_Text_Json_JsonSerializerOptions_System_Threading_CancellationToken_
        using var graphQlResponseStream =
            await httpResponseMessage.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
        // For debugging, the following lines of code write the response to standard output.
        // Console.WriteLine(new StreamReader(graphQlResponseStream).ReadToEnd());
        var deserializedGraphQlResponse =
            await JsonSerializer.DeserializeAsync<GraphQLResponse<TGraphQlResponse>>(
                graphQlResponseStream,
                SerializerOptions,
                cancellationToken
            ).ConfigureAwait(false) ?? throw new JsonException("Failed to deserialize the GraphQL response.");
        return deserializedGraphQlResponse;
    }

    // private GraphQLHttpClient CreateGraphQlClient(
    //     Data.Database database
    //     )
    // {
    //     return new GraphQLHttpClient(
    //         new GraphQLHttpClientOptions { EndPoint = database.Locator },
    //         new SystemTextJsonSerializer(SerializerOptions),
    //         _httpClientFactory.CreateClient()
    //         );
    // }

    private static ByteArrayContent MakeJsonHttpContent<TContent>(
        TContent content
    )
    {
        // For some reason using `JsonContent.Create<TContent>(content, null, SerializerOptions)` results in status code `BadRequest`.
        var result =
            new ByteArrayContent(
                JsonSerializer.SerializeToUtf8Bytes(
                    content,
                    SerializerOptions
                )
            );
        result.Headers.ContentType =
            new MediaTypeHeaderValue("application/json");
        return result;
    }
}