using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalDataConnection(
    IReadOnlyList<OpticalDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<OpticalDataEdge>(
        edges,
        totalCount,
        pageInfo
        )
{
    internal static OpticalDataConnection? From(OpticalDataConnectionIgsdb? allOpticalData)
    {
        if (allOpticalData is null)
        {
            return null;
        }
        return new OpticalDataConnection(
            allOpticalData.Edges.Select(OpticalDataEdge.From).ToList().AsReadOnly(),
            Convert.ToUInt32(allOpticalData.Edges.Count),
            allOpticalData.PageInfo
        );
    }
}