using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class AddUserRoleError(
    AddUserRoleErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddUserRoleErrorCode>(code, message, path)
{
}