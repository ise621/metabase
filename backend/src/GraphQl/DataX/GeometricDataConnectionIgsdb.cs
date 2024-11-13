using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataConnectionIgsdb(
    IReadOnlyList<GeometricDataEdgeIgsdb> edges
    )
{
    public IReadOnlyList<GeometricDataEdgeIgsdb> Edges { get; } = edges;
}