using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Metabase.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Log = Serilog.Log;

namespace Metabase;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "An error occurred creating and seeding the database.")]
    public static partial void FailedToCreateAndSeedDatabase(
        this ILogger logger,
        // The first exception is implicitly taken care of as detailed in
        // https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator#log-method-anatomy
        Exception exception
    );
}

public sealed class Program
{
    public const string TestEnvironment = "test";

    public static async Task<int> Main(
        string[] commandLineArguments
    )
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? throw new ArgumentException("Unknown enrivornment.");
        // https://github.com/serilog/serilog-aspnetcore#two-stage-initialization
        ConfigureBootstrapLogging(environment);
        try
        {
            Log.Information("Starting web host");
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication
            var builder = CreateWebApplicationBuilder(commandLineArguments);
            var startup = new Startup(builder.Environment, builder.Configuration);
            startup.ConfigureServices(builder.Services);
            var application = builder.Build();
            startup.Configure(application);
            using (var scope = application.Services.CreateScope())
            {
                // Inspired by https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro#initialize-db-with-test-data
                await CreateAndSeedDb(scope.ServiceProvider).ConfigureAwait(false);
            }

            await application.RunAsync().ConfigureAwait(false);
            return 0;
        }
        catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design") // see https://github.com/dotnet/efcore/issues/29923
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureBootstrapLogging(
        string environment
    )
    {
        var configuration = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information);
        ConfigureLogging(configuration, environment);
        Log.Logger = configuration.CreateBootstrapLogger();
    }

    private static void ConfigureLogging(
        LoggerConfiguration configuration,
        string environment
    )
    {
        configuration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Environment", environment)
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.File(
                new CompactJsonFormatter(),
                "./logs/serilog.json",
                fileSizeLimitBytes: 1073741824, // 1 GB
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 7);
        if (environment != "production")
        {
            configuration.WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture);
        }
    }

    private static async Task CreateAndSeedDb(
        IServiceProvider services
    )
    {
        try
        {
            using var dbContext =
                services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                    .CreateDbContext();
            dbContext.Database.EnsureCreated();
            await DbSeeder.DoAsync(services).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.FailedToCreateAndSeedDatabase(exception);
        }
    }

    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host
    private static WebApplicationBuilder CreateWebApplicationBuilder(
        string[] commandLineArguments
    )
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                Args = commandLineArguments,
                ContentRootPath = Directory.GetCurrentDirectory() // PlatformServices.Default.Application.ApplicationBasePath
            }
        );
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/
        ConfigureAppConfiguration(
            builder.Configuration,
            builder.Environment,
            commandLineArguments
        );
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
        // https://github.com/dotnet/aspnetcore/issues/38334#issuecomment-967709919
        builder.Host.UseDefaultServiceProvider(_ =>
        {
            _.ValidateScopes = true;
            _.ValidateOnBuild = true;
        });
        // https://github.com/serilog/serilog-aspnetcore#instructions
        builder.Host.UseSerilog((webHostBuilderContext, loggerConfiguration) =>
        {
            ConfigureLogging(
                loggerConfiguration,
                webHostBuilderContext.HostingEnvironment.EnvironmentName
            );
            loggerConfiguration
                .ReadFrom.Configuration(webHostBuilderContext.Configuration);
        });
        return builder;
    }

    public static void ConfigureAppConfiguration(
        IConfigurationBuilder configuration,
        IHostEnvironment environment,
        string[] commandLineArguments
    )
    {
        configuration.Sources.Clear();
        configuration
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile(
                "appsettings.json",
                false,
                !environment.IsEnvironment(TestEnvironment)
            )
            .AddJsonFile(
                $"appsettings.{environment.EnvironmentName}.json",
                false,
                !environment.IsEnvironment(TestEnvironment)
            )
            .AddEnvironmentVariables()
            .AddEnvironmentVariables(
                "XBASE_") // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#environment-variables
            .AddCommandLine(commandLineArguments);
    }
}