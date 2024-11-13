using System;

namespace Metabase.GraphQl.DataX;

public sealed class OpenEndedDateTimeRange(
    DateTime from,
    DateTime until
    )
{
    public DateTime From { get; } = from;
    public DateTime Until { get; } = until;
}