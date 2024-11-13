using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

// Inspired by https://github.com/npgsql/efcore.pg/blob/main/src/EFCore.PG/Infrastructure/Internal/NpgsqlOptionsExtension.cs
namespace Metabase.Data.Extensions;

public sealed class SchemaNameOptionsExtension(string schemaName)
        : IDbContextOptionsExtension
{
    public string SchemaName { get; } = schemaName;

    public DbContextOptionsExtensionInfo Info
        => new SchemaNameExtensionInfo(this);

    public void ApplyServices(IServiceCollection services)
    {
    }

    public void Validate(IDbContextOptions options)
    {
    }

    public sealed class SchemaNameExtensionInfo(SchemaNameOptionsExtension extension)
                : DbContextOptionsExtensionInfo(extension)
    {
        public override bool IsDatabaseProvider
            => false;

        public override string LogFragment
            => $"{nameof(Extension.SchemaName)}={Extension.SchemaName}";

        public new SchemaNameOptionsExtension Extension => (SchemaNameOptionsExtension)base.Extension;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return true;
        }

        public override int GetServiceProviderHashCode()
        {
            return 0;
        }

        public override void PopulateDebugInfo(
            IDictionary<string, string> debugInfo
        )
        {
            debugInfo[$"Metabase.Data.Extensions:${nameof(SchemaName)}"]
                = Extension.SchemaName;
        }
    }
}