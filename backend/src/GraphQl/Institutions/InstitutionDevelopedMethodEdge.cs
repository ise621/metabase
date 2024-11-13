using Metabase.Data;
using Metabase.GraphQl.Methods;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionDevelopedMethodEdge(
    InstitutionMethodDeveloper association
    )
        : Edge<Method, MethodByIdDataLoader>(association.MethodId)
{
}