using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class LoginUserWithTwoFactorCodeError(
    LoginUserWithTwoFactorCodeErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<LoginUserWithTwoFactorCodeErrorCode>(code, message, path)
{
}