using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Authorizations
{
    public sealed class DeleteAuthorizationError
    : UserErrorBase<DeleteAuthorizationErrorCode>
    {
        public DeleteAuthorizationError(
            DeleteAuthorizationErrorCode code,
            string message,
            IReadOnlyList<string> path
        )
            : base(code, message, path)
        {
        }
    }
}