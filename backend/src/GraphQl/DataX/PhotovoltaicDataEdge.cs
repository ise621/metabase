namespace Metabase.GraphQl.DataX;

public sealed class PhotovoltaicDataEdge(
    string cursor,
    PhotovoltaicData node
    )
        : DataEdgeBase<PhotovoltaicData>(
        cursor,
        node
        )
{
}