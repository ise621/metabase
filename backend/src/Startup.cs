using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotChocolate.AspNetCore;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Data.Extensions;
using Metabase.Enumerations;
using Metabase.GraphQl.Databases;
using Metabase.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;

namespace Metabase;

public sealed class Startup(
    IWebHostEnvironment environment,
    IConfiguration configuration
    )
{
    private const string GraphQlCorsPolicy = "GraphQlCorsPolicy";
    private const string AntiforgeryHeaderName = "X-XSRF-TOKEN";
    private readonly AppSettings _appSettings = configuration.Get<AppSettings>() ??
                       throw new InvalidOperationException(
                           "Failed to get application settings from configuration.");

    private readonly IWebHostEnvironment _environment = environment;

    public void ConfigureServices(IServiceCollection services)
    {
        AuthConfiguration.ConfigureServices(services, _environment, _appSettings);
        GraphQlConfiguration.ConfigureServices(services, _environment);
        ConfigureDatabaseServices(services);
        ConfigureMessageSenderServices(services);
        ConfigureRequestResponseServices(services);
        ConfigureSessionServices(services, _environment);
        services.AddAntiforgery(_ => { _.HeaderName = AntiforgeryHeaderName; });
        services
            .AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>();
        ConfigureHttpClientServices(services);
        services.AddHttpContextAccessor();
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
        services.AddSingleton(_appSettings);
        services.AddSingleton(_environment);
        // services.AddDatabaseDeveloperPageExceptionFilter();
    }

    private static void ConfigureRequestResponseServices(IServiceCollection services)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forwarded-headers-middleware-order
        services.Configure<ForwardedHeadersOptions>(_ =>
            {
                // TODO _.AllowedHosts = ...
                _.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forward-the-scheme-for-linux-and-non-iis-reverse-proxies
                _.KnownNetworks.Clear();
                _.KnownProxies.Clear();
            }
        );
        services.AddCors(_ =>
            _.AddPolicy(
                GraphQlCorsPolicy,
                policy =>
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            )
        );
        services.AddControllersWithViews();
    }

    private void ConfigureMessageSenderServices(IServiceCollection services)
    {
        services.AddTransient<IEmailSender>(serviceProvider =>
            new EmailSender(
                _appSettings.Email.SmtpHost,
                _appSettings.Email.SmtpPort,
                serviceProvider.GetRequiredService<ILogger<EmailSender>>()
            )
        );
    }

    private static void ConfigureSessionServices(
            IServiceCollection services,
            IWebHostEnvironment environment
            )
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state#session-state
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            // Set a short timeout for easy testing in development and a long
            // one otherwise.
            options.IdleTimeout =
                environment.IsDevelopment()
                ? TimeSpan.FromSeconds(10)
                : TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            // Make the session cookie essential
            options.Cookie.IsEssential = true;
        });
    }

    private static void ConfigureDatabaseContext(
        DbContextOptionsBuilder options,
        IWebHostEnvironment environment,
        AppSettings appSettings
        )
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(appSettings.Database.ConnectionString);
        // https://www.npgsql.org/efcore/mapping/enum.html#mapping-your-enum
        // Keep in sync with `ApplicationDbContext.CreateEnumerations`.
        dataSourceBuilder.MapEnum<ComponentCategory>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.ComponentCategoryTypeName}");
        dataSourceBuilder.MapEnum<DatabaseVerificationState>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.DatabaseVerificationStateTypeName}");
        dataSourceBuilder.MapEnum<InstitutionRepresentativeRole>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.InstitutionRepresentativeRoleTypeName}");
        dataSourceBuilder.MapEnum<InstitutionState>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.InstitutionStateTypeName}");
        dataSourceBuilder.MapEnum<InstitutionOperatingState>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.InstitutionOperatingStateTypeName}");
        dataSourceBuilder.MapEnum<MethodCategory>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.MethodCategoryTypeName}");
        dataSourceBuilder.MapEnum<PrimeSurface>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.PrimeSurfaceTypeName}");
        dataSourceBuilder.MapEnum<Standardizer>($"{appSettings.Database.SchemaName}.{ApplicationDbContext.StandardizerTypeName}");
        options
            .UseNpgsql(dataSourceBuilder.Build() /*, optionsBuilder => optionsBuilder.UseNodaTime() */)
            .UseSchemaName(appSettings.Database.SchemaName)
            .UseOpenIddict<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, Guid>();
        if (!environment.IsProduction())
        {
            options
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        if (environment.IsEnvironment(Program.TestEnvironment))
        {
            options.ConfigureWarnings(x =>
                x.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)
            );
        }
    }

    private void ConfigureDatabaseServices(IServiceCollection services)
    {
        var optionsLifetime = ServiceLifetime.Singleton;
        services.AddPooledDbContextFactory<ApplicationDbContext>(options => { });
        // Database context as service are used by `Identity` and
        // `OpenIddict`, see in particular `AuthConfiguration`,
        // `UseUserManagerAttribute` and `UseSignInManagerAttribute`.
        services.AddDbContext<ApplicationDbContext>(
            (services, options) =>
                services
                    .GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                    .CreateDbContext(),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: optionsLifetime
        );
        services.ConfigureDbContext<ApplicationDbContext>(options =>
            ConfigureDatabaseContext(options, _environment, _appSettings),
            optionsLifetime: optionsLifetime
        );
        // services.AddSingleton(services =>
        // {
        //     // new DbContextOptionsBuilder<TContext>(new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));
        //     return services.GetRequiredService<IEnumerable<IDbContextOptionsConfiguration<ApplicationDbContext>>>();
        // });
    }

    private static void ConfigureHttpClientServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpClient(QueryingDatabases.DatabaseHttpClient);
        // var httpClientBuilder = services.AddHttpClient(QueryingDatabases.DATABASE_HTTP_CLIENT);
        // if (!_environment.IsProduction())
        // {
        //     httpClientBuilder.ConfigurePrimaryHttpMessageHandler(_ =>
        //         new HttpClientHandler
        //         {
        //             ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //         }
        //     );
        // }
    }

    public void Configure(WebApplication app)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/
        if (_environment.IsDevelopment() || _environment.IsEnvironment(Program.TestEnvironment))
        {
            app.UseDeveloperExceptionPage();
            // app.UseMigrationsEndPoint();
            // Forwarded Headers Middleware must run before other middleware except diagnostics and error handling middleware. In particular before HSTS middleware.
            // See https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#other-proxy-server-and-load-balancer-scenarios
            app.UseForwardedHeaders();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // Forwarded Headers Middleware must run before other middleware except diagnostics and error handling middleware. In particular before HSTS middleware.
            // See https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#other-proxy-server-and-load-balancer-scenarios
            app.UseForwardedHeaders();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // ASP.NET advices to not use HSTS for APIs, see the warning on
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
            // app.UseHsts(); // Done by NGINX, see https://www.nginx.com/blog/http-strict-transport-security-hsts-and-nginx/
        }

        // app.UseStatusCodePages();
        // app.UseHttpsRedirection(); // Done by NGINX
        app.UseSerilogRequestLogging();
        app.UseStaticFiles();
        app.UseCookiePolicy(); // [SameSite cookies](https://learn.microsoft.com/en-us/aspnet/core/security/samesite)
        app.UseRouting();
        // TODO Do we really want this? See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0
        app.UseRequestLocalization(_ =>
        {
            _.AddSupportedCultures("en-US", "de-DE");
            _.AddSupportedUICultures("en-US", "de-DE");
            _.SetDefaultCulture("en-US");
        });
        app.UseCors();
        // app.UseCertificateForwarding(); // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0#other-web-proxies
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        // app.UseResponseCompression(); // Done by Nginx
        // app.UseResponseCaching(); // Done by Nginx
        /* app.UseWebSockets(); */
        app.MapGraphQL()
            .WithOptions(
                // https://chillicream.com/docs/hotchocolate/server/middleware
                new GraphQLServerOptions
                {
                    EnableSchemaRequests = true,
                    EnableGetRequests = false,
                    // AllowedGetOperations = AllowedGetOperations.Query
                    EnableMultipartRequests = false,
                    Tool =
                    {
                        DisableTelemetry = true,
                        Enable = true, // _environment.IsDevelopment()
                        IncludeCookies = false,
                        GraphQLEndpoint = "/graphql",
                        HttpMethod = DefaultHttpMethod.Post,
                        Title = "GraphQL"
                    }
                }
            )
            .RequireCors(GraphQlCorsPolicy);
        app.MapControllers();
        app.MapHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = WriteJsonResponse
            }
        ).DisableHttpMetrics();
    }

    // Inspired by https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0#customize-output
    private static Task WriteJsonResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        var options = new JsonWriterOptions { Indented = true };
        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteString("duration", healthReport.TotalDuration.ToString());
            jsonWriter.WriteStartObject("results");
            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteString("duration",
                    healthReportEntry.Value.Duration.ToString());
                jsonWriter.WriteStartArray("tags");
                foreach (var tag in healthReportEntry.Value.Tags)
                {
                    jsonWriter.WriteStringValue(tag);
                }

                jsonWriter.WriteEndArray();
                var exception = healthReportEntry.Value.Exception;
                if (exception is not null)
                {
                    jsonWriter.WriteStartObject("exception");
                    jsonWriter.WriteString("message", exception.Message);
                    if (exception.StackTrace is not null)
                    {
                        jsonWriter.WriteString("stackTrace", exception.StackTrace);
                    }

                    if (exception.InnerException is not null)
                    {
                        jsonWriter.WriteString("innerException", exception.InnerException.ToString());
                    }

                    if (exception.Source is not null)
                    {
                        jsonWriter.WriteString("source", exception.Source);
                    }

                    if (exception.TargetSite is not null)
                    {
                        jsonWriter.WriteString("targetSite", exception.TargetSite.ToString());
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteStartObject("data");
                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);
                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}