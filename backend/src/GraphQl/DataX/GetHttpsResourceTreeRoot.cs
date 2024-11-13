using System;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResourceTreeRoot(
    string vertexId,
    GetHttpsResource value
    ) : IGetHttpsResourceTreeVertex
{
    internal static GetHttpsResourceTreeRoot From(GetHttpsResourceTreeRootIgsdb root)
    {
        return new GetHttpsResourceTreeRoot(
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("root")),
            GetHttpsResource.From(root.Value)
        );
    }

    public string VertexId { get; } = vertexId;
    public GetHttpsResource Value { get; } = value;
}