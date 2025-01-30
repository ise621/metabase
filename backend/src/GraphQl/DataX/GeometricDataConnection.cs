using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataConnection(
    IReadOnlyList<GeometricDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<GeometricDataEdge>(
        edges,
        totalCount,
        pageInfo
        )
{
    internal static GeometricDataConnection? From(GeometricDataConnectionIgsdb? allGeometricData)
    {
        if (allGeometricData is null)
        {
            return null;
        }
        return new GeometricDataConnection(
            allGeometricData.Edges.Select(GeometricDataEdge.From).ToList().AsReadOnly(),
            Convert.ToUInt32(allGeometricData.Edges.Count),
            allGeometricData.PageInfo
        );
    }
}