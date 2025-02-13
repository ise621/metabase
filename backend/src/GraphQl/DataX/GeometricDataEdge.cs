using System;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataEdge(
    string cursor,
    GeometricData node
    )
        : DataEdgeBase<GeometricData>(
        cursor,
        node
        )
{
}