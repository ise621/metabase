using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalDataConnection(
    IReadOnlyList<OpticalDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<OpticalDataEdge>(
        edges,
        totalCount,
        pageInfo
        )
{
}