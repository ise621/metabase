namespace Metabase.GraphQl.DataX;

public sealed class PageInfo(
    bool hasNextPage,
    bool hasPreviousPage,
    string startCursor,
    string endCursor,
    uint count
    )
{
    public bool HasNextPage { get; } = hasNextPage;
    public bool HasPreviousPage { get; } = hasPreviousPage;
    public string StartCursor { get; } = startCursor;
    public string EndCursor { get; } = endCursor;
    public uint Count { get; } = count;
}