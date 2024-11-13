using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Users;

public sealed class RemoveUserRolePayload
{
    public RemoveUserRolePayload(
        User user
    )
    {
        User = user;
    }

    public RemoveUserRolePayload(
        IReadOnlyCollection<RemoveUserRoleError> errors
    )
    {
        Errors = errors;
    }

    public RemoveUserRolePayload(
        User user,
        IReadOnlyCollection<RemoveUserRoleError> errors
    )
    {
        User = user;
        Errors = errors;
    }

    public RemoveUserRolePayload(
        RemoveUserRoleError error
    )
        : this([error])
    {
    }

    public User? User { get; }
    public IReadOnlyCollection<RemoveUserRoleError>? Errors { get; }
}