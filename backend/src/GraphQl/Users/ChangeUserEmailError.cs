using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ChangeUserEmailError(
    ChangeUserEmailErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ChangeUserEmailErrorCode>(code, message, path)
{
}