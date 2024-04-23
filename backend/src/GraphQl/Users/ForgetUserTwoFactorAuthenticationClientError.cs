using System.Collections.Generic;

namespace Metabase.GraphQl.Users
{
    public sealed class ForgetUserTwoFactorAuthenticationClientError
        : GraphQl.UserErrorBase<ForgetUserTwoFactorAuthenticationClientErrorCode>
    {
        public ForgetUserTwoFactorAuthenticationClientError(
            ForgetUserTwoFactorAuthenticationClientErrorCode code,
            string message,
            IReadOnlyList<string> path
        )
            : base(code, message, path)
        {
        }
    }
}