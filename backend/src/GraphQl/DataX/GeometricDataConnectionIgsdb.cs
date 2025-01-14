using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataConnectionIgsdb(
    IReadOnlyList<GeometricDataEdgeIgsdb> edges,
    ConnectionPageInfo pageInfo
    )
{
    public IReadOnlyList<GeometricDataEdgeIgsdb> Edges { get; } = edges;
    public ConnectionPageInfo PageInfo { get; } = pageInfo;
}