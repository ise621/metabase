using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionApplicationConnection
    : Connection<Institution, InstitutionApplication,
        InstitutionApplicationsByInstitutionIdDataLoader, InstitutionApplicationEdge>
{
    public InstitutionApplicationConnection(
        Institution institution
    )
        : base(
            institution,
            x => new InstitutionApplicationEdge(x)
        )
    {
    }
}