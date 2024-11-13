using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentGeneralizations;

public sealed class RemoveComponentGeneralizationError(
    RemoveComponentGeneralizationErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveComponentGeneralizationErrorCode>(code, message, path)
{
}