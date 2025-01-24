using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalDataConnectionIgsdb(
    IReadOnlyList<OpticalDataEdgeIgsdb> edges,
    ConnectionPageInfo pageInfo
    )
{
    public IReadOnlyList<OpticalDataEdgeIgsdb> Edges { get; } = edges;
    public ConnectionPageInfo PageInfo { get; } = pageInfo;
}