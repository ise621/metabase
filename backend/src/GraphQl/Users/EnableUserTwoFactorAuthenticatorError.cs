using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class EnableUserTwoFactorAuthenticatorError(
    EnableUserTwoFactorAuthenticatorErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<EnableUserTwoFactorAuthenticatorErrorCode>(code, message, path)
{
}