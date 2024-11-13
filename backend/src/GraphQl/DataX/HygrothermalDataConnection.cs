using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class HygrothermalDataConnection(
    IReadOnlyList<HygrothermalDataEdge> edges,
    uint totalCount,
    DateTime timestamp
    )
        : DataConnectionBase<HygrothermalDataEdge>(
        edges,
        totalCount,
        timestamp
        )
{
}