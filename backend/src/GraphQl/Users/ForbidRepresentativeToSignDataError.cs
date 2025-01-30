using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ForbidRepresentativeToSignDataError(
    ForbidRepresentativeToSignDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ForbidRepresentativeToSignDataErrorCode>(code, message, path)
{
}