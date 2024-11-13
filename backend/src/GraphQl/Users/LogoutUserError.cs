using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class LogoutUserError(
    LogoutUserErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<LogoutUserErrorCode>(code, message, path)
{
}