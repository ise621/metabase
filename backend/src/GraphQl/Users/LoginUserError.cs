using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class LoginUserError(
    LoginUserErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<LoginUserErrorCode>(code, message, path)
{
}