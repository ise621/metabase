using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResourceTree(
    GetHttpsResourceTreeRoot root,
    IReadOnlyList<GetHttpsResourceTreeNonRootVertex> nonRootVertices
    )
{
    internal static GetHttpsResourceTree From(GetHttpsResourceTreeIgsdb resourceTree)
    {
        return new GetHttpsResourceTree(
            GetHttpsResourceTreeRoot.From(resourceTree.Root),
            Array.Empty<GetHttpsResourceTreeNonRootVertex>().AsReadOnly()
        );
    }

    public GetHttpsResourceTreeRoot Root { get; } = root;
    public IReadOnlyList<GetHttpsResourceTreeNonRootVertex> NonRootVertices { get; } = nonRootVertices;
}