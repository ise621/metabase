using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedMethodEdge(
    Method node
    )
{
    public Method Node { get; } = node;
}