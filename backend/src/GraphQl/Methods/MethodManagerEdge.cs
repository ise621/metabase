using Metabase.Data;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.Methods;

public sealed class MethodManagerEdge(
    Method association
    )
        : Edge<Institution, InstitutionByIdDataLoader>(association.ManagerId)
{
}