using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ResetUserPasswordError(
    ResetUserPasswordErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ResetUserPasswordErrorCode>(code, message, path)
{
}