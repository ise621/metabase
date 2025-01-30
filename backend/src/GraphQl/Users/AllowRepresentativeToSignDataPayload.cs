using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Users;

public class AllowRepresentativeToSignDataPayload
{
    public AllowRepresentativeToSignDataPayload(
        User user
    )
    {
        User = user;
    }

    public AllowRepresentativeToSignDataPayload(
        IReadOnlyCollection<AllowRepresentativeToSignDataError> errors
    )
    {
        Errors = errors;
    }

    public AllowRepresentativeToSignDataPayload(
        User user,
        IReadOnlyCollection<AllowRepresentativeToSignDataError> errors
    )
    {
        User = user;
        Errors = errors;
    }

    public AllowRepresentativeToSignDataPayload(
        AllowRepresentativeToSignDataError error
    )
        : this([error])
    {
    }

    public User? User { get; }
    public IReadOnlyCollection<AllowRepresentativeToSignDataError>? Errors { get; }
}