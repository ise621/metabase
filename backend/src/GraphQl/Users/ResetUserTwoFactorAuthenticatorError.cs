using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ResetUserTwoFactorAuthenticatorError(
    ResetUserTwoFactorAuthenticatorErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ResetUserTwoFactorAuthenticatorErrorCode>(code, message, path)
{
}