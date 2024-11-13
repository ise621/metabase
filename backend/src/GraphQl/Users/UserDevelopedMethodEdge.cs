using Metabase.Data;
using Metabase.GraphQl.Methods;

namespace Metabase.GraphQl.Users;

public sealed class UserDevelopedMethodEdge(
    UserMethodDeveloper association
    )
        : Edge<Method, MethodByIdDataLoader>(association.MethodId)
{
}