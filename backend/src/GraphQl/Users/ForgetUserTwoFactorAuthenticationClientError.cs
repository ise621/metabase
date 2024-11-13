using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class ForgetUserTwoFactorAuthenticationClientError(
    ForgetUserTwoFactorAuthenticationClientErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ForgetUserTwoFactorAuthenticationClientErrorCode>(code, message, path)
{
}