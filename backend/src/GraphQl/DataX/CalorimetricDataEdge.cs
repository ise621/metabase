namespace Metabase.GraphQl.DataX;

public sealed class CalorimetricDataEdge(
    string cursor,
    CalorimetricData node
    )
        : DataEdgeBase<CalorimetricData>(
        cursor,
        node
        )
{
}