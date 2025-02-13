using System;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalDataEdge(
    string cursor,
    OpticalData node
    )
        : DataEdgeBase<OpticalData>(
        cursor,
        node
        )
{
}