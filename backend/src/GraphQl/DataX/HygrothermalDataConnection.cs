using System;
using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class HygrothermalDataConnection(
    IReadOnlyList<HygrothermalDataEdge> edges,
    uint totalCount,
    DateTime timestamp,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<HygrothermalDataEdge>(
        edges,
        totalCount,
        timestamp,
        pageInfo
        )
{
}