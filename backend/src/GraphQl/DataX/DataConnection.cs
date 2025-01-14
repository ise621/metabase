using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class DataConnection(
    IReadOnlyList<DataEdge> edges,
    uint totalCount,
    DateTime timestamp,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<DataEdge>(
        edges,
        totalCount,
        timestamp,
        pageInfo
        )
{
    internal static DataConnection? From(DataConnectionIgsdb? connection)
    {
        if (connection is null)
        {
            return null;
        }
        return new DataConnection(
            connection.Edges.Select(DataEdge.From).ToList().AsReadOnly(),
            connection.TotalCount,
            connection.Timestamp,
            connection.PageInfo
        );
    }
}