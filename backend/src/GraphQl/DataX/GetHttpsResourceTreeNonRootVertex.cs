namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResourceTreeNonRootVertex(
    string vertexId,
    GetHttpsResource value,
    string parentId,
    ToTreeVertexAppliedConversionMethod appliedConversionMethod
    )
        : IGetHttpsResourceTreeVertex
{
    public string VertexId { get; } = vertexId;
    public string ParentId { get; } = parentId;
    public ToTreeVertexAppliedConversionMethod AppliedConversionMethod { get; } = appliedConversionMethod;
    public GetHttpsResource Value { get; } = value;
}