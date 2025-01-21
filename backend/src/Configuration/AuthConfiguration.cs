using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using Quartz;

namespace Metabase.Configuration;

public abstract class AuthConfiguration
{
    // `IdentityConstants.ApplicationScheme` is not a constant but only read-only. It can thus not
    // be used in the `Authorize` attribute. See the corresponding issue
    // https://github.com/dotnet/aspnetcore/issues/20122 and un-merged pull request https://github.com/dotnet/aspnetcore/pull/21343/files
    public const string IdentityConstantsApplicationScheme = "Identity.Application";

    public const string Audience = "metabase";
    public const string ReadPolicy = "Read";
    public const string WritePolicy = "Write";
    public const string ManageUserPolicy = "ManageUser";
    public const string ScopePrefixApi = "api";
    public const string ReadApiScope = ScopePrefixApi + ":read";
    public const string WriteApiScope = ScopePrefixApi + ":write";
    public const string ManageUserApiScope = ScopePrefixApi + ":user:manage";

    // Keep in sync with the scopes set in `OpenIddictClientRegistration`.
    private static readonly HashSet<string> s_clientScopes =
    [
        OpenIddictConstants.Scopes.Address,
        OpenIddictConstants.Scopes.Email,
        OpenIddictConstants.Scopes.Phone,
        OpenIddictConstants.Scopes.Profile,
        OpenIddictConstants.Scopes.Roles,
        ReadApiScope,
        WriteApiScope,
        ManageUserApiScope
    ];

    public static void ConfigureServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings
    )
    {
        var encryptionCertificate = LoadCertificate("jwt-encryption-certificate.pfx",
        appSettings.JsonWebToken.EncryptionCertificatePassword);
        var signingCertificate = LoadCertificate("jwt-signing-certificate.pfx",
        appSettings.JsonWebToken.SigningCertificatePassword);
        ConfigureIdentityServices(services);
        ConfigureAuthenticationAndAuthorizationServices(services);
        ConfigureTaskScheduling(services, environment);
        ConfigureOpenIddictServices(services, environment, appSettings, encryptionCertificate, signingCertificate);
    }

    private static X509Certificate2 LoadCertificate(
        string fileName,
        string password
    )
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException($"Empty password for certificate {fileName}.");
        }

        var stream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"Metabase.{fileName}")
            ?? throw new ArgumentException($"Missing certificate {fileName}.");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return X509CertificateLoader.LoadPkcs12(
            buffer.ToArray(),
            password,
            X509KeyStorageFlags.EphemeralKeySet
        );
    }

    private static void ConfigureIdentityServices(
        IServiceCollection services
    )
    {
        services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(600);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                // Configure Identity to use the same JWT claims as OpenIddict instead of the legacy
                // WS-Federation claims it uses by default (ClaimTypes), which saves you from doing
                // the mapping in your authorization controller.
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<ApplicationUserStore>()
            .AddDefaultTokenProviders(); // used to generate tokens for reset passwords, change email and change telephone number operations, and for two factor authentication token generation
        // The application cookies is used by the metabase acting as authentication server through
        // the authentication scheme `IdentityConstants.ApplicationScheme`, that is,
        // "Identity.Application". See also the constant `IdentityConstantsApplicationScheme`.
        services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/unauthorized";
                options.LoginPath = "/users/login";
                options.LogoutPath = "/me/logout";
                options.ReturnUrlParameter = "returnTo";
            }
        );
    }

    private static void ConfigureAuthenticationAndAuthorizationServices(
        IServiceCollection services
    )
    {
        // Dot not use the single authentication scheme as the default scheme https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-7.0#defaultscheme
        AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressAutoDefaultScheme", true);
        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/
        services.AddAuthentication(options =>
            {
                // To make the various authentication control flows obvious, do not use default
                // schemes for anything and always be explicit instead.
                options.DefaultAuthenticateScheme = null;
                options.DefaultChallengeScheme = null;
                options.DefaultForbidScheme = null;
                options.DefaultScheme = null;
                options.DefaultSignInScheme = null;
                options.DefaultSignOutScheme = null;
            })
            // The cookie is used by the metabase acting as its own client application through the
            // authentication scheme `CookieAuthenticationDefaults.AuthenticationScheme`, that is, "Cookies".
            .AddCookie(options =>
            {
                options.AccessDeniedPath = "/unauthorized";
                options.LoginPath = "/connect/client/login";
                options.LogoutPath = "/connect/client/logout";
                options.ReturnUrlParameter = "returnTo";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(600);
                options.SlidingExpiration = true;
                options.Events.OnValidatePrincipal = context =>
                {
                    if (context?.Principal is not null)
                    {
                        var identity = new ClaimsIdentity();
                        // The metabase frontend uses the "Cookies" scheme for user authentication
                        // and is allowed to show user data for all standard scopes. The
                        // corresponding authorization logic in `UserType` uses
                        // `ClaimsPrincipal.HasScope`. And it is also allowed to read data, write
                        // data, and manage users. The corresponding policies `*Policy` use scopes,
                        // so we need to add them.
                        identity.SetScopes(s_clientScopes);
                        context.Principal.AddIdentity(identity);
                    }

                    return Task.CompletedTask;
                };
            });
        services.AddAuthorization(options =>
            {
                foreach (var (policyName, scope) in new[]
                         {
                             (ReadPolicy, ReadApiScope),
                             (WritePolicy, WriteApiScope),
                             (ManageUserPolicy, ManageUserApiScope)
                         }
                        )
                {
                    options.AddPolicy(policyName, policy =>
                        {
                            policy.AuthenticationSchemes =
                            [
                                OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
                            ];
                            policy.RequireAuthenticatedUser();
                            policy.RequireAssertion(context =>
                                {
                                    // How the `HttpContext` can be accessed when the policies are
                                    // used in GraphQL queries or mutations. if (context.Resource is
                                    // IResolverContext resolverContext) { if
                                    // (resolverContext.ContextData.ContainsKey(nameof(HttpContext)))
                                    // { if (resolverContext.ContextData[nameof(HttpContext)] is
                                    // HttpContext httpContext) { if
                                    // (httpContext.Request.Headers.ContainsKey("Sec-Fetch-Site") &&
                                    // httpContext.Request.Headers.ContainsKey("Origin") ) { // Note
                                    // that CORS cannot serve as a security mechanism. Secure access
                                    // from the frontend by some other means. return
                                    // httpContext.Request.Headers["Sec-Fetch-Site"] ==
                                    // "same-origin" && httpContext.Request.Host ==
                                    // httpContext.Request.Headers["Origin"]; // Comparison does not
                                    // work because one includes the protocol HTTPS while the other
                                    // does not. } } } }
                                    return context.User.HasScope(scope);
                                }
                            );
                        }
                    );
                }
            }
        );
    }

    private static void ConfigureTaskScheduling(
        IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks (like
        // pruning orphaned authorizations/tokens from the database) at regular intervals. For
        // configuring Quartz see https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/hosted-services-integration.html
        services.AddQuartz(options =>
        {
            options.SchedulerId = "metabase";
            options.SchedulerName = "Metabase";
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
            options.UseDefaultThreadPool(_ =>
                _.MaxConcurrency = 10
            );
            if (environment.IsEnvironment(Program.TestEnvironment))
            {
                // See https://gitter.im/MassTransit/MassTransit?at=5db2d058f6db7f4f856fb404
                options.SchedulerName = Guid.NewGuid().ToString();
            }
        });
        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(_ =>
            _.WaitForJobsToComplete = true
        );
    }

    private static void ConfigureOpenIddictServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings,
        X509Certificate2 encryptionCertificate,
        X509Certificate2 signingCertificate
    )
    {
        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>()
                        .ReplaceDefaultEntities<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, Guid>();
                    // Enable Quartz.NET integration.
                    options.UseQuartz();
                }
            )
            // Register the OpenIddict server components.
            .AddServer(options =>
                {
                    options.SetIssuer(new Uri(appSettings.Host, UriKind.Absolute));
                    options.SetAuthorizationEndpointUris("connect/authorize")
                        .SetDeviceAuthorizationEndpointUris("connect/device")
                        .SetEndSessionEndpointUris("connect/logout")
                        .SetIntrospectionEndpointUris("connect/introspect")
                        // .SetRevocationEndpointUris("")
                        // .SetJSONWebKeySetEndpointUris("")
                        // .SetConfigurationEndpointUris("")
                        .SetTokenEndpointUris("connect/token")
                        .SetUserInfoEndpointUris("connect/userinfo")
                        .SetEndUserVerificationEndpointUris("connect/verify");
                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.Address,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Phone,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Roles,
                        ReadApiScope,
                        WriteApiScope,
                        ManageUserApiScope
                    );
                    options.AllowAuthorizationCodeFlow()
                        .AllowDeviceAuthorizationFlow()
                        .AllowRefreshTokenFlow();
                    // .AllowHybridFlow()
                    if (environment.IsEnvironment(Program.TestEnvironment))
                    {
                        options.AllowPasswordFlow();
                    }
                    // Register the signing and encryption credentials. See
                    // https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios
                    // and https://stackoverflow.com/questions/50862755/signing-keys-certificates-and-client-secrets-confusion/50932120#50932120
                    options.AddEncryptionCertificate(encryptionCertificate)
                        .AddSigningCertificate(signingCertificate);
                    // Force client applications to use Proof Key for Code Exchange (PKCE): https://documentation.openiddict.com/configuration/proof-key-for-code-exchange.html#enabling-pkce-enforcement-at-the-global-level
                    options.RequireProofKeyForCodeExchange();
                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    var builder = options.UseAspNetCore()
                        .EnableStatusCodePagesIntegration()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough()
                        .EnableEndUserVerificationEndpointPassthrough();
                    // .EnableStatusCodePagesIntegration();
                    if (environment.IsEnvironment(Program.TestEnvironment))
                    {
                        builder.DisableTransportSecurityRequirement();
                    }
                    // _.UseDataProtection();
                    // Note: if you don't want to specify a client_id when sending a token or
                    // revocation request, uncomment the following line: _.AcceptAnonymousClients();
                    // Note: if you want to process authorization and token requests that specify
                    // non-registered scopes, uncomment the following line: _.DisableScopeValidation();
                    // Note: if you don't want to use permissions, you can disable permission
                    // enforcement by uncommenting the following lines:
                    // _.IgnoreEndpointPermissions() .IgnoreGrantTypePermissions()
                    // .IgnoreResponseTypePermissions() .IgnoreScopePermissions();
                    // Note: when issuing access tokens used by third-party APIs you don't own, you
                    // can disable access token encryption: _.DisableAccessTokenEncryption();
                    // Note: To decrypt a JWT token, like an application code or an access token,
                    // you can execute ``` var jwt =
                    // "eyJhbGciOiJBMjU2S1ciLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwidHlwIjoiYXQrand0In0.tn0jqCR5-01c_SDRwex6sPBNl1Vl1KSRA5Zs2UoXri9F4tG4v9B6qbLmxzsfkd0rLe55BozRV8VCChsZAt_mCZfoVGOYewwP.ogGZQ5Q2p7Yue7D6zPJlSQ.uNX1Qs9R30vZxAPj-LuJSwWnisoKHQ5qNC_K9WvA0JDSCS_orw6TsboSVCe5b_aUg3JvrkJX_Ir0c-bFMf0HVhOVNy1UJwxo9zegJOsm7MybPzK58H4ubt_PRnrSueLgnQX8aDcjbM38Imy3RN6a3r3aKawdWGcyA23sIH8XnVmGk0lDB_PqFrFE7x2MmG4fyVJINoki441UI-7x0sLFUi4o98Z-2vTFuCd9cLRY5LAeb0ZIuWwOI7dv2Q54w7uV765kHS3VIPtupzSSXgQmfPBJOzDeV_-sCZGwUuC0jL8x1vJw573fejPwpPmXj3EKgzXgbGfAHmjoIzkNvcfr--Dy7O8WxLjSERoBgW45Tq0xcCLL9Vx4JGWehOw1jY-KaKIzfjUW9CTSwLgWdhqonetAULZFRJAYOEJ8PtP49jXlvkdjmavRXaX2UD8FD5TUx30TkGQ_xyuo6HvokURQqIYILZiL6R_kVqh7kwUnj4dgBqni-56M1GYFuX2UzynagL6c6t4OlO7RhQJpgz1pzmGNdEb5nvZplXru8KVEt_e9bQOy54EWudAemEVAyX-4P6FdWWazU5vMaRnDg53Y8gy4rt6LCwXK9WHNRbhkdAmTNNsfiuUd588lzMJVSVPeqzbl70yW8IH7hbwfscLjSf2-gP9evwDKoLMKjqyBtbsqGx-qtBE47y_-LKyP3TOJug0SBtiHIMb3xzN2rJaykAMRRz4kMaMF4_TZRV8kqZQqzF_xBoMJQD3nLYaN-G0qJYIse1JhVu4yHuH7vSvXdHA0x5dnBwwq4P3g35W5zv-cw4-b1cXnLq7TYUvHNVe6DcMAJogxW5ovA7wtEjF4yKxSbvlRaO065Jed5siLrIL17RahoHqew34kMzqXL0OUHvxId0A7myvLFy0YqhLnKg.1jfT6-IVamiQbS84hfX4lLtmoTiAmf0Ea0rpLPgYEek";
                    // var handler = new JwtSecurityTokenHandler(); var claimsPrincipal =
                    // handler.ValidateToken( jwt, new TokenValidationParameters { IssuerSigningKey
                    // = signingKey, TokenDecryptionKey = encryptionKey, // ValidIssuer =
                    // environment.IsEnvironment(Program.TestEnvironment) ? "http://localhost/" :
                    // appSettings.Host, ValidateActor = false, ValidateAudience = false,
                    // ValidateIssuer = false, ValidateIssuerSigningKey = false, ValidateLifetime =
                    // false, ValidateTokenReplay = false, }, out var validatedToken );
                    // Console.WriteLine(validatedToken.ToString()); ``` which as of this writing
                    // outputs ```
                    // {"alg":"A256KW","enc":"A256CBC-HS512","typ":"at+jwt"}.{"sub":"075561fa-98c0-40db-ad3d-9dc8abf240fd","name":"sw@ise.de","http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress":"sw@ise.de","oi_prst":"metabase","iss":"https://localhost:4041/","oi_au_id":"5dc00347-30fa-4ddb-91c1-471c505e7842","client_id":"metabase","oi_tkn_id":"e7430a26-4e13-4c21-8e6d-0f07dca66ff6","aud":"metabase","scope":"openid
                    // email profile roles api:read api:write
                    // offline_access","exp":1615405869,"iat":1615402269} ```
                }
            )
            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                options.SetIssuer(new Uri(appSettings.Host, UriKind.Absolute));
                // Configure the audience accepted by this resource server.
                options.AddAudiences(Audience);
                // Import the configuration from the local OpenIddict server instance:
                // https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#using-the-optionsuselocalserver-integration
                // Alternatively, OpenId Connect discovery can be used: https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#using-openid-connect-discovery-asymmetric-signing-keys-only
                options.UseLocalServer();
                // Register the ASP.NET Core host.
                options.UseAspNetCore();
                // Enable token entry validation: https://documentation.openiddict.com/configuration/token-storage.html#enabling-token-entry-validation-at-the-api-level
                options.EnableTokenEntryValidation();
                // Enable authorization entry validation: https://documentation.openiddict.com/configuration/authorization-storage.html#enabling-authorization-entry-validation-at-the-api-level
                options.EnableAuthorizationEntryValidation();
                // Note: the validation handler uses OpenID Connect discovery
                // to retrieve the address of the introspection endpoint.
                //options.SetIssuer("http://localhost:12345/");
                // Configure the validation handler to use introspection and register the client
                // credentials used when communicating with the remote introspection endpoint.
                //options.UseIntrospection()
                //       .SetClientId("resource_server_1")
                //       .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");
                // Register the System.Net.Http integration.
                //options.UseSystemNetHttp();
            })
            .AddClient(options =>
            {
                options.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials. See https://stackoverflow.com/questions/50862755/signing-keys-certificates-and-client-secrets-confusion/50932120#50932120
                options.AddEncryptionCertificate(encryptionCertificate)
                    .AddSigningCertificate(signingCertificate);

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                    .EnableStatusCodePagesIntegration()
                    .EnableRedirectionEndpointPassthrough()
                    .EnablePostLogoutRedirectionEndpointPassthrough();
                // .DisableTransportSecurityRequirement();

                // Register the System.Net.Http integration and use the identity of the current
                // assembly as a more specific user agent, which can be useful when dealing with
                // providers that use the user agent as a way to throttle requests (e.g Reddit).
                options.UseSystemNetHttp()
                    .SetProductInformation(typeof(Startup).Assembly);

                // Add a client registration matching the client application definition in the
                // server project.
                options.AddRegistration(
                    new OpenIddictClientRegistration
                    {
                        Issuer = new Uri(appSettings.Host, UriKind.Absolute),

                        // Note: these settings must match the application details inserted in the
                        // database at the server level.
                        ClientId = DbSeeder.MetabaseClientId,
                        ClientSecret = appSettings.OpenIdConnectClientSecret,

                        // https://auth0.com/docs/get-started/apis/scopes/openid-connect-scopes#standard-claims
                        Scopes =
                        {
                            // Keep in sync with `s_clientScopes`.
                            OpenIddictConstants.Scopes.Address,
                            OpenIddictConstants.Scopes.Email,
                            OpenIddictConstants.Scopes.Phone,
                            OpenIddictConstants.Scopes.Profile,
                            OpenIddictConstants.Scopes.Roles,
                            ReadApiScope,
                            WriteApiScope,
                            ManageUserApiScope
                        },

                        // Note: to mitigate mix-up attacks, it's recommended to use a unique
                        // redirection endpoint URI per provider, unless all the registered
                        // providers support returning a special "iss" parameter containing their
                        // URL as part of authorization responses. For more information, see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                        RedirectUri = new Uri("connect/callback/login/metabase", UriKind.Relative),
                        PostLogoutRedirectUri = new Uri("connect/callback/logout/metabase", UriKind.Relative)
                    }
                );
            });
    }
}