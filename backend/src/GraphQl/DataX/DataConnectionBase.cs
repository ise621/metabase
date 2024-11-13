using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;

namespace Metabase.GraphQl.DataX;

public abstract class DataConnectionBase<TDataEdge>(
    IReadOnlyList<TDataEdge> edges,
    uint totalCount,
    DateTime timestamp
    )
{
    public IReadOnlyList<TDataEdge> Edges { get; } = edges;

    [GraphQLType<NonNegativeIntType>] public uint TotalCount { get; } = totalCount;

    // public PageInfo PageInfo { get; } // TODO Resolve clash with `PageInfo` provided by `HotChocolate` 

    public DateTime Timestamp { get; } = timestamp;
}