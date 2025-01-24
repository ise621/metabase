using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionOperatedDatabaseEdge(
    Database node
    )
{
    public Database Node { get; } = node;
}