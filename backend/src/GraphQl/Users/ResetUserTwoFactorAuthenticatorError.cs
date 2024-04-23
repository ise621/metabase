using System.Collections.Generic;

namespace Metabase.GraphQl.Users
{
    public sealed class ResetUserTwoFactorAuthenticatorError
        : GraphQl.UserErrorBase<ResetUserTwoFactorAuthenticatorErrorCode>
    {
        public ResetUserTwoFactorAuthenticatorError(
            ResetUserTwoFactorAuthenticatorErrorCode code,
            string message,
            IReadOnlyList<string> path
        )
            : base(code, message, path)
        {
        }
    }
}