using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Users;

public sealed class AddUserRolePayload
{
    public AddUserRolePayload(
        User user
    )
    {
        User = user;
    }

    public AddUserRolePayload(
        IReadOnlyCollection<AddUserRoleError> errors
    )
    {
        Errors = errors;
    }

    public AddUserRolePayload(
        User user,
        IReadOnlyCollection<AddUserRoleError> errors
    )
    {
        User = user;
        Errors = errors;
    }

    public AddUserRolePayload(
        AddUserRoleError error
    )
        : this([error])
    {
    }

    public User? User { get; }
    public IReadOnlyCollection<AddUserRoleError>? Errors { get; }
}