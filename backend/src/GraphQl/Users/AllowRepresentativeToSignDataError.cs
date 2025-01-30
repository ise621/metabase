using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class AllowRepresentativeToSignDataError(
    AllowRepresentativeToSignDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AllowRepresentativeToSignDataErrorCode>(code, message, path)
{
}