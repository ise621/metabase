using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResourceTree(
    GetHttpsResourceTreeRoot root,
    IReadOnlyList<GetHttpsResourceTreeNonRootVertex> nonRootVertices
    )
{
    public GetHttpsResourceTreeRoot Root { get; } = root;
    public IReadOnlyList<GetHttpsResourceTreeNonRootVertex> NonRootVertices { get; } = nonRootVertices;
}