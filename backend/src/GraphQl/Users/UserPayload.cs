using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Users;

public abstract class UserPayload<TUserError>
    : Payload
    where TUserError : IUserError
{
    protected UserPayload(
        User user
    )
    {
        User = user;
    }

    protected UserPayload(
        IReadOnlyCollection<TUserError> errors
    )
    {
        Errors = errors;
    }

    protected UserPayload(
        TUserError error
    )
        : this([error])
    {
    }

    protected UserPayload(
        User user,
        IReadOnlyCollection<TUserError> errors
    )
    {
        User = user;
        Errors = errors;
    }

    protected UserPayload(
        User user,
        TUserError error
    )
        : this(user, [error])
    {
    }

    public User? User { get; }
    public IReadOnlyCollection<TUserError>? Errors { get; }
}