using System;

namespace Metabase.GraphQl.DataX;

public abstract class DataIgsdb(
    string id,
    Guid? uuid,
    DateTime timestamp,
    Guid componentId,
    string? name,
    string? description,
    GetHttpsResourceTreeIgsdb resourceTree
    )
        : IDataIgsdb
{
    public string Id { get; } = id;
    public Guid? Uuid { get; } = uuid;
    public DateTime Timestamp { get; } = timestamp;
    public Guid ComponentId { get; } = componentId;
    public string? Name { get; } = name;
    public string? Description { get; } = description;
    public GetHttpsResourceTreeIgsdb ResourceTree { get; } = resourceTree;
}