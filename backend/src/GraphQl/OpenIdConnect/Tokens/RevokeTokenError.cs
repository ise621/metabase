using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Tokens
{
    public sealed class RevokeTokenError
    : UserErrorBase<RevokeTokenErrorCode>
    {
        public RevokeTokenError(
            RevokeTokenErrorCode code,
            string message,
            IReadOnlyList<string> path
        )
            : base(code, message, path)
        {
        }
    }
}