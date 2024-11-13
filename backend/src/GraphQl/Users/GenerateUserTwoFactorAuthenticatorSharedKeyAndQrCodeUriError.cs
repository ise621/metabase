using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriError(
    GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<GenerateUserTwoFactorAuthenticatorSharedKeyAndQrCodeUriErrorCode>(code, message, path)
{
}