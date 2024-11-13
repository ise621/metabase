using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalDataConnectionIgsdb(
    IReadOnlyList<OpticalDataEdgeIgsdb> edges
    )
{
    public IReadOnlyList<OpticalDataEdgeIgsdb> Edges { get; } = edges;
}