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
}