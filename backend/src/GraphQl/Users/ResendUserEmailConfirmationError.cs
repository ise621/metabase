using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ResendUserEmailConfirmationError(
    ResendUserEmailConfirmationErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ResendUserEmailConfirmationErrorCode>(code, message, path)
{
}