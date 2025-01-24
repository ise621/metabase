using System.Collections.Generic;

namespace Metabase.GraphQl.Components;

public sealed class CreateComponentError(
    CreateComponentErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateComponentErrorCode>(code, message, path)
{
}