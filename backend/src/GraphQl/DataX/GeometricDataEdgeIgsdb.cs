namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataEdgeIgsdb
{
    public GeometricDataIgsdb Node { get; }

    public GeometricDataEdgeIgsdb(
        GeometricDataIgsdb node
    )
    {
        Node = node;
    }
}