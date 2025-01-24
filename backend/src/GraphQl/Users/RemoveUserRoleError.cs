using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class RemoveUserRoleError(
    RemoveUserRoleErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveUserRoleErrorCode>(code, message, path)
{
}