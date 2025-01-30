using System;
using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Metabase.GraphQl.DataX;

public sealed class HygrothermalDataConnection(
    IReadOnlyList<HygrothermalDataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
    )
        : DataConnectionBase<HygrothermalDataEdge>(
        edges,
        totalCount,
        pageInfo
        )
{
}