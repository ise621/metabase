using System;
using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class PhotovoltaicDataConnection(
    IReadOnlyList<PhotovoltaicDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<PhotovoltaicDataEdge>(
        edges,
        totalCount,
        pageInfo
        )
{
}