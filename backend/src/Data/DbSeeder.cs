using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Metabase.Configuration;
using Metabase.Enumerations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Metabase.Data;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Seeding the database")]
    public static partial void SeedingDatabase(
        this ILogger logger
    );

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Debug,
        Message = "Creating role {Role}")]
    public static partial void CreatingRole(
        this ILogger logger,
        Enumerations.UserRole role
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Debug,
        Message = "Creating user {Name}")]
    public static partial void CreatingUser(
        this ILogger logger,
        string name
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Debug,
        Message = "Creating application client '{ClientId}'")]
    public static partial void CreatingApplicationClient(
        this ILogger logger,
        string clientId
    );

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Debug,
        Message = "Creating scope '{Scope}'")]
    public static partial void CreatingScope(
        this ILogger logger,
        string scope
    );
}

public sealed class DbSeeder
{
    public const string MetabaseClientId = "metabase";
    public const string TestlabSolarFacadesClientId = "testlab-solar-facades";

    public static readonly ReadOnlyCollection<(string Name, string EmailAddress, Enumerations.UserRole Role)> Users =
        Role.AllEnum.Select(role => (
            Role.EnumToName(role),
            $"{Role.EnumToName(role).ToLowerInvariant()}@buildingenvelopedata.org",
            role
        )).ToList().AsReadOnly();

    public static readonly (string Name, string EmailAddress, Enumerations.UserRole Role)
        AdministratorUser =
            Users.First(x => x.Role == Enumerations.UserRole.ADMINISTRATOR);

    public static readonly (string Name, string EmailAddress, Enumerations.UserRole Role)
        VerifierUser =
            Users.First(x => x.Role == Enumerations.UserRole.VERIFIER);

    private const string IseInstitutionName = "Fraunhofer ISE";
    private const string TestlabInstitutionName = "TestLab Solar Facades";
    private const string LbnlInstitutionName = "LBNL";

    private const string TestlabDatabaseName = "TestLab DB";
    private const string IgsdbDatabaseName = "IGSDB";

    public static async Task DoAsync(
        IServiceProvider services
    )
    {
        var logger = services.GetRequiredService<ILogger<DbSeeder>>();
        logger.SeedingDatabase();
        var environment = services.GetRequiredService<IWebHostEnvironment>();
        var appSettings = services.GetRequiredService<AppSettings>();
        await CreateRolesAsync(services, logger).ConfigureAwait(false);
        await CreateUsersAsync(services, environment, appSettings, logger).ConfigureAwait(false);
        await RegisterApplicationsAsync(services, logger, environment, appSettings).ConfigureAwait(false);
        await RegisterScopesAsync(services, logger).ConfigureAwait(false);
        await CreateInstitutionsAsync(services, logger, environment).ConfigureAwait(false);
        await CreateDatabasesAsync(services, logger, environment, appSettings).ConfigureAwait(false);
    }

    private static async Task CreateRolesAsync(
        IServiceProvider services,
        ILogger<DbSeeder> logger
    )
    {
        var manager = services.GetRequiredService<RoleManager<Role>>();
        foreach (var role in Role.AllEnum)
            if (await manager.FindByNameAsync(Role.EnumToName(role)).ConfigureAwait(false) is null)
            {
                logger.CreatingRole(role);
                await manager.CreateAsync(
                    new Role(role)
                ).ConfigureAwait(false);
            }
    }

    private static async Task CreateUsersAsync(
        IServiceProvider services,
        IWebHostEnvironment environment,
        AppSettings appSettings,
        ILogger<DbSeeder> logger
    )
    {
        var manager = services.GetRequiredService<UserManager<User>>();
        if (environment.IsProduction())
        {
            if ((await manager.GetUsersInRoleAsync(Role.Administrator).ConfigureAwait(false)).Count == 0)
            {
                await CreateUserAsync(manager, AdministratorUser, appSettings.BootstrapUserPassword, logger).ConfigureAwait(false);
            }
        }
        else
        {
            foreach (var userInfo in Users)
                if (await manager.FindByEmailAsync(userInfo.EmailAddress).ConfigureAwait(false) is null)
                {
                    await CreateUserAsync(manager, userInfo, appSettings.BootstrapUserPassword, logger).ConfigureAwait(false);
                }
            if (await manager.FindByEmailAsync("admin@admin.de").ConfigureAwait(false) is null)
            {
                await CreateUserAsync(manager, ("admin", "admin@admin.de", Enumerations.UserRole.ADMINISTRATOR), "Admin2005!", logger).ConfigureAwait(false);
            }
        }
    }

    private static async Task CreateUserAsync(
        UserManager<User> manager,
        (string Name, string EmailAddress, Enumerations.UserRole Role) userInfo,
        string Password,
        ILogger<DbSeeder> logger
    )
    {
        logger.CreatingUser(userInfo.Name);
        var user = new User(userInfo.Name, userInfo.EmailAddress, null, null);
        await manager.CreateAsync(
            user,
            Password
        ).ConfigureAwait(false);
        var confirmationToken =
            await manager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
        await manager.ConfirmEmailAsync(user, confirmationToken).ConfigureAwait(false);
        await manager.AddToRoleAsync(user, Role.EnumToName(userInfo.Role)).ConfigureAwait(false);
    }

    private static async Task RegisterApplicationsAsync(
        IServiceProvider services,
        ILogger<DbSeeder> logger,
        IWebHostEnvironment environment,
        AppSettings appSettings
    )
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();
        if (await manager.FindByClientIdAsync(MetabaseClientId).ConfigureAwait(false) is null)
        {
            logger.CreatingApplicationClient(MetabaseClientId);
            var host = appSettings.Host;
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = MetabaseClientId,
                    // The secret is used in tests, see `IntegrationTests#RequestAuthToken` and in
                    // the metabase client, see `OPEN_ID_CONNECT_CLIENT_SECRET` in `.env.*`.
                    ClientSecret = appSettings.OpenIdConnectClientSecret,
                    ConsentType = environment.IsEnvironment(Program.TestEnvironment)
                        ? OpenIddictConstants.ConsentTypes.Systematic
                        : OpenIddictConstants.ConsentTypes.Explicit,
                    DisplayName = "Metabase client application",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("de-DE")] = "Metabase-Klient-Anwendung"
                    },
                    RedirectUris =
                    {
                        new Uri(environment.IsEnvironment(Program.TestEnvironment)
                            ? "urn:test"
                            : $"{host}/connect/callback/login/metabase",
                            UriKind.Absolute)
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri(environment.IsEnvironment(Program.TestEnvironment)
                            ? "urn:test"
                            : $"{host}/connect/callback/logout/metabase",
                            UriKind.Absolute)
                    },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        // OpenIddictConstants.Permissions.Endpoints.Device,
                        OpenIddictConstants.Permissions.Endpoints.Introspection,
                        OpenIddictConstants.Permissions.Endpoints.Logout,
                        OpenIddictConstants.Permissions.Endpoints.Revocation,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        environment.IsEnvironment(Program.TestEnvironment)
                            ? OpenIddictConstants.Permissions.GrantTypes.Password
                            : OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        // OpenIddictConstants.Permissions.GrantTypes.ClientCredentials, OpenIddictConstants.Permissions.GrantTypes.DeviceCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        environment.IsEnvironment(Program.TestEnvironment)
                            ? OpenIddictConstants.Permissions.ResponseTypes.Token
                            : OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Address,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Phone,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                        OpenIddictConstants.Permissions.Prefixes.Scope +
                        AuthConfiguration.ReadApiScope,
                        OpenIddictConstants.Permissions.Prefixes.Scope +
                        AuthConfiguration.WriteApiScope,
                        OpenIddictConstants.Permissions.Prefixes.Scope +
                        AuthConfiguration.ManageUserApiScope
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                    }
                }
            ).ConfigureAwait(false);
        }

        if (await manager.FindByClientIdAsync(TestlabSolarFacadesClientId).ConfigureAwait(false) is null)
        {
            logger.CreatingApplicationClient(TestlabSolarFacadesClientId);
            var host = appSettings.TestlabSolarFacadesHost;
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = TestlabSolarFacadesClientId,
                    // The secret is used in the database client, see
                    // `OPEN_ID_CONNECT_CLIENT_SECRET` in `.env.*`.
                    ClientSecret = appSettings.TestlabSolarFacadesOpenIdConnectClientSecret,
                    ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                    DisplayName = "Testlab-Solar-Facades client application",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("de-DE")] = "Testlab-Solar-Facades-Klient-Anwendung"
                    },
                    RedirectUris =
                    {
                        new Uri($"{host}/connect/callback/login/metabase", UriKind.Absolute)
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri($"{host}/connect/callback/logout/metabase", UriKind.Absolute)
                    },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        // OpenIddictConstants.Permissions.Endpoints.Device,
                        OpenIddictConstants.Permissions.Endpoints.Introspection,
                        OpenIddictConstants.Permissions.Endpoints.Logout,
                        OpenIddictConstants.Permissions.Endpoints.Revocation,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        // OpenIddictConstants.Permissions.GrantTypes.ClientCredentials, OpenIddictConstants.Permissions.GrantTypes.DeviceCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Address,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Phone,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                        OpenIddictConstants.Permissions.Prefixes.Scope +
                        AuthConfiguration.ReadApiScope,
                        OpenIddictConstants.Permissions.Prefixes.Scope +
                        AuthConfiguration.WriteApiScope
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                    }
                }
            ).ConfigureAwait(false);
        }
    }

    private static async Task RegisterScopesAsync(
        IServiceProvider services,
        ILogger<DbSeeder> logger
    )
    {
        var manager = services.GetRequiredService<IOpenIddictScopeManager>();
        if (await manager.FindByNameAsync(AuthConfiguration.ReadApiScope)
                .ConfigureAwait(false) is null)
        {
            logger.CreatingScope(AuthConfiguration.ReadApiScope);
            await manager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    DisplayName = "Read API access",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("de-DE")] = "API Lesezugriff"
                    },
                    Name = AuthConfiguration.ReadApiScope,
                    Resources =
                    {
                        AuthConfiguration.Audience
                    }
                }
            ).ConfigureAwait(false);
        }

        if (await manager.FindByNameAsync(AuthConfiguration.WriteApiScope)
                .ConfigureAwait(false) is null)
        {
            logger.CreatingScope(AuthConfiguration.WriteApiScope);
            await manager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    DisplayName = "Write API access",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("de-DE")] = "API Schreibzugriff"
                    },
                    Name = AuthConfiguration.WriteApiScope,
                    Resources =
                    {
                        AuthConfiguration.Audience
                    }
                }
            ).ConfigureAwait(false);
        }

        if (await manager.FindByNameAsync(AuthConfiguration.ManageUserApiScope)
                .ConfigureAwait(false) is null)
        {
            logger.CreatingScope(AuthConfiguration.ManageUserApiScope);
            await manager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    DisplayName = "Manage user API access",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("de-DE")] = "Benutzerverwaltung-API-Zugriff"
                    },
                    Name = AuthConfiguration.ManageUserApiScope,
                    Resources =
                    {
                        AuthConfiguration.Audience
                    }
                }
            ).ConfigureAwait(false);
        }
    }

    private static async Task CreateInstitutionsAsync(
        IServiceProvider services,
        ILogger<DbSeeder> logger,
        IWebHostEnvironment environment
    )
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (environment.IsDevelopment())
        {
            var iseInstitution = await context.Institutions.Where(x => x.Name == IseInstitutionName).SingleOrDefaultAsync().ConfigureAwait(false);
            if (iseInstitution is null)
            {
                iseInstitution = new Institution(
                    IseInstitutionName,
                    "ISE",
                    "Fraunhofer Institute for Solar Energy Systems (ISE)",
                    new Uri("https://www.ise.fraunhofer.de", UriKind.Absolute),
                    null,
                    InstitutionState.VERIFIED,
                    InstitutionOperatingState.OPERATING
                );
                iseInstitution.RepresentativeEdges.Add(
                    new InstitutionRepresentative
                    {
                        UserId = (await context.Users.Where(x => x.Email == AdministratorUser.EmailAddress).SingleAsync().ConfigureAwait(false)).Id,
                        Role = InstitutionRepresentativeRole.OWNER,
                        Pending = false
                    }
                );
                context.Institutions.Add(iseInstitution);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!await context.Institutions.Where(x => x.Name == TestlabInstitutionName).AnyAsync().ConfigureAwait(false))
            {
                var institution = new Institution(
                    TestlabInstitutionName,
                    "TLSF",
                    "This institution represents the TestLab Solar Facades of Fraunhofer ISE",
                    new Uri("https://www.ise.fraunhofer.de/en/rd-infrastructure/accredited-labs/testlab-solar-facades.html", UriKind.Absolute),
                    null,
                    InstitutionState.VERIFIED,
                    InstitutionOperatingState.OPERATING
                )
                {
                    ManagerId = iseInstitution.Id
                };
                context.Institutions.Add(institution);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!await context.Institutions.Where(x => x.Name == LbnlInstitutionName).AnyAsync().ConfigureAwait(false))
            {
                var institution = new Institution(
                    LbnlInstitutionName,
                    "LBNL",
                    "Lawrence Berkeley National Laboratory or Berkeley Lab",
                    new Uri("https://www.lbl.gov", UriKind.Absolute),
                    null,
                    InstitutionState.VERIFIED,
                    InstitutionOperatingState.OPERATING
                )
                {
                    ManagerId = iseInstitution.Id
                };
                context.Institutions.Add(institution);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }

    private static async Task CreateDatabasesAsync(
        IServiceProvider services,
        ILogger<DbSeeder> logger,
        IWebHostEnvironment environment,
        AppSettings appSettings
    )
    {
        if (environment.IsDevelopment())
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            if (!await context.Databases.Where(x => x.Name == TestlabDatabaseName).AnyAsync().ConfigureAwait(false))
            {
                var uriBuilder = new UriBuilder(new Uri(appSettings.TestlabSolarFacadesHost, UriKind.Absolute));
                uriBuilder.Path = "/graphql/";
                var database = new Database(
                    TestlabDatabaseName,
                    "The database of the TestLab Solar Facades of Fraunhofer ISE",
                    uriBuilder.Uri
                )
                {
                    OperatorId = (await context.Institutions.SingleAsync(x => x.Name == TestlabInstitutionName).ConfigureAwait(false)).Id
                };
                database.Verify();
                context.Databases.Add(database);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!await context.Databases.Where(x => x.Name == IgsdbDatabaseName).AnyAsync().ConfigureAwait(false))
            {
                var database = new Database(
                    IgsdbDatabaseName,
                    "The International Glazing and Shading Database (IGSDB)",
                    new Uri(environment.IsProduction() ? "https://igsdb-v2.herokuapp.com/graphql/" : "https://igsdb-v2-staging.herokuapp.com/graphql/", UriKind.Absolute)
                )
                {
                    OperatorId = (await context.Institutions.SingleAsync(x => x.Name == LbnlInstitutionName).ConfigureAwait(false)).Id
                };
                database.Verify();
                context.Databases.Add(database);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}