using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedDataFormatEdge(
    DataFormat node
    )
{
    public DataFormat Node { get; } = node;
}