using Metabase.Data;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.DataFormats;

public sealed class DataFormatManagerEdge(
    DataFormat association
    )
        : Edge<Institution, InstitutionByIdDataLoader>(association.ManagerId)
{
}