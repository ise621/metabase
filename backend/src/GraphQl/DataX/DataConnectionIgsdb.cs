using System;
using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class DataConnectionIgsdb(
    IReadOnlyList<DataEdgeIgsdb> edges,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<DataEdgeIgsdb>(
        edges,
        Convert.ToUInt32(edges.Count),
        DateTime.UtcNow,
        pageInfo
        )
{
}