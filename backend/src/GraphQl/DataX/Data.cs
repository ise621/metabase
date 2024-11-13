using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Components;
using Metabase.GraphQl.Databases;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.DataX;

public abstract class Data(
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
        : IData
{
    protected const string IgsdbLocale = "en-US";
    protected const string IgsdbDatabaseId = "48994b60-670d-488d-aaf7-53333a64f1d6";
    protected const string IgsdbInstitutionId = "c17af5ef-2f1d-4c73-bcc9-fcfb722420f3";
    protected const string IgsdbMethodId = "35e98d58-9627-4bdf-bf9f-f265471c1f24";

    public string Id { get; } = id;

    public string Locale { get; } = locale;
    public IReadOnlyList<string> Warnings { get; } = warnings;
    public Guid CreatorId { get; } = creatorId;
    public DateTime CreatedAt { get; } = createdAt;
    public IReadOnlyList<GetHttpsResource> Resources { get; } = resources;
    public Guid Uuid { get; } = uuid;
    public DateTime Timestamp { get; } = timestamp;
    public Guid DatabaseId { get; } = databaseId;
    public Guid ComponentId { get; } = componentId;
    public string? Name { get; } = name;
    public string? Description { get; } = description;
    public AppliedMethod AppliedMethod { get; } = appliedMethod;
    public GetHttpsResourceTree ResourceTree { get; } = resourceTree;

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