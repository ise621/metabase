using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class DisableUserTwoFactorAuthenticationError(
    DisableUserTwoFactorAuthenticationErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<DisableUserTwoFactorAuthenticationErrorCode>(code, message, path)
{
}