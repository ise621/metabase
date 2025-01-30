using Metabase.Data;
using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public class ForbidRepresentativeToSignDataPayload
{
    public ForbidRepresentativeToSignDataPayload(
        User user
    )
    {
        User = user;
    }

    public ForbidRepresentativeToSignDataPayload(
        IReadOnlyCollection<ForbidRepresentativeToSignDataError> errors
    )
    {
        Errors = errors;
    }

    public ForbidRepresentativeToSignDataPayload(
        User user,
        IReadOnlyCollection<ForbidRepresentativeToSignDataError> errors
    )
    {
        User = user;
        Errors = errors;
    }

    public ForbidRepresentativeToSignDataPayload(
        ForbidRepresentativeToSignDataError error
    )
        : this([error])
    {
    }

    public User? User { get; }
    public IReadOnlyCollection<ForbidRepresentativeToSignDataError>? Errors { get; }
}