using System;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResourceTreeRoot(
    string vertexId,
    GetHttpsResource value
    ) : IGetHttpsResourceTreeVertex
{
    public string VertexId { get; } = vertexId;
    public GetHttpsResource Value { get; } = value;
}