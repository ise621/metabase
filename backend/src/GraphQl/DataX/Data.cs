using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Components;
using Metabase.GraphQl.Databases;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.DataX;

public abstract class Data
    : IData
{
    protected const string IgsdbLocale = "en-US";
    protected const string IgsdbDatabaseId = "48994b60-670d-488d-aaf7-53333a64f1d6";
    protected const string IgsdbInstitutionId = "c17af5ef-2f1d-4c73-bcc9-fcfb722420f3";
    protected const string IgsdbMethodId = "35e98d58-9627-4bdf-bf9f-f265471c1f24";

    // public IReadOnlyList<DataApproval> Approvals { get; }
    // public ResponseApproval Approval { get; }

    // [GraphQLIgnore]
    // [JsonExtensionData]
    // public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    protected Data(
        string id,
        Guid uuid,
        DateTime timestamp,
        string locale,
        Guid databaseId,
        Guid componentId,
        string? name,
        string? description,
        IReadOnlyList<string> warnings,
        Guid creatorId,
        DateTime createdAt,
        AppliedMethod appliedMethod,
        IReadOnlyList<GetHttpsResource> resources,
        GetHttpsResourceTree resourceTree
        // IReadOnlyList<DataApproval> approvals
        // ResponseApproval approval
    )
    {
        Id = id;
        Uuid = uuid;
        Timestamp = timestamp;
        Locale = locale;
        DatabaseId = databaseId;
        ComponentId = componentId;
        Name = name;
        Description = description;
        Warnings = warnings;
        CreatorId = creatorId;
        CreatedAt = createdAt;
        AppliedMethod = appliedMethod;
        Resources = resources;
        ResourceTree = resourceTree;
        // Approvals = approvals;
        // Approval = approval;
    }

    public string Id { get; }

    public string Locale { get; }
    public IReadOnlyList<string> Warnings { get; }
    public Guid CreatorId { get; }
    public DateTime CreatedAt { get; }
    public IReadOnlyList<GetHttpsResource> Resources { get; }
    public Guid Uuid { get; }
    public DateTime Timestamp { get; }
    public Guid DatabaseId { get; }
    public Guid ComponentId { get; }
    public string? Name { get; }
    public string? Description { get; }
    public AppliedMethod AppliedMethod { get; }
    public GetHttpsResourceTree ResourceTree { get; }

    public Task<Database?> GetDatabaseAsync(
            DatabaseByIdDataLoader databaseById,
            CancellationToken cancellationToken
    )
    {
        return databaseById.LoadAsync(
            DatabaseId,
            cancellationToken
            );
    }

    public Task<Component?> GetComponentAsync(
        ComponentByIdDataLoader componentById,
        CancellationToken cancellationToken
    )
    {
        return componentById.LoadAsync(
            ComponentId,
            cancellationToken
        );
    }

    public Task<Institution?> GetCreatorAsync(
        InstitutionByIdDataLoader institutionById,
        CancellationToken cancellationToken
    )
    {
        return institutionById.LoadAsync(
            CreatorId,
            cancellationToken
        );
    }
}