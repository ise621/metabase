using System;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using HotChocolate.Language;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl;
using Metabase.GraphQl.Common;
using Metabase.GraphQl.ComponentAssemblies;
using Metabase.GraphQl.ComponentGeneralizations;
using Metabase.GraphQl.ComponentManufacturers;
using Metabase.GraphQl.Components;
using Metabase.GraphQl.ComponentVariants;
using Metabase.GraphQl.Databases;
using Metabase.GraphQl.DataFormats;
using Metabase.GraphQl.DataX;
using Metabase.GraphQl.DescriptionOrReferences;
using Metabase.GraphQl.InstitutionMethodDevelopers;
using Metabase.GraphQl.InstitutionRepresentatives;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Methods;
using Metabase.GraphQl.Numerations;
using Metabase.GraphQl.OpenIdConnect.Application;
using Metabase.GraphQl.OpenIdConnect.Authorizations;
using Metabase.GraphQl.OpenIdConnect.Scopes;
using Metabase.GraphQl.OpenIdConnect.Tokens;
using Metabase.GraphQl.Publications;
using Metabase.GraphQl.References;
using Metabase.GraphQl.Stakeholders;
using Metabase.GraphQl.Standards;
using Metabase.GraphQl.UserMethodDevelopers;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace Metabase.Configuration;

public static class GraphQlConfiguration
{
    public static void ConfigureServices(
        IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        // Automatic-Persisted-Queries Services
        services
            .AddMemoryCache()
            .AddSha256DocumentHashProvider(HashFormat.Hex);
        // GraphQL Server
        services
            .AddGraphQLServer()
            // Services https://chillicream.com/docs/hotchocolate/v13/integrations/entity-framework#registerdbcontext
            .RegisterDbContextFactory<ApplicationDbContext>()
            .AddMutationConventions(new MutationConventionOptions { ApplyToAllMutations = false })
            // Extensions
            .AddProjections()
            .AddFiltering<CustomFilterConvention>()
            .AddSorting<CustomSortConvention>()
            .AddAuthorization()
            .AddGlobalObjectIdentification()
            .AddQueryFieldToMutationPayloads()
            .ModifyOptions(options =>
                {
                    // https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Types/Configuration/Contracts/ISchemaOptions.cs
                    options.StrictValidation = true;
                    options.UseXmlDocumentation = false;
                    options.SortFieldsByName = true;
                    options.RemoveUnreachableTypes = false;
                    options.DefaultBindingBehavior = BindingBehavior.Implicit;
                    /* options.FieldMiddleware = ... */
                }
            )
            .ModifyRequestOptions(options =>
                {
                    // https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Execution/Options/RequestExecutorOptions.cs
                    /* options.ExecutionTimeout = ...; */
                    options.IncludeExceptionDetails = !environment.IsProduction(); // Default is `Debugger.IsAttached`.
                    /* options.QueryCacheSize = ...; */
                    /* options.UseComplexityMultipliers = ...; */
                }
            )
            // Configure
            // `https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Validation/Options/ValidationOptions.cs`.
            // But how? Subscriptions
            /* .AddInMemorySubscriptions() */
            // Persisted queries
            /* .AddFileSystemOperationDocumentStorage("./persisted_operations") */
            /* .UsePersistedOperationPipeline(); */
            // HotChocolate uses the default authentication scheme,
            // which we set to `null` in `AuthConfiguration` to force
            // users to be explicit about what scheme to use when
            // making it easier to grasp the various authentication
            // flows.
            .AddHttpRequestInterceptor(async (httpContext, requestExecutor, requestBuilder, cancellationToken) =>
            {
                try
                {
                    await HttpContextAuthentication.Authenticate(httpContext);
                }
                catch (Exception e)
                {
                    // TODO Log to a `ILogger<GraphQlConfiguration>` instead.
                    Console.WriteLine(e);
                }
            })
            .AddDiagnosticEventListener(_ =>
                new LoggingDiagnosticEventListener(
                    _.GetApplicationService<ILogger<LoggingDiagnosticEventListener>>()
                )
            )
            // Scalar Types
            .AddType(new UuidType("Uuid",
                defaultFormat: 'D')) // https://chillicream.com/docs/hotchocolate/defining-a-schema/scalars#uuid-type
            .AddType(new UrlType("Url"))
            .AddType(new JsonType("Any",
                BindingBehavior
                    .Implicit)) // https://chillicream.com/blog/2023/02/08/new-in-hot-chocolate-13#json-scalar
                                // .BindRuntimeType<Guid, MyUuidType>()
                                // Query Types
            .AddQueryType(d => d.Name(nameof(Query)))
            .AddType<ComponentQueries>()
            .AddType<DataFormatQueries>()
            .AddType<DatabaseQueries>()
            .AddType<InstitutionQueries>()
            .AddType<MethodQueries>()
            .AddType<ApplicationQueries>()
            .AddType<AuthorizationQueries>()
            .AddType<TokenQueries>()
            .AddType<ScopeQueries>()
            .AddType<UserQueries>()
            // Mutation Types
            .AddMutationType(d => d.Name(nameof(Mutation)))
            .AddType<ComponentAssemblyMutations>()
            .AddType<ComponentGeneralizationMutations>()
            .AddType<ComponentManufacturerMutations>()
            .AddType<ComponentVariantMutations>()
            .AddType<ComponentMutations>()
            .AddType<DataFormatMutations>()
            .AddType<DatabaseMutations>()
            .AddType<InstitutionMethodDeveloperMutations>()
            .AddType<InstitutionRepresentativeMutations>()
            .AddType<InstitutionMutations>()
            .AddType<MethodMutations>()
            .AddType<UserMethodDeveloperMutations>()
            .AddType<UserMutations>()
            .AddType<ApplicationMutations>()
            .AddType<AuthorizationMutations>()
            .AddType<TokenMutations>()
            /* .AddSubscriptionType(d => d.Name(nameof(GraphQl.Subscription))) */
            /*     .AddType<ComponentSubscriptions>() */
            // Object Types
            .AddType<OpenEndedDateTimeRangeType>()
            .AddType<ComponentType>()
            .AddType<DataFormatType>()
            .AddType<DescriptionOrReferenceType>()
            .AddType<CalorimetricData>()
            .AddType<DataApproval>()
            .AddType<GetHttpsResourceTreeNonRootVertex>()
            .AddType<GetHttpsResourceTreeRoot>()
            .AddType<IData>()
            .AddType<HygrothermalData>()
            .AddType<OpticalData>()
            .AddType<PhotovoltaicData>()
            .AddType<GeometricData>()
            .AddType<ResponseApproval>()
            .AddType<DatabaseType>()
            .AddType<InstitutionType>()
            .AddType<MethodType>()
            .AddType<NumerationType>()
            .AddType<ApplicationType>()
            .AddType<AuthorizationType>()
            .AddType<ScopeType>()
            .AddType<TokenType>()
            .AddType<PublicationType>()
            .AddType<ReferenceType>()
            .AddType<StakeholderType>()
            .AddType<StandardType>()
            .AddType<UserType>()
            // Data Loaders
            .AddDataLoader<ComponentByIdDataLoader>()
            .AddDataLoader<DataFormatByIdDataLoader>()
            .AddDataLoader<DatabaseByIdDataLoader>()
            .AddDataLoader<InstitutionByIdDataLoader>()
            .AddDataLoader<InstitutionRepresentativesByInstitutionIdDataLoader>()
            .AddDataLoader<MethodByIdDataLoader>()
            // Paging
            .AddDbContextCursorPagingProvider()
            .ModifyPagingOptions(_ =>
                {
                    _.MaxPageSize = 100;
                    _.DefaultPageSize = 100;
                    _.IncludeTotalCount = true;
                    _.IncludeNodesField = false;
                }
            )
            .UseAutomaticPersistedOperationPipeline()
            .AddInMemoryOperationDocumentStorage(); // Needed by the automatic persisted operation pipeline
    }

    private sealed class MyUuidType : UuidType
    {
        private const string SpecifiedByString = "https://tools.ietf.org/html/rfc4122";

        public MyUuidType(
            string name,
            string? description = null,
            char defaultFormat = '\0',
            bool enforceFormat = false,
            BindingBehavior bind = BindingBehavior.Explicit
        )
            : base(name, description, defaultFormat, enforceFormat,
                bind)
        {
            SpecifiedBy = new Uri(SpecifiedByString, UriKind.Absolute);
        }
    }

    private sealed class MyUrlType : UrlType
    {
        private const string SpecifiedByString = "https://tools.ietf.org/html/rfc3986";

        public MyUrlType(
            string name,
            string? description = null,
            BindingBehavior bind = BindingBehavior.Explicit)
            : base(name, description, bind)
        {
            SpecifiedBy = new Uri(SpecifiedByString, UriKind.Absolute);
        }
    }
}

// See https://chillicream.com/docs/hotchocolate/fetching-data/filtering/#filter-conventions
public partial class CustomFilterConvention : FilterConvention
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        descriptor.AddDefaults();
        // Use argument name `where`
        descriptor.ArgumentName("where");
        // Allow conjunction and disjunction
        descriptor.AllowAnd();
        descriptor.AllowOr();
        // Bind custom types
        descriptor.BindRuntimeType<Component, ComponentFilterType>();
        descriptor.BindRuntimeType<ComponentAssembly, ComponentAssemblyFilterType>();
        descriptor.BindRuntimeType<ComponentManufacturer, ComponentManufacturerFilterType>();
        descriptor.BindRuntimeType<DataFormat, DataFormatFilterType>();
        descriptor.BindRuntimeType<Database, DatabaseFilterType>();
        descriptor.BindRuntimeType<DescriptionOrReference, DescriptionOrReferenceFilterType>();
        descriptor.BindRuntimeType<Institution, InstitutionFilterType>();
        descriptor.BindRuntimeType<InstitutionMethodDeveloper, InstitutionMethodDeveloperFilterType>();
        descriptor.BindRuntimeType<InstitutionRepresentative, InstitutionRepresentativeFilterType>();
        descriptor.BindRuntimeType<Method, MethodFilterType>();
        descriptor.BindRuntimeType<User, UserFilterType>();
        descriptor.BindRuntimeType<UserMethodDeveloper, UserMethodDeveloperFilterType>();
    }
}

public static class FilterConventionDescriptorExtensions
{
    // Inspired by FilterConventionDescriptorExtensions#AddDefaults
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L16
    public static IFilterConventionDescriptor AddDefaults(
        this IFilterConventionDescriptor descriptor)
    {
        return descriptor.AddDefaultOperations().BindDefaultTypes().UseQueryableProvider();
    }

    // Inspired by FilterConventionDescriptorExtensions#AddDefaultOperations
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L28
    public static IFilterConventionDescriptor AddDefaultOperations(
        this IFilterConventionDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals).Name("equalTo");
        descriptor.Operation(DefaultFilterOperations.NotEquals).Name("notEqualTo");
        descriptor.Operation(DefaultFilterOperations.GreaterThan).Name("greaterThan");
        descriptor.Operation(DefaultFilterOperations.NotGreaterThan).Name("notGreaterThan");
        descriptor.Operation(DefaultFilterOperations.GreaterThanOrEquals).Name("greaterThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.NotGreaterThanOrEquals).Name("notGreaterThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.LowerThan).Name("lessThan");
        descriptor.Operation(DefaultFilterOperations.NotLowerThan).Name("notLessThan");
        descriptor.Operation(DefaultFilterOperations.LowerThanOrEquals).Name("lessThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.NotLowerThanOrEquals).Name("notLessThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.Contains).Name("contains");
        descriptor.Operation(DefaultFilterOperations.NotContains).Name("doesNotContain");
        descriptor.Operation(DefaultFilterOperations.In).Name("in");
        descriptor.Operation(DefaultFilterOperations.NotIn).Name("notIn");
        descriptor.Operation(DefaultFilterOperations.StartsWith).Name("startsWith");
        descriptor.Operation(DefaultFilterOperations.NotStartsWith).Name("doesNotStartWith");
        descriptor.Operation(DefaultFilterOperations.EndsWith).Name("endsWith");
        descriptor.Operation(DefaultFilterOperations.NotEndsWith).Name("doesNotEndWith");
        descriptor.Operation(DefaultFilterOperations.All).Name("all");
        descriptor.Operation(DefaultFilterOperations.None).Name("none");
        descriptor.Operation(DefaultFilterOperations.Some).Name("some");
        descriptor.Operation(DefaultFilterOperations.Any).Name("any");
        descriptor.Operation(DefaultFilterOperations.And).Name("and");
        descriptor.Operation(DefaultFilterOperations.Or).Name("or");
        descriptor.Operation(DefaultFilterOperations.Data).Name("data");
        // TODO `descriptor.Operation(AdditionalFilterOperations.Not).Name("not");` as in the project `database`
        // TODO `inClosedInterval`
        return descriptor;
    }

    // Inspired by FilterConventionDescriptorExtensions#BindDefaultTypes
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L73
    public static IFilterConventionDescriptor BindDefaultTypes(
        this IFilterConventionDescriptor descriptor)
    {
        descriptor
            .BindRuntimeType<string, StringOperationFilterInputType>()
            .BindRuntimeType<bool, BooleanOperationFilterInputType>()
            .BindRuntimeType<bool?, BooleanOperationFilterInputType>()
            .BindComparableType<byte>("BytePropositionInput")
            .BindComparableType<short>("ShortPropositionInput")
            .BindComparableType<int>("IntPropositionInput")
            .BindComparableType<long>("LongPropositionInput")
            .BindComparableType<float>("FloatXPropositionInput")
            .BindComparableType<double>("FloatPropositionInput")
            .BindComparableType<decimal>("DecimalPropositionInput")
            .BindComparableType<sbyte>("SignedBytePropositionInput")
            .BindComparableType<ushort>("UnsignedShortPropositionInput")
            .BindComparableType<uint>("UnsignedIntPropositionInput")
            .BindComparableType<ulong>("UnsigendLongPropositionInput")
            .BindComparableType<Guid>("UuidPropositionInput")
            .BindComparableType<DateTime>("DateTimePropositionInput")
            .BindComparableType<DateTimeOffset>("DateTimeOffsetPropositionInput")
            .BindComparableType<TimeSpan>("TimeSpanPropositionInput");
        // TODO Why does this not work?
        // descriptor
        //     .Configure<StringOperationFilterInputType>(x => x.Name("StringPropositionInput"))
        //     .Configure<BooleanOperationFilterInputType>(x => x.Name("BooleanPropositionInput"));
        return descriptor;
    }

    // Inspired by FilterConventionDescriptorExtensions#FilterConventionDescriptorExtensions
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L102
    private static IFilterConventionDescriptor BindComparableType<T>(
        this IFilterConventionDescriptor descriptor,
        string? name = null)
        where T : struct
    {
        descriptor
            .BindRuntimeType<T, ComparableOperationFilterInputType<T>>()
            .BindRuntimeType<T?, ComparableOperationFilterInputType<T?>>();
        // TODO Why does this not work?
        // if (name is not null)
        // {
        //     descriptor
        //         .Configure<ComparableOperationFilterInputType<T>>(x => x.Name(name))
        //         .Configure<ComparableOperationFilterInputType<T?>>(x => x.Name($"Maybe{name}"));
        // }
        return descriptor;
    }
}

// See https://chillicream.com/docs/hotchocolate/fetching-data/sorting/#sorting-conventions
public partial class CustomSortConvention : SortConvention
{
    protected override void Configure(ISortConventionDescriptor descriptor)
    {
        descriptor.AddDefaults();
        // Bind custom types
        descriptor.BindRuntimeType<Component, ComponentSortType>();
        descriptor.BindRuntimeType<ComponentAssembly, ComponentAssemblySortType>();
        descriptor.BindRuntimeType<ComponentManufacturer, ComponentManufacturerSortType>();
        descriptor.BindRuntimeType<DataFormat, DataFormatSortType>();
        descriptor.BindRuntimeType<Database, DatabaseSortType>();
        descriptor.BindRuntimeType<DescriptionOrReference, DescriptionOrReferenceSortType>();
        descriptor.BindRuntimeType<Institution, InstitutionSortType>();
        descriptor.BindRuntimeType<InstitutionMethodDeveloper, InstitutionMethodDeveloperSortType>();
        descriptor.BindRuntimeType<InstitutionRepresentative, InstitutionRepresentativeSortType>();
        descriptor.BindRuntimeType<Method, MethodSortType>();
        descriptor.BindRuntimeType<User, UserSortType>();
        descriptor.BindRuntimeType<UserMethodDeveloper, UserMethodDeveloperSortType>();
    }
}