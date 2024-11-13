using System.Collections.Generic;

namespace Metabase.GraphQl.UserMethodDevelopers;

public sealed class ConfirmUserMethodDeveloperError(
    ConfirmUserMethodDeveloperErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ConfirmUserMethodDeveloperErrorCode>(code, message, path)
{
}