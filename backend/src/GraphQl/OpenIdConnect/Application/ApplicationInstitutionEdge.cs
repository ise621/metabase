using Metabase.Data;

namespace Metabase.GraphQl.OpenIdConnect.Application
{
    public sealed class ApplicationInstitutionEdge : Edge<OpenIdApplication, ApplicationByIdDataLoader>
    {
        public ApplicationInstitutionEdge(
            InstitutionApplication association
        )
            : base(association.InstitutionId)
        {
        }
    }
}