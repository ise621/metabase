// Inspired by
// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0#customize-webapplicationfactory
// https://www.thinktecture.com/en/entity-framework-core/isolation-of-integration-tests-in-2-1/

using System;
using System.Linq;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Metabase.Tests.Integration;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Metabase.Program>
{
    private bool _disposed;

    public CustomWebApplicationFactory()
    {
        EmailSender = new CollectingEmailSender();
        Do(
            services =>
                SetUpDatabase(services.GetRequiredService<ApplicationDbContext>())
        );
    }

    public CollectingEmailSender EmailSender { get; }

    private void Do(Action<IServiceProvider> what)
    {
        using var scope = Services.CreateScope();
        what(scope.ServiceProvider);
    }

    private T Get<T>(Func<IServiceProvider, T> what)
    {
        using var scope = Services.CreateScope();
        return what(scope.ServiceProvider);
    }

    private async Task DoAsync(Func<IServiceProvider, Task> what)
    {
        using var scope = Services.CreateScope();
        await what(scope.ServiceProvider).ConfigureAwait(false);
    }

    // private TResult Get<TResult>(Func<IServiceProvider, TResult> what)
    // {
    //     using var scope = Services.CreateScope();
    //     return what(scope.ServiceProvider);
    // }

    public AppSettings AppSettings
    {
        get
        {
            return Get(
                    services =>
                        services.GetRequiredService<AppSettings>()
                );
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Metabase.Program.TestEnvironment);
        builder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
            {
                configurationBuilder.Sources.Clear();
                Metabase.Program.ConfigureAppConfiguration(
                    configurationBuilder,
                    webHostBuilderContext.HostingEnvironment,
                    []
                );
            }
        );
        builder.ConfigureServices(serviceCollection =>
            {
                using var scope = serviceCollection.BuildServiceProvider().CreateScope();
                var appSettings = scope.ServiceProvider.GetRequiredService<AppSettings>();
                // appSettings.Database.SchemaName += Guid.NewGuid().ToString().Replace("-", ""); // does not work because enumeration types in public schema cannot be created when they already exist. We therefore create a whole new database for each test instead of just a new schema.
                appSettings.Database.ConnectionString =
                    $"Host=database; Port=5432; Database=xbase_test_{Guid.NewGuid().ToString().Replace("-", "")}; User Id=postgres; Password=postgres; Maximum Pool Size=90;";
                // Configure `IEmailSender`
                var emailSenderServiceDescriptor =
                    serviceCollection.SingleOrDefault(d =>
                        d.ServiceType == typeof(IEmailSender)
                    );
                if (emailSenderServiceDescriptor is not null)
                {
                    serviceCollection.Remove(emailSenderServiceDescriptor);
                }

                serviceCollection.AddTransient<IEmailSender>(_ => EmailSender);
            }
        );
    }

    private void SetUpDatabase(DbContext dbContext)
    {
        // https://docs.microsoft.com/en-us/ef/core/managing-schemas/ensure-created#multiple-dbcontext-classes
        var databaseCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
        try
        {
            // There are faster ways to reset databases than below. For details see
            // https://www.mikee.se/posts/fastest_way_to_reset_database_with_ef_core_20220103
            databaseCreator.EnsureDeleted();
            databaseCreator.EnsureCreated();
        }
        catch (Npgsql.PostgresException exception)
        {
            if (exception.Message == "57P01: terminating connection due to administrator command")
            {
                // TODO Instead of ignoring the exception, change the code such that it is not thrown in the first place.
                // For reasons on why this exception may occur see
                // https://github.com/npgsql/efcore.pg/issues/1926
                Console.WriteLine($"Caught and ignored 'Npgsql.PostgresException' exception '{exception.Message}'.");
            }
            else
            {
                throw;
            }
        }
        databaseCreator.CreateTables();
        Task.Run(SeedDatabase).GetAwaiter().GetResult();
    }

    private async Task SeedDatabase()
    {
        await DoAsync(
            DbSeeder.DoAsync
        ).ConfigureAwait(false);
    }

    // https://docs.microsoft.com/en-us/dotnet/standard/managed-code
    // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    ~CustomWebApplicationFactory()
    {
        Dispose(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Do(
                services =>
                {
                    services
                        .GetRequiredService<ApplicationDbContext>()
                        .Database
                        .EnsureDeleted();
                }
            );
            _disposed = true;
        }

        base.Dispose(disposing);
    }
}