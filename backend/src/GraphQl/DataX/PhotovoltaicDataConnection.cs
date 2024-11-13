using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class PhotovoltaicDataConnection(
    IReadOnlyList<PhotovoltaicDataEdge> edges,
    uint totalCount,
    DateTime timestamp
    )
        : DataConnectionBase<PhotovoltaicDataEdge>(
        edges,
        totalCount,
        timestamp
        )
{
}