using System;
using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagerEdge(
    Institution association
    )
        : Edge<Institution, InstitutionByIdDataLoader>(association.ManagerId ?? Guid.Empty)
{
}