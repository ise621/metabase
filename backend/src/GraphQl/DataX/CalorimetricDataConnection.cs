using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class CalorimetricDataConnection(
    IReadOnlyList<CalorimetricDataEdge> edges,
    uint totalCount,
    DateTime timestamp
    )
        : DataConnectionBase<CalorimetricDataEdge>(
        edges,
        totalCount,
        timestamp
        )
{
}