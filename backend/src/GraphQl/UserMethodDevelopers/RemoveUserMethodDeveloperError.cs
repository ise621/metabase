using System.Collections.Generic;

namespace Metabase.GraphQl.UserMethodDevelopers;

public sealed class RemoveUserMethodDeveloperError(
    RemoveUserMethodDeveloperErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveUserMethodDeveloperErrorCode>(code, message, path)
{
}