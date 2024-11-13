using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class GenerateUserTwoFactorRecoveryCodesError(
    GenerateUserTwoFactorRecoveryCodesErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<GenerateUserTwoFactorRecoveryCodesErrorCode>(code, message, path)
{
}