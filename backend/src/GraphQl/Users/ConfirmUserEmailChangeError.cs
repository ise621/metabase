using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ConfirmUserEmailChangeError(
    ConfirmUserEmailChangeErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ConfirmUserEmailChangeErrorCode>(code, message, path)
{
}