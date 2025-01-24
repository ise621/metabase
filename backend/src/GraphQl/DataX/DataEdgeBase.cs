namespace Metabase.GraphQl.DataX;

public abstract class DataEdgeBase<TData>(
    string cursor,
    TData node
    )
{
    public string Cursor { get; } = cursor;
    public TData Node { get; } = node;
}