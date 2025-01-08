using Metabase.Data;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.OpenIdConnect.Application
{
    public sealed class ApplicationInstitutionConnection
    : OpenIdConnectConnection<OpenIdApplication, InstitutionApplication,
        InstitutionApplicationsByInstitutionIdDataLoader, InstitutionApplicationEdge>
    {
        public ApplicationInstitutionConnection(
            OpenIdApplication application
        )
            : base(
                application,
                x => new InstitutionApplicationEdge(x)
            )
        {
        }
    }
}