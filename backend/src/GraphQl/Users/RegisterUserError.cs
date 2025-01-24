using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class RegisterUserError(
    RegisterUserErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RegisterUserErrorCode>(code, message, path)
{
}