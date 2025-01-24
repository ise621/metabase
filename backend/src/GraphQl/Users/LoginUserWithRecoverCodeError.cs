using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class LoginUserWithRecoveryCodeError(
    LoginUserWithRecoveryCodeErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<LoginUserWithRecoveryCodeErrorCode>(code, message, path)
{
}