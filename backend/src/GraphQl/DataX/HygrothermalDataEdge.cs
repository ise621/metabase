namespace Metabase.GraphQl.DataX;

public sealed class HygrothermalDataEdge(
    string cursor,
    HygrothermalData node
    )
        : DataEdgeBase<HygrothermalData>(
        cursor,
        node
        )
{
}