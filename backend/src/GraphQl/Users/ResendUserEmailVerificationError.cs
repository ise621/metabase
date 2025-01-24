using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ResendUserEmailVerificationError(
    ResendUserEmailVerificationErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ResendUserEmailVerificationErrorCode>(code, message, path)
{
}