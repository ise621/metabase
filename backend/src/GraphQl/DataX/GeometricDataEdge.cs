using System;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataEdge
    : DataEdgeBase<GeometricData>
{
    internal static GeometricDataEdge From(GeometricDataEdgeIgsdb edge)
    {
        return new GeometricDataEdge(
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(edge.Node.Id)),
            GeometricData.From(edge.Node)
        );
    }

    public GeometricDataEdge(
        string cursor,
        GeometricData node
    )
        : base(
            cursor,
            node
        )
    {
    }
}