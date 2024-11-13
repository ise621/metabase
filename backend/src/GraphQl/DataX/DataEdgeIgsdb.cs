using System;

namespace Metabase.GraphQl.DataX;

public sealed class DataEdgeIgsdb(
    IDataIgsdb node
    )
        : DataEdgeBase<IDataIgsdb>(
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(node.Id)),
        node
        )
{
}