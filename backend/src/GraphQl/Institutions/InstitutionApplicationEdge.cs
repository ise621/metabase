using Metabase.Data;

namespace Metabase.GraphQl.Institutions
{
    public sealed class InstitutionApplicationEdge : Edge<Institution, InstitutionByIdDataLoader>
    {
        public InstitutionApplicationEdge(
            InstitutionApplication association
        )
            : base(association.ApplicationId)
        {
        }
    }
}