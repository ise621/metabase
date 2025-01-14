using System;
using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class CalorimetricDataConnection(
    IReadOnlyList<CalorimetricDataEdge> edges,
    uint totalCount,
    DateTime timestamp,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<CalorimetricDataEdge>(
        edges,
        totalCount,
        timestamp,
        pageInfo
        )
{
}