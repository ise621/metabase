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
    internal static GeometricDataEdge From(GeometricDataEdgeIgsdb edge)
    {
        return new GeometricDataEdge(
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(edge.Node.Id)),
            GeometricData.From(edge.Node)
        );
    }
}