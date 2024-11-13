using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedInstitutionEdge(
    Institution node
    )
{
    public Institution Node { get; } = node;
}