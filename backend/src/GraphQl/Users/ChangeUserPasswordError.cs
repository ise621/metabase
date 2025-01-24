using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ChangeUserPasswordError(
    ChangeUserPasswordErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ChangeUserPasswordErrorCode>(code, message, path)
{
}