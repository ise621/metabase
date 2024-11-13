using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class SetUserPasswordError(
    SetUserPasswordErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<SetUserPasswordErrorCode>(code, message, path)
{
}