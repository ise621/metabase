using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataConnectionIgsdb
{
    public IReadOnlyList<GeometricDataEdgeIgsdb> Edges { get; }

    public GeometricDataConnectionIgsdb(
        IReadOnlyList<GeometricDataEdgeIgsdb> edges
    )
    {
        Edges = edges;
    }
}