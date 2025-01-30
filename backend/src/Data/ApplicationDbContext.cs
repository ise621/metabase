using System;
using Metabase.Enumerations;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchemaNameOptionsExtension = Metabase.Data.Extensions.SchemaNameOptionsExtension;

namespace Metabase.Data;

// Inspired by
// [Authentication and authorization for SPAs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.0)
// [Customize Identity Model](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.0)
public sealed class ApplicationDbContext
    : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IDataProtectionKeyContext
{
    private const string DefaultSchemaName = "metabase";
    private readonly string _schemaName;

    internal const string ComponentCategoryTypeName = "component_category";
    internal const string DatabaseVerificationStateTypeName = "database_verification_state";
    internal const string InstitutionRepresentativeRoleTypeName = "institution_representative_role";
    internal const string InstitutionStateTypeName = "institution_state";
    internal const string InstitutionOperatingStateTypeName = "institution_operating_state";
    internal const string MethodCategoryTypeName = "method_category";
    internal const string PrimeSurfaceTypeName = "prime_surface";
    internal const string StandardizerTypeName = "standardizer";

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
    )
        : base(options)
    {
        // The schema-name option is set in `Metabase.Startup` by an invocation
        // of `UseSchemaName` on a `DbContextOptionsBuilder` instance.
        var schemaNameOptions = options.FindExtension<SchemaNameOptionsExtension>();
        _schemaName = schemaNameOptions is null ? DefaultSchemaName : schemaNameOptions.SchemaName;
    }

    // https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset
    public DbSet<Component> Components { get; private set; } = default!;
    public DbSet<ComponentAssembly> ComponentAssemblies { get; private set; } = default!;

    public DbSet<ComponentConcretizationAndGeneralization> ComponentConcretizationAndGeneralizations
    {
        get;
        private set;
    } = default!;

    public DbSet<ComponentManufacturer> ComponentManufacturers { get; private set; } = default!;
    public DbSet<ComponentVariant> ComponentVariants { get; private set; } = default!;
    public DbSet<DataFormat> DataFormats { get; private set; } = default!;
    public DbSet<Database> Databases { get; private set; } = default!;
    public DbSet<Institution> Institutions { get; private set; } = default!;
    public DbSet<InstitutionMethodDeveloper> InstitutionMethodDevelopers { get; private set; } = default!;
    public DbSet<InstitutionRepresentative> InstitutionRepresentatives { get; private set; } = default!;
    public DbSet<Method> Methods { get; private set; } = default!;
    public DbSet<UserMethodDeveloper> UserMethodDevelopers { get; private set; } = default!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; private set; } = default!;
    public DbSet<InstitutionApplication> ApplicationInstitutions { get; private set; } = default!;

    // Inspired by https://github.com/openiddict/openiddict-core/issues/1376#issuecomment-1151275376
    // It is needed to fix the following error that occurred when trying to redeem OpenId Connect tokens in production:
    // `Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported (...)`
    // See also https://github.com/openiddict/openiddict-core/issues/1376
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<UtcValueConverter>();
        base.ConfigureConventions(configurationBuilder);
    }

    private sealed class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter()
            : base(
                v => v,
                v => v.Kind != DateTimeKind.Unspecified ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc)
            )
        {
        }
    }

    private static
        EntityTypeBuilder<TEntity>
        ConfigureEntity<TEntity>(
            EntityTypeBuilder<TEntity> builder
        )
        where TEntity : Entity
    {
        // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        // https://www.npgsql.org/efcore/modeling/concurrency.html#the-postgresql-xmin-system-column
        builder
            .Property(e => e.Version)
            .IsRowVersion();
        return builder;
    }

    private static void ConfigureIdentityEntities(
        ModelBuilder builder
    )
    {
        // https://stackoverflow.com/questions/19902756/asp-net-identity-dbcontext-confusion/35722688#35722688
        builder.Entity<User>()
            .ToTable("user")
            .Property(e => e.Version)
            .IsRowVersion();
        builder.Entity<Role>().ToTable("role");
        builder.Entity<UserClaim>().ToTable("user_claim");
        builder.Entity<UserRole>().ToTable("user_role");
        builder.Entity<UserLogin>().ToTable("user_login");
        builder.Entity<RoleClaim>().ToTable("role_claim");
        builder.Entity<UserToken>().ToTable("user_token");
    }

    private static void ConfigureComponentAssembly(ModelBuilder builder)
    {
        // https://docs.microsoft.com/en-us/ef/core/modeling/relationships#join-entity-type-configuration
        builder.Entity<Component>()
            .HasMany(c => c.Parts)
            .WithMany(c => c.PartOf)
            .UsingEntity<ComponentAssembly>(
                j => j
                    .HasOne(e => e.PartComponent)
                    .WithMany(c => c.PartOfEdges)
                    .HasForeignKey(e => e.PartComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.AssembledComponent)
                    .WithMany(c => c.PartEdges)
                    .HasForeignKey(e => e.AssembledComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("component_assembly")
                    .HasKey(a => new { a.AssembledComponentId, a.PartComponentId })
            );
    }

    private static void ConfigureComponentConcretizationAndGeneralization(ModelBuilder builder)
    {
        // https://docs.microsoft.com/en-us/ef/core/modeling/relationships#join-entity-type-configuration
        builder.Entity<Component>()
            .HasMany(c => c.Concretizations)
            .WithMany(c => c.Generalizations)
            .UsingEntity<ComponentConcretizationAndGeneralization>(
                j => j
                    .HasOne(e => e.ConcreteComponent)
                    .WithMany(c => c.GeneralizationEdges)
                    .HasForeignKey(e => e.ConcreteComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.GeneralComponent)
                    .WithMany(c => c.ConcretizationEdges)
                    .HasForeignKey(e => e.GeneralComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("component_concretization_and_generalization")
                    .HasKey(a => new { a.GeneralComponentId, a.ConcreteComponentId })
            );
    }

    private static void ConfigureComponentVariant(ModelBuilder builder)
    {
        // https://docs.microsoft.com/en-us/ef/core/modeling/relationships#join-entity-type-configuration
        builder.Entity<Component>()
            .HasMany(c => c.Variants)
            .WithMany(c => c.VariantOf)
            .UsingEntity<ComponentVariant>(
                j => j
                    .HasOne(e => e.ToComponent)
                    .WithMany(c => c.VariantOfEdges)
                    .HasForeignKey(e => e.ToComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.OfComponent)
                    .WithMany(c => c.VariantEdges)
                    .HasForeignKey(e => e.OfComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("component_variant")
                    .HasKey(a => new { a.OfComponentId, a.ToComponentId })
            );
    }

    private static void ConfigureComponentManufacturer(ModelBuilder builder)
    {
        // https://docs.microsoft.com/en-us/ef/core/modeling/relationships#join-entity-type-configuration
        builder.Entity<Component>()
            .HasMany(c => c.Manufacturers)
            .WithMany(i => i.ManufacturedComponents)
            .UsingEntity<ComponentManufacturer>(
                j => j
                    .HasOne(e => e.Institution)
                    .WithMany(i => i.ManufacturedComponentEdges)
                    .HasForeignKey(e => e.InstitutionId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.Component)
                    .WithMany(c => c.ManufacturerEdges)
                    .HasForeignKey(e => e.ComponentId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("component_manufacturer")
                    .HasKey(a => new { a.ComponentId, a.InstitutionId })
            );
    }

    private static void ConfigureInstitutionMethodDeveloper(ModelBuilder builder)
    {
        builder.Entity<Method>()
            .HasMany(m => m.InstitutionDevelopers)
            .WithMany(i => i.DevelopedMethods)
            .UsingEntity<InstitutionMethodDeveloper>(
                j => j
                    .HasOne(e => e.Institution)
                    .WithMany(i => i.DevelopedMethodEdges)
                    .HasForeignKey(e => e.InstitutionId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.Method)
                    .WithMany(m => m.InstitutionDeveloperEdges)
                    .HasForeignKey(e => e.MethodId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("institution_method_developer")
                    .HasKey(a => new { a.InstitutionId, a.MethodId })
            );
    }

    private static void ConfigureInstitutionRepresentative(ModelBuilder builder)
    {
        builder.Entity<Institution>()
            .HasMany(i => i.Representatives)
            .WithMany(u => u.RepresentedInstitutions)
            .UsingEntity<InstitutionRepresentative>(
                j => j
                    .HasOne(e => e.User)
                    .WithMany(u => u.RepresentedInstitutionEdges)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.Institution)
                    .WithMany(i => i.RepresentativeEdges)
                    .HasForeignKey(e => e.InstitutionId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("institution_representative")
                    .HasKey(a => new { a.InstitutionId, a.UserId })
            );
    }

    private static void ConfigureInstitutionApplication(ModelBuilder builder)
    {
        builder.Entity<Institution>()
            .HasMany(i => i.Applications)
            .WithMany(a => a.Institutions)
            .UsingEntity<InstitutionApplication>(
                j => j
                    .HasOne(e => e.Application)
                    .WithMany(u => u.InstitutionEdges)
                    .HasForeignKey(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.Institution)
                    .WithMany(i => i.ApplicationEdges)
                    .HasForeignKey(e => e.InstitutionId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("institution_application")
                    .HasKey(a => new { a.InstitutionId, a.ApplicationId })
            );
    }

    private static void ConfigureDatabaseOperator(ModelBuilder builder)
    {
        builder.Entity<Institution>()
            .HasMany(i => i.OperatedDatabases)
            .WithOne(i => i.Operator)
            .HasForeignKey(i => i.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureUserMethodDeveloper(ModelBuilder builder)
    {
        builder.Entity<Method>()
            .HasMany(m => m.UserDevelopers)
            .WithMany(i => i.DevelopedMethods)
            .UsingEntity<UserMethodDeveloper>(
                j => j
                    .HasOne(e => e.User)
                    .WithMany(i => i.DevelopedMethodEdges)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(e => e.Method)
                    .WithMany(m => m.UserDeveloperEdges)
                    .HasForeignKey(e => e.MethodId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .ToTable("user_method_developer")
                    .HasKey(a => new { a.UserId, a.MethodId })
            );
    }

    private static void ConfigureInstitutionManager(ModelBuilder builder)
    {
        builder.Entity<Institution>()
            .HasMany(i => i.ManagedInstitutions)
            .WithOne(i => i.Manager)
            .HasForeignKey(i => i.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureDataFormatManager(ModelBuilder builder)
    {
        builder.Entity<Institution>()
            .HasMany(i => i.ManagedDataFormats)
            .WithOne(i => i.Manager)
            .HasForeignKey(i => i.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureMethodManager(ModelBuilder builder)
    {
        builder.Entity<Institution>()
            .HasMany(i => i.ManagedMethods)
            .WithOne(i => i.Manager)
            .HasForeignKey(i => i.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(_schemaName);
        builder.HasPostgresExtension(
            "pgcrypto"); // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        ConfigureIdentityEntities(builder);
        ConfigureEntity(
                builder.Entity<Component>()
            )
            .ToTable("component");
        ConfigureComponentAssembly(builder);
        ConfigureComponentConcretizationAndGeneralization(builder);
        ConfigureComponentManufacturer(builder);
        ConfigureComponentVariant(builder);
        ConfigureEntity(
                builder.Entity<Database>()
            )
            .ToTable("database");
        ConfigureEntity(
                builder.Entity<DataFormat>()
            )
            .ToTable("data_format");
        ConfigureEntity(
                builder.Entity<Institution>()
            )
            .ToTable("institution");
        ConfigureInstitutionMethodDeveloper(builder);
        ConfigureInstitutionRepresentative(builder);
        ConfigureInstitutionApplication(builder);
        ConfigureDatabaseOperator(builder);
        ConfigureEntity(
                builder.Entity<Method>()
            )
            .ToTable("method");
        ConfigureUserMethodDeveloper(builder);
        ConfigureInstitutionManager(builder);
        ConfigureDataFormatManager(builder);
        ConfigureMethodManager(builder);
    }
}