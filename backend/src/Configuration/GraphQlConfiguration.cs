using System;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
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
using Metabase.GraphQl.InstitutionMethodDevelopers;
using Metabase.GraphQl.InstitutionRepresentatives;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Methods;
using Metabase.GraphQl.Numerations;
using Metabase.GraphQl.OpenIdConnect;
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
            .AddFiltering()
            .AddSorting()
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
            // Configure `https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Validation/Options/ValidationOptions.cs`. But how?
            // Subscriptions
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
            .AddType<OpendIdConnectQueries>()
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
            /* .AddSubscriptionType(d => d.Name(nameof(GraphQl.Subscription))) */
            /*     .AddType<ComponentSubscriptions>() */
            // Object Types
            .AddType<OpenEndedDateTimeRangeType>()
            .AddType<ComponentType>()
            .AddType<DataFormatType>()
            .AddType<CalorimetricData>()
            .AddType<DataApproval>()
            .AddType<GetHttpsResourceTreeNonRootVertex>()
            .AddType<GetHttpsResourceTreeRoot>()
            .AddType<HygrothermalData>()
            .AddType<OpticalData>()
            .AddType<PhotovoltaicData>()
            .AddType<ResponseApproval>()
            .AddType<DatabaseType>()
            .AddType<InstitutionType>()
            .AddType<MethodType>()
            .AddType<NumerationType>()
            .AddType<OpenIdConnectApplicationType>()
            .AddType<OpenIdConnectAuthorizationType>()
            .AddType<OpenIdConnectScopeType>()
            .AddType<OpenIdConnectTokenType>()
            .AddType<PublicationType>()
            .AddType<ReferenceType>()
            .AddType<StakeholderType>()
            .AddType<StandardType>()
            .AddType<UserType>()
            .AddType<GeometricData>()
            // Data Loaders
            .AddDataLoader<ComponentByIdDataLoader>()
            .AddDataLoader<DataFormatByIdDataLoader>()
            .AddDataLoader<DatabaseByIdDataLoader>()
            .AddDataLoader<InstitutionByIdDataLoader>()
            .AddDataLoader<InstitutionRepresentativesByInstitutionIdDataLoader>()
            .AddDataLoader<MethodByIdDataLoader>()
            // Filtering
            .AddConvention<IFilterConvention>(
                new FilterConventionExtension(descriptor =>
                    {
                        descriptor.BindRuntimeType<Component, ComponentFilterType>();
                        descriptor
                            .BindRuntimeType<ComponentAssembly,
                                ComponentAssemblyFilterType>();
                        descriptor
                            .BindRuntimeType<ComponentManufacturer,
                                ComponentManufacturerFilterType>();
                        descriptor.BindRuntimeType<DataFormat, DataFormatFilterType>();
                        descriptor.BindRuntimeType<Database, DatabaseFilterType>();
                        descriptor.BindRuntimeType<Institution, InstitutionFilterType>();
                        descriptor
                            .BindRuntimeType<InstitutionMethodDeveloper, InstitutionMethodDeveloperFilterType>();
                        descriptor
                            .BindRuntimeType<InstitutionRepresentative,
                                InstitutionRepresentativeFilterType>();
                        descriptor.BindRuntimeType<Method, MethodFilterType>();
                        descriptor.BindRuntimeType<User, UserFilterType>();
                        descriptor
                            .BindRuntimeType<UserMethodDeveloper,
                                UserMethodDeveloperFilterType>();
                    }
                )
            )
            // Paging
            .AddDbContextCursorPagingProvider()
            .ModifyPagingOptions(_ =>
                {
                    _.MaxPageSize = int.MaxValue - 1;
                    _.DefaultPageSize = int.MaxValue - 1;
                    _.IncludeTotalCount = true;
                    _.IncludeNodesField = false;
                }
            )
            .UseAutomaticPersistedOperationPipeline()
            .AddInMemoryOperationDocumentStorage(); // Needed by the automatic persisted operation pipeline
    }

    private sealed class MyUuidType : UuidType
    {
        private const string _specifiedBy = "https://tools.ietf.org/html/rfc4122";

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
            SpecifiedBy = new Uri(_specifiedBy, UriKind.Absolute);
        }
    }

    private sealed class MyUrlType : UrlType
    {
        private const string _specifiedBy = "https://tools.ietf.org/html/rfc3986";

        public MyUrlType(
            string name,
            string? description = null,
            BindingBehavior bind = BindingBehavior.Explicit)
            : base(name, description, bind)
        {
            SpecifiedBy = new Uri(_specifiedBy, UriKind.Absolute);
        }
    }
}