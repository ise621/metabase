using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class RequestUserPasswordResetError(
    RequestUserPasswordResetErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RequestUserPasswordResetErrorCode>(code, message, path)
{
}