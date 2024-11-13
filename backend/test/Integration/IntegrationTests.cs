using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Json.Path;
using NUnit.Framework;
using Snapshooter;
using TokenResponse = IdentityModel.Client.TokenResponse;
using WebApplicationFactoryClientOptions = Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions;

namespace Metabase.Tests.Integration;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract partial class IntegrationTests
    : IDisposable
{
    public const string DefaultName = "John Doe";
    public const string DefaultEmail = "john.doe@ise.fraunhofer.de";
    public const string DefaultPassword = "aaaAAA123$!@";

    private static readonly JsonSerializerOptions _customJsonSerializerOptions =
        new()
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    private bool _disposed;

    protected IntegrationTests()
    {
        Factory = new CustomWebApplicationFactory();
        HttpClient = CreateHttpClient();
    }

    private CustomWebApplicationFactory Factory { get; }

    protected CollectingEmailSender EmailSender => Factory.EmailSender;

    protected AppSettings AppSettings => Factory.AppSettings;

    protected HttpClient HttpClient { get; }

    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    [GeneratedRegex("confirmationCode=(?<confirmationCode>\\w+)")]
    private static partial Regex ConfirmationCodeRegex();

    [GeneratedRegex("resetCode=(?<resetCode>\\w+)")]
    private static partial Regex ResetCodeRegex();

    // https://docs.microsoft.com/en-us/dotnet/standard/managed-code
    // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    ~IntegrationTests()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Factory.Dispose();
                HttpClient.Dispose();
            }

            _disposed = true;
        }
    }

    private static IEnumerable<Cookie> ExtractCookies(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookieEntries))
        {
            return Enumerable.Empty<Cookie>();
        }
        var uri = response.RequestMessage?.RequestUri ?? throw new ArgumentException($"The request URI cannot be extracted from the given response {response}.");
        var cookieContainer = new CookieContainer();
        foreach (var cookieEntry in cookieEntries)
        {
            cookieContainer.SetCookies(uri, cookieEntry);
        }
        return cookieContainer.GetCookies(uri).Cast<Cookie>();
    }

    private static async Task UpdateAntiforgeryCookieAndToken(
        HttpClient httpClient
    )
    {
        // Get the antiforgery token in the cookie "XSRF-TOKEN" and set it
        // permanently on the HTTP client by requesting /antiforgery/token
        // synchronously.
        var response = await httpClient.GetAsync("/antiforgery/token").ConfigureAwait(false);
        // Add the antiforgery token as the default request header "X-XSRF-TOKEN".
        var xsrfToken =
            ExtractCookies(response)
            .SingleOrDefault(cookie => cookie.Name == "XSRF-TOKEN")
            ?.Value
            ?? throw new ArgumentException("The `XSRF-TOKEN` cookie is missing in the response of a request to /antiforgery/token");
        httpClient.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");
        httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", xsrfToken);
    }

    protected static HttpClient CreateHttpClient(
        CustomWebApplicationFactory factory,
        bool allowAutoRedirect = true
    )
    {
        var httpClient = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = allowAutoRedirect,
                BaseAddress = new Uri("http://localhost", UriKind.Absolute),
                HandleCookies = true,
                MaxAutomaticRedirections = 3
            }
        );
        Task.Run(() => UpdateAntiforgeryCookieAndToken(httpClient)).Wait();
        return httpClient;
    }

    protected HttpClient CreateHttpClient(
        bool allowAutoRedirect = true
    )
    {
        return CreateHttpClient(
            Factory,
            allowAutoRedirect
        );
    }

    protected static async Task<TokenResponse> RequestAuthToken(
        HttpClient httpClient,
        string emailAddress,
        string password
    )
    {
        var response =
            await httpClient.RequestPasswordTokenAsync(
                    new PasswordTokenRequest
                    {
                        Address = "http://localhost/connect/token",
                        ClientId = "metabase",
                        ClientSecret = "secret",
                        Scope = "address email phone profile roles api:read api:write api:user:manage",
                        UserName = emailAddress,
                        Password = password
                    }
                )
                .ConfigureAwait(false);
        if (response.IsError)
        {
            throw new HttpRequestException($"Error {response.Error} of type {response.ErrorType} with description {response.ErrorDescription}");
        }

        return response;
    }

    protected Task<TokenResponse> RequestAuthToken(
        string emailAddress,
        string password
    )
    {
        return RequestAuthToken(
            HttpClient,
            emailAddress,
            password
        );
    }

    protected static async Task LoginUser(
        HttpClient httpClient,
        string email = DefaultEmail,
        string password = DefaultPassword
    )
    {
        var tokenResponse =
            await RequestAuthToken(
                    httpClient,
                    email,
                    password
                )
                .ConfigureAwait(false);
        httpClient.SetBearerToken(tokenResponse.AccessToken ??
                                  throw new InvalidOperationException(
                                      $"The auth-token request to {httpClient.BaseAddress} with email address {email} and password {password} returned `null` as access token."));
        await UpdateAntiforgeryCookieAndToken(httpClient).ConfigureAwait(false);
    }

    protected Task LoginUser(
        string email = DefaultEmail,
        string password = DefaultPassword
    )
    {
        return LoginUser(
            HttpClient,
            email,
            password
        );
    }

    protected static async Task LogoutUser(
        HttpClient httpClient
    )
    {
        httpClient.SetBearerToken("");
        await UpdateAntiforgeryCookieAndToken(httpClient).ConfigureAwait(false);
    }

    protected Task LogoutUser()
    {
        return LogoutUser(HttpClient);
    }

    protected static async Task<TResult> AsUser<TResult>(
        HttpClient httpClient,
        string email,
        string password,
        Func<HttpClient, Task<TResult>> task
    )
    {
        // This is fragile as it uses the fact that at the moment of this
        // writing, `LoginUser` calls `SetBearerToken` which sets
        // `httpClient.DefaultRequestHeaders.Authorization`. Thus, by
        // remembering and restoring this value the original user is kept
        // logged-in. For details see
        // https://github.com/IdentityModel/IdentityModel/blob/main/src/Client/Extensions/AuthorizationHeaderExtensions.cs
        var originalAuthorizationRequestHeader = httpClient.DefaultRequestHeaders.Authorization;
        try
        {
            await LoginUser(
                httpClient,
                email,
                password
            ).ConfigureAwait(false);
            var result = await task(httpClient).ConfigureAwait(false);
            await LogoutUser(httpClient).ConfigureAwait(false);
            return result;
        }
        finally
        {
            httpClient.DefaultRequestHeaders.Authorization = originalAuthorizationRequestHeader;
        }
    }

    protected async Task<string> RegisterUser(
        string name = DefaultName,
        string email = DefaultEmail,
        string password = DefaultPassword,
        string? passwordConfirmation = null
    )
    {
        return await SuccessfullyQueryGraphQlContentAsString(
            File.ReadAllText("Integration/GraphQl/Users/RegisterUser.graphql"),
            variables: new Dictionary<string, object?>
            {
                ["name"] = name,
                ["email"] = email,
                ["password"] = password,
                ["passwordConfirmation"] = passwordConfirmation ?? password
            }
        ).ConfigureAwait(false);
    }

    protected async Task<Guid> RegisterUserReturningUuid(
        string name = DefaultName,
        string email = DefaultEmail,
        string password = DefaultPassword,
        string? passwordConfirmation = null
    )
    {
        var response = await SuccessfullyQueryGraphQlContentAsJson(
            File.ReadAllText("Integration/GraphQl/Users/RegisterUser.graphql"),
            variables: new Dictionary<string, object?>
            {
                ["name"] = name,
                ["email"] = email,
                ["password"] = password,
                ["passwordConfirmation"] = passwordConfirmation ?? password
            }
        ).ConfigureAwait(false);
        return new Guid(
            ExtractString(
                "$.data.registerUser.user.uuid",
                response
            )
        );
    }

    protected string ExtractConfirmationCodeFromEmail()
    {
        return ConfirmationCodeRegex()
            .Match(EmailSender.Emails.Single().Body)
            .Groups["confirmationCode"]
            .Captures
            .Single()
            .Value;
    }

    protected string ExtractResetCodeFromEmail()
    {
        return ResetCodeRegex()
            .Match(EmailSender.Emails.Single().Body)
            .Groups["resetCode"]
            .Captures
            .Single()
            .Value;
    }

    protected async Task<string> ConfirmUserEmail(
        string confirmationCode,
        string email = DefaultEmail
    )
    {
        return await SuccessfullyQueryGraphQlContentAsString(
            File.ReadAllText("Integration/GraphQl/Users/ConfirmUserEmail.graphql"),
            variables: new Dictionary<string, object?>
            {
                ["email"] = email,
                ["confirmationCode"] = confirmationCode
            }
        ).ConfigureAwait(false);
    }

    protected async Task<Guid> RegisterAndConfirmUser(
        string name = DefaultName,
        string email = DefaultEmail,
        string password = DefaultPassword
    )
    {
        var uuid =
            await RegisterUserReturningUuid(
                name,
                email,
                password
            ).ConfigureAwait(false);
        var confirmationCode = ExtractConfirmationCodeFromEmail();
        await ConfirmUserEmail(
            confirmationCode,
            email
        ).ConfigureAwait(false);
        return uuid;
    }

    protected async Task<Guid> RegisterAndConfirmAndLoginUser(
        string name = DefaultName,
        string email = DefaultEmail,
        string password = DefaultPassword
    )
    {
        var uuid =
            await RegisterAndConfirmUser(
                name,
                email,
                password
            ).ConfigureAwait(false);
        await LoginUser(
            email,
            password
        ).ConfigureAwait(false);
        return uuid;
    }

    protected Task<HttpResponseMessage> QueryGraphQl(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return QueryGraphQl(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static Task<HttpResponseMessage> QueryGraphQl(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return httpClient.PostAsync(
            "/graphql",
            MakeJsonHttpContent(
                new GraphQlRequest(
                    query,
                    operationName,
                    variables
                )
            )
        );
    }

    protected Task<HttpContent> SuccessfullyQueryGraphQlContent(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return SuccessfullyQueryGraphQlContent(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<HttpContent> SuccessfullyQueryGraphQlContent(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        var httpResponseMessage = await QueryGraphQl(
            httpClient,
            query,
            operationName,
            variables
        ).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
        {
            // We wrap this check in an if-condition such that the message
            // content is only read when the status code is not 200.
            httpResponseMessage.StatusCode.Should().Be(
                HttpStatusCode.OK,
                await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)
            );
        }

        return httpResponseMessage.Content;
    }

    protected Task<HttpContent> UnsuccessfullyQueryGraphQlContent(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return UnsuccessfullyQueryGraphQlContent(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<HttpContent> UnsuccessfullyQueryGraphQlContent(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        var httpResponseMessage = await QueryGraphQl(
            httpClient,
            query,
            operationName,
            variables
        ).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            // We wrap this check in an if-condition such that the message
            // content is only read when the status code is not 200.
            httpResponseMessage.StatusCode.Should().NotBe(
                HttpStatusCode.OK,
                await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)
            );
        }

        return httpResponseMessage.Content;
    }

    protected Task<string> SuccessfullyQueryGraphQlContentAsString(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<string> SuccessfullyQueryGraphQlContentAsString(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return await (
                await SuccessfullyQueryGraphQlContent(
                        httpClient,
                        query,
                        operationName,
                        variables
                    )
                    .ConfigureAwait(false)
            )
            .ReadAsStringAsync()
            .ConfigureAwait(false);
    }

    protected Task<JsonElement> SuccessfullyQueryGraphQlContentAsJson(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return SuccessfullyQueryGraphQlContentAsJson(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<JsonElement> SuccessfullyQueryGraphQlContentAsJson(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return (
                await JsonDocument.ParseAsync(
                        await (
                                await SuccessfullyQueryGraphQlContent(
                                        httpClient,
                                        query,
                                        operationName,
                                        variables
                                    )
                                    .ConfigureAwait(false)
                            )
                            .ReadAsStreamAsync()
                            .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false)
            )
            .RootElement;
    }

    protected Task<string> UnsuccessfullyQueryGraphQlContentAsString(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return UnsuccessfullyQueryGraphQlContentAsString(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<string> UnsuccessfullyQueryGraphQlContentAsString(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return await (
                await UnsuccessfullyQueryGraphQlContent(
                        httpClient,
                        query,
                        operationName,
                        variables
                    )
                    .ConfigureAwait(false)
            )
            .ReadAsStringAsync()
            .ConfigureAwait(false);
    }

    protected static string ExtractString(
        string jsonPath,
        JsonElement jsonElement
    )
    {
        var pathResult =
            JsonPath.Parse(jsonPath).Evaluate(
                JsonObject.Create(jsonElement)
            );

        return pathResult.Matches?.Single()?.Value?.GetValue<string>()
               ?? throw new ArgumentException("String is null");
    }

    protected static string Base64Encode(string text)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(text)
        );
    }

    protected static string Base64Decode(string text)
    {
        return Encoding.UTF8.GetString(
            Convert.FromBase64String(text)
        );
    }

    protected void EmailsShouldContainSingle(
        (string name, string address) recipient,
        string subject,
        string bodyRegEx
    )
    {
        EmailSender.Emails.Should().ContainSingle();
        var email = EmailSender.Emails.First();
        email.Recipient.Should().Be(recipient);
        email.Subject.Should().Be(subject);
        email.Body.Should().MatchRegex(bodyRegEx);
    }

    private static ByteArrayContent MakeJsonHttpContent<TContent>(
        TContent content
    )
    {
        var result =
            new ByteArrayContent(
                JsonSerializer.SerializeToUtf8Bytes(
                    content,
                    _customJsonSerializerOptions
                )
            );
        result.Headers.ContentType =
            new MediaTypeHeaderValue("application/json");
        return result;
    }

    // With NUnit using async Snapshooter is not able to calculate
    // the necessary Fullname, due to reasons mentioned in
    // https://stackoverflow.com/questions/22598323/movenext-instead-of-actual-method-task-name
    // The workaround with optional parameters is inspired by the same source.
    protected static SnapshotFullName SnapshotFullNameHelper(
        Type testType,
        string keyName,
        [CallerMemberName] string testMethod = "",
        [CallerFilePath] string testFilePath = ""
    )
    {
        var testName = $"{testType.Name}.{testMethod}_{keyName}.snap";
        var testDirectory =
            Path.GetDirectoryName(testFilePath)
            ?? throw new ArgumentException($"The path '{testFilePath}' denotes a root directory or is `null`.");
        return new SnapshotFullName(testName, testDirectory);
    }

    private sealed class GraphQlRequest(
        string query,
        string? operationName,
        object? variables
        )
    {
        public string Query { get; } = query;
        public string? OperationName { get; } = operationName;
        public object? Variables { get; } = variables;
    }
}