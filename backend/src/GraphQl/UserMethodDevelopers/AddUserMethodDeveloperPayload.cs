using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Methods;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.UserMethodDevelopers;

public sealed class AddUserMethodDeveloperPayload
{
    public AddUserMethodDeveloperPayload(
        UserMethodDeveloper userMethodDeveloper
    )
    {
        DevelopedMethodEdge = new UserDevelopedMethodEdge(userMethodDeveloper);
        MethodDeveloperEdge = new UserMethodDeveloperEdge(userMethodDeveloper);
    }

    public AddUserMethodDeveloperPayload(
        IReadOnlyCollection<AddUserMethodDeveloperError> errors
    )
    {
        Errors = errors;
    }

    public AddUserMethodDeveloperPayload(
        AddUserMethodDeveloperError error
    )
        : this([error])
    {
    }

    public UserDevelopedMethodEdge? DevelopedMethodEdge { get; }
    public UserMethodDeveloperEdge? MethodDeveloperEdge { get; }
    public IReadOnlyCollection<AddUserMethodDeveloperError>? Errors { get; }
}