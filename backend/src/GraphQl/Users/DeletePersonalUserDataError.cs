using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class DeletePersonalUserDataError(
    DeletePersonalUserDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<DeletePersonalUserDataErrorCode>(code, message, path)
{
}