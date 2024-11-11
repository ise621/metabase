using System;
using System.Collections.Generic;
using System.Linq;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataConnection
    : DataConnectionBase<GeometricDataEdge>
{
    internal static GeometricDataConnection? From(GeometricDataConnectionIgsdb? allGeometricData)
    {
        if (allGeometricData is null)
        {
            return null;
        }
        return new GeometricDataConnection(
            allGeometricData.Edges.Select(GeometricDataEdge.From).ToList().AsReadOnly(),
            Convert.ToUInt32(allGeometricData.Edges.Count),
            DateTime.UtcNow
        );
    }

    public GeometricDataConnection(
        IReadOnlyList<GeometricDataEdge> edges,
        uint totalCount,
        DateTime timestamp
    )
        : base(
            edges,
            totalCount,
            timestamp
        )
    {
    }
}