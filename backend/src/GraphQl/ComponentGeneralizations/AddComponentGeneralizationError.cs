using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentGeneralizations;

public sealed class AddComponentGeneralizationError(
    AddComponentGeneralizationErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddComponentGeneralizationErrorCode>(code, message, path)
{
}