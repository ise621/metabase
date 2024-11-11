using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using HotChocolate;
using HotChocolate.Resolvers;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.DataX;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Metabase.GraphQl.Databases;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Warning,
        Message = "Failed with errors {Errors} to query the database {Locator} for {Request}.")]
    public static partial void FailedWithErrors(
        this ILogger logger,
        string Errors,
        Uri Locator,
        string Request
    );

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Failed with status code {StatusCode} to request {Locator} for {Request}.")]
    public static partial void FailedWithStatusCode(
        this ILogger logger,
        Exception exception,
        HttpStatusCode? StatusCode,
        Uri Locator,
        string Request
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message =
            "Failed to deserialize GraphQL response of request to {Locator} for {Request}. The details given are: Zero-based number of bytes read within the current line before the exception are {BytePositionInLine}, zero-based number of lines read before the exception are {LineNumber}, message that describes the current exception is '{Message}', path within the JSON where the exception was encountered is {Path}.")]
    public static partial void FailedToDeserialize(
        this ILogger logger,
        Exception exception,
        Uri Locator,
        string Request,
        long? BytePositionInLine,
        long? LineNumber,
        string Message,
        string? Path
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Failed to request {Locator} for {Request} or failed to deserialize the response.")]
    public static partial void FailedToRequestOrDeserialize(
        this ILogger logger,
        Exception exception,
        Uri Locator,
        string Request
    );
}

public sealed class DatabaseResolvers
{
    private const string IgsdbUrl = "https://igsdb-v2.herokuapp.com/graphql/";
    private const string IgsdbStagingUrl = "https://igsdb-v2-staging.herokuapp.com/graphql/";

    private static readonly string[] _dataFileNames =
    {
        "DataFields.graphql",
        "Data.graphql"
    };

    private static readonly string[] _opticalDataFileNames =
    {
        "DataFields.graphql",
        "OpticalDataFields.graphql",
        "OpticalData.graphql"
    };

    private static readonly string[] _hygrothermalDataFileNames =
    {
        "DataFields.graphql",
        "HygrothermalDataFields.graphql",
        "HygrothermalData.graphql"
    };

    private static readonly string[] _calorimetricDataFileNames =
    {
        "DataFields.graphql",
        "CalorimetricDataFields.graphql",
        "CalorimetricData.graphql"
    };

    private static readonly string[] _photovoltaicDataFileNames =
    {
        "DataFields.graphql",
        "PhotovoltaicDataFields.graphql",
        "PhotovoltaicData.graphql"
    };

    private static readonly string[] _geometricDataFileNames =
    {
        "DataFields.graphql",
        "GeometricDataFields.graphql",
        "GeometricData.graphql"
    };

    private static readonly string[] _igsdbAllDataFileNames =
    {
        "AllDataIgsdb.graphql"
    };

    private static readonly string[] _allDataFileNames =
    {
        "DataFields.graphql",
        // "PageInfoFields.graphql",
        "AllData.graphql"
    };

    private static readonly string[] _igsdbAllOpticalDataFileNames =
    {
        "AllOpticalDataIgsdb.graphql"
    };

    private static readonly string[] _allOpticalDataFileNames =
    {
        "DataFields.graphql",
        "OpticalDataFields.graphql",
        // "PageInfoFields.graphql",
        "AllOpticalData.graphql"
    };

    private static readonly string[] _allHygrothermalDataFileNames =
    {
        "DataFields.graphql",
        "HygrothermalDataFields.graphql",
        // "PageInfoFields.graphql",
        "AllHygrothermalData.graphql"
    };

    private static readonly string[] _allCalorimetricDataFileNames =
    {
        "DataFields.graphql",
        "CalorimetricDataFields.graphql",
        // "PageInfoFields.graphql",
        "AllCalorimetricData.graphql"
    };

    private static readonly string[] _allPhotovoltaicDataFileNames =
    {
        "DataFields.graphql",
        "PhotovoltaicDataFields.graphql",
        // "PageInfoFields.graphql",
        "AllPhotovoltaicData.graphql"
    };

    private static readonly string[] _igsdbAllGeometricDataFileNames =
    {
        "AllGeometricDataIgsdb.graphql"
    };

    private static readonly string[] _allGeometricDataFileNames =
    {
        "DataFields.graphql",
        "GeometricDataFields.graphql",
        "AllGeometricData.graphql"
    };

    private static readonly string[] _hasDataFileNames =
    {
        "HasData.graphql"
    };

    private static readonly string[] _hasOpticalDataFileNames =
    {
        "HasOpticalData.graphql"
    };

    private static readonly string[] _hasCalorimetricDataFileNames =
    {
        "HasCalorimetricData.graphql"
    };

    private static readonly string[] _hasHygrothermalDataFileNames =
    {
        "HasHygrothermalData.graphql"
    };

    private static readonly string[] _hasPhotovoltaicDataFileNames =
    {
        "HasPhotovoltaicData.graphql"
    };

    private static readonly string[] _hasGeometricDataFileNames =
    {
        "HasGeometricData.graphql"
    };

    private readonly AppSettings _appSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DatabaseResolvers> _logger;

    public DatabaseResolvers(
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        ILogger<DatabaseResolvers> logger
    )
    {
        _appSettings = appSettings;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private static bool IsIgsdbDatabase(Database database)
    {
        return new[] { IgsdbUrl, IgsdbStagingUrl }
            .Contains(database.Locator.AbsoluteUri);
    }

    public Task<bool> GetCanCurrentUserUpdateNodeAsync(
        [Parent] Database database,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return DatabaseAuthorization.IsAuthorizedToUpdate(claimsPrincipal, database.Id, userManager, context,
            cancellationToken);
    }

    public Task<bool> GetCanCurrentUserVerifyNodeAsync(
        [Parent] Database database,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return DatabaseAuthorization.IsAuthorizedToVerify(claimsPrincipal, database.Id, userManager, context,
            cancellationToken);
    }

    public async Task<IData?> GetDataAsync(
        [Parent] Database database,
        Guid id,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<DataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _dataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            id,
                            timestamp,
                            locale
                        },
                        nameof(Data)
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.Data;
    }

    public async Task<OpticalData?> GetOpticalDataAsync(
        [Parent] Database database,
        Guid id,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<OpticalDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _opticalDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            id,
                            timestamp,
                            locale
                        },
                        nameof(OpticalData)
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.OpticalData;
    }

    public async Task<HygrothermalData?> GetHygrothermalDataAsync(
        [Parent] Database database,
        Guid id,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HygrothermalDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hygrothermalDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            id,
                            timestamp,
                            locale
                        },
                        nameof(HygrothermalData)
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HygrothermalData;
    }

    public async Task<CalorimetricData?> GetCalorimetricDataAsync(
        [Parent] Database database,
        Guid id,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<CalorimetricDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _calorimetricDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            id,
                            timestamp,
                            locale
                        },
                        nameof(CalorimetricData)
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.CalorimetricData;
    }

    public async Task<PhotovoltaicData?> GetPhotovoltaicDataAsync(
        [Parent] Database database,
        Guid id,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<PhotovoltaicDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _photovoltaicDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            id,
                            timestamp,
                            locale
                        },
                        nameof(PhotovoltaicData)
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.PhotovoltaicData;
    }

    public async Task<GeometricData?> GetGeometricDataAsync(
        [Parent] Database database,
        Guid id,
        DateTime? timestamp,
        string? locale,
        [Service] IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<GeometricDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _geometricDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            id,
                            timestamp,
                            locale
                        },
                        nameof(GeometricData)
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.GeometricData;
    }

    public async Task<DataConnection?> GetAllDataAsync(
        [Parent] Database database,
        DataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        uint? first,
        string? after,
        uint? last,
        string? before,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (IsIgsdbDatabase(database))
        {
            return DataConnection.From(
                (await QueryDatabase<AllDataDataIgsdb>(
                        database,
                        new GraphQLRequest(
                            await QueryingDatabases.ConstructQuery(_igsdbAllDataFileNames).ConfigureAwait(false),
                            new
                            {
                                where = RewriteDataPropositionInput(where, database)
                            },
                            "AllData"
                        ),
                        httpContextAccessor,
                        resolverContext,
                        cancellationToken
                    ).ConfigureAwait(false)
                )?.AllData
            );
        }
        return (await QueryDatabase<AllDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(_allDataFileNames).ConfigureAwait(false),
                        new
                        {
                            where = RewriteDataPropositionInput(where, database),
                            timestamp,
                            locale,
                            first,
                            after,
                            last,
                            before
                        },
                        "AllData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.AllData;
    }

    private static DataPropositionInput? RewriteDataPropositionInput(
        DataPropositionInput? where,
        Database database
    )
    {
        return IsIgsdbDatabase(database)
            ? where ?? new DataPropositionInput(null, null, null, null, null, null, null, null, null, null, null, null, null, null)
            : where;
    }

    public async Task<OpticalDataConnection?> GetAllOpticalDataAsync(
        [Parent] Database database,
        OpticalDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        uint? first,
        string? after,
        uint? last,
        string? before,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (IsIgsdbDatabase(database))
        {
            return OpticalDataConnection.From(
                (await QueryDatabase<AllOpticalDataDataIgsdb>(
                        database,
                        new GraphQLRequest(
                            await QueryingDatabases.ConstructQuery(
                                _igsdbAllOpticalDataFileNames).ConfigureAwait(false),
                            new
                            {
                                where = RewriteOpticalDataPropositionInput(where, database)
                            },
                            "AllOpticalData"
                        ),
                        httpContextAccessor,
                        resolverContext,
                        cancellationToken
                    ).ConfigureAwait(false)
                )?.AllOpticalData
            );
        }
        return (await QueryDatabase<AllOpticalDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _allOpticalDataFileNames).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale,
                            first,
                            after,
                            last,
                            before
                        },
                        "AllOpticalData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.AllOpticalData;
    }

    private static OpticalDataPropositionInput? RewriteOpticalDataPropositionInput(
        OpticalDataPropositionInput? where,
        Database database
    )
    {
        return IsIgsdbDatabase(database)
            ? where ?? new OpticalDataPropositionInput(null, null, null, null, null, null, null, null, null, null, null, null)
            : where;
    }

    public async Task<HygrothermalDataConnection?> GetAllHygrothermalDataAsync(
        [Parent] Database database,
        HygrothermalDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        uint? first,
        string? after,
        uint? last,
        string? before,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<AllHygrothermalDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _allHygrothermalDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale,
                            first,
                            after,
                            last,
                            before
                        },
                        "AllHygrothermalData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.AllHygrothermalData;
    }

    public async Task<CalorimetricDataConnection?> GetAllCalorimetricDataAsync(
        [Parent] Database database,
        CalorimetricDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        uint? first,
        string? after,
        uint? last,
        string? before,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<AllCalorimetricDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _allCalorimetricDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale,
                            first,
                            after,
                            last,
                            before
                        },
                        "AllCalorimetricData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.AllCalorimetricData;
    }

    public async Task<PhotovoltaicDataConnection?> GetAllPhotovoltaicDataAsync(
        [Parent] Database database,
        PhotovoltaicDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        uint? first,
        string? after,
        uint? last,
        string? before,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<AllPhotovoltaicDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _allPhotovoltaicDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale,
                            first,
                            after,
                            last,
                            before
                        },
                        "AllPhotovoltaicData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.AllPhotovoltaicData;
    }

    private static GeometricDataPropositionInput? RewriteGeometricDataPropositionInput(
        GeometricDataPropositionInput? where,
        Database database
    )
    {
        return IsIgsdbDatabase(database)
            ? where ?? new GeometricDataPropositionInput(null, null, null, null, null, null)
            : where;
    }

    public async Task<GeometricDataConnection?> GetAllGeometricDataAsync(
        [Parent] Database database,
        GeometricDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        uint? first,
        string? after,
        uint? last,
        string? before,
        [Service] IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (IsIgsdbDatabase(database))
        {
            return GeometricDataConnection.From(
                (await QueryDatabase<AllGeometricDataDataIgsdb>(
                        database,
                        new GraphQLRequest(
                            await QueryingDatabases.ConstructQuery(
                                _igsdbAllGeometricDataFileNames).ConfigureAwait(false),
                            new
                            {
                                where = RewriteGeometricDataPropositionInput(where, database)
                            },
                            "AllGeometricData"
                        ),
                        httpContextAccessor,
                        resolverContext,
                        cancellationToken
                    ).ConfigureAwait(false)
                )?.AllGeometricData
            );
        }
        return (await QueryDatabase<AllGeometricDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _allGeometricDataFileNames).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale,
                            first,
                            after,
                            last,
                            before
                        },
                        "AllGeometricData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.AllGeometricData;
    }

    public async Task<bool?> GetHasDataAsync(
        [Parent] Database database,
        DataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HasDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hasDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where = RewriteDataPropositionInput(where, database),
                            timestamp,
                            locale
                        },
                        "HasData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HasData;
    }

    public async Task<bool?> GetHasOpticalDataAsync(
        [Parent] Database database,
        OpticalDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HasOpticalDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hasOpticalDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where = RewriteOpticalDataPropositionInput(where, database),
                            timestamp,
                            locale
                        },
                        "HasOpticalData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HasOpticalData;
    }

    public async Task<bool?> GetHasCalorimetricDataAsync(
        [Parent] Database database,
        CalorimetricDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HasCalorimetricDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hasCalorimetricDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale
                        },
                        "HasCalorimetricData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HasCalorimetricData;
    }

    public async Task<bool?> GetHasHygrothermalDataAsync(
        [Parent] Database database,
        HygrothermalDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HasHygrothermalDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hasHygrothermalDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale
                        },
                        "HasHygrothermalData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HasHygrothermalData;
    }

    public async Task<bool?> GetHasPhotovoltaicDataAsync(
        [Parent] Database database,
        PhotovoltaicDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HasPhotovoltaicDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hasPhotovoltaicDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale
                        },
                        "HasPhotovoltaicData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HasPhotovoltaicData;
    }

    public async Task<bool?> GetHasGeometricDataAsync(
        [Parent] Database database,
        GeometricDataPropositionInput? where,
        DateTime? timestamp,
        string? locale,
        [Service] IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return (await QueryDatabase<HasGeometricDataData>(
                    database,
                    new GraphQLRequest(
                        await QueryingDatabases.ConstructQuery(
                            _hasGeometricDataFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where,
                            timestamp,
                            locale
                        },
                        "HasGeometricData"
                    ),
                    httpContextAccessor,
                    resolverContext,
                    cancellationToken
                ).ConfigureAwait(false)
            )?.HasGeometricData;
    }

    private async
        Task<TGraphQlResponse?>
        QueryDatabase<TGraphQlResponse>(
            Database database,
            GraphQLRequest request,
            IHttpContextAccessor httpContextAccessor,
            IResolverContext resolverContext,
            CancellationToken cancellationToken
        )
        where TGraphQlResponse : class
    {
        try
        {
            var deserializedGraphQlResponse =
                await QueryingDatabases.QueryDatabase<TGraphQlResponse>(
                    database,
                    request,
                    _httpClientFactory,
                    httpContextAccessor,
                    cancellationToken,
                    IsIgsdbDatabase(database) ? _appSettings.IgsdbApiToken : null
                );
            if (deserializedGraphQlResponse.Errors?.Length >= 1)
            {
                _logger.FailedWithErrors(JsonSerializer.Serialize(deserializedGraphQlResponse.Errors),
                    database.Locator, JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions));
                foreach (var error in deserializedGraphQlResponse.Errors)
                {
                    var errorBuilder = ErrorBuilder.New()
                        .SetCode("DATABASE_QUERY_ERROR")
                        .SetMessage(
                            $"The GraphQL response received from the database {database.Locator} for the request {JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions)} reported the error {error.Message}.")
                        .SetPath(error.Path);
                    if (error.Extensions is not null)
                        foreach (var (key, value) in error.Extensions)
                            errorBuilder.SetExtension(key, value);

                    // TODO Add `error.Locations` to `errorBuilder`.
                    resolverContext.ReportError(errorBuilder.Build());
                }
            }

            return deserializedGraphQlResponse.Data;
        }
        catch (HttpRequestException e)
        {
            _logger.FailedWithStatusCode(e, e.StatusCode, database.Locator,
                JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions));
            resolverContext.ReportError(
                ErrorBuilder.New()
                    .SetCode("DATABASE_REQUEST_FAILED")
                    .SetPath(resolverContext.Path)
                    .SetMessage(
                        $"Failed with status code {e.StatusCode} to request {database.Locator} for {JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions)}.")
                    .SetException(e)
                    .Build()
            );
            return null;
        }
        catch (JsonException e)
        {
            _logger.FailedToDeserialize(e, database.Locator,
                JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions), e.BytePositionInLine,
                e.LineNumber, e.Message, e.Path);
            resolverContext.ReportError(
                ErrorBuilder.New()
                    .SetCode("DESERIALIZATION_FAILED")
                    .SetPath(resolverContext.Path.ToList().Concat(e.Path?.Split('.') ?? Array.Empty<string>())
                        .ToList()) // TODO Splitting the path at '.' is wrong in general.
                    .SetMessage(
                        $"Failed to deserialize GraphQL response of request to {database.Locator} for {JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions)}. The details given are: Zero-based number of bytes read within the current line before the exception are {e.BytePositionInLine}, zero-based number of lines read before the exception are {e.LineNumber}, message that describes the current exception is '{e.Message}', path within the JSON where the exception was encountered is {e.Path}.")
                    .SetException(e)
                    .Build()
            );
            return null;
        }
        catch (Exception e)
        {
            _logger.FailedToRequestOrDeserialize(e, database.Locator,
                JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions));
            resolverContext.ReportError(
                ErrorBuilder.New()
                    .SetCode("DATABASE_REQUEST_FAILED")
                    .SetPath(resolverContext.Path)
                    .SetMessage(
                        $"Failed to request {database.Locator} for {JsonSerializer.Serialize(request, QueryingDatabases.SerializerOptions)} or failed to deserialize the response.")
                    .SetException(e)
                    .Build()
            );
            return null;
        }
    }

    private sealed record DataData(DataX.Data Data);
    private sealed record OpticalDataData(OpticalData OpticalData);
    private sealed record HygrothermalDataData(HygrothermalData HygrothermalData);
    private sealed record CalorimetricDataData(CalorimetricData CalorimetricData);
    private sealed record PhotovoltaicDataData(PhotovoltaicData PhotovoltaicData);
    private sealed record GeometricDataData(GeometricData GeometricData);
    private sealed record AllDataData(DataConnection AllData);
    private sealed record AllDataDataIgsdb(DataConnectionIgsdb AllData);
    private sealed record AllOpticalDataData(OpticalDataConnection AllOpticalData);
    private sealed record AllOpticalDataDataIgsdb(OpticalDataConnectionIgsdb AllOpticalData);
    private sealed record AllHygrothermalDataData(HygrothermalDataConnection AllHygrothermalData);
    private sealed record AllCalorimetricDataData(CalorimetricDataConnection AllCalorimetricData);
    private sealed record AllGeometricDataData(GeometricDataConnection AllGeometricData);
    private sealed record AllGeometricDataDataIgsdb(GeometricDataConnectionIgsdb AllGeometricData);
    private sealed record AllPhotovoltaicDataData(PhotovoltaicDataConnection AllPhotovoltaicData);
    private sealed record HasDataData(bool HasData);
    private sealed record HasOpticalDataData(bool HasOpticalData);
    private sealed record HasCalorimetricDataData(bool HasCalorimetricData);
    private sealed record HasGeometricDataData(bool HasGeometricData);
    private sealed record HasHygrothermalDataData(bool HasHygrothermalData);
    private sealed record HasPhotovoltaicDataData(bool HasPhotovoltaicData);
}