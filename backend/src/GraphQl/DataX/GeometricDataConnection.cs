using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataConnection(
    IReadOnlyList<GeometricDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<GeometricDataEdge>(
        edges,
        totalCount,
        pageInfo
        )
{
}