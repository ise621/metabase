using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public abstract class DataConnectionBase<TDataEdge>(
    IReadOnlyList<TDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
{
    public IReadOnlyList<TDataEdge> Edges { get; } = edges;

    [GraphQLType<NonNegativeIntType>] public uint TotalCount { get; } = totalCount;

    public ConnectionPageInfo PageInfo { get; } = pageInfo;
}