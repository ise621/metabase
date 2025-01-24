using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ConfirmUserEmailError(
    ConfirmUserEmailErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ConfirmUserEmailErrorCode>(code, message, path)
{
}