namespace Metabase.GraphQl.DataX;

public sealed class DataEdge(
    string cursor,
    IData node
    )
        : DataEdgeBase<IData>(
        cursor,
        node
        )
{
    internal static DataEdge From(DataEdgeIgsdb edge)
    {
        return new DataEdge(
            edge.Cursor,
            OpticalData.From((OpticalDataIgsdb)edge.Node)
        );
    }
}