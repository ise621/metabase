using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class DataConnectionIgsdb(
    IReadOnlyList<DataEdgeIgsdb> edges
    )
        : DataConnectionBase<DataEdgeIgsdb>(
        edges,
        Convert.ToUInt32(edges.Count),
        DateTime.UtcNow
        )
{
}