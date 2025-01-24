using System.Collections.Generic;

namespace Metabase.GraphQl.Components;

public sealed class UpdateComponentError(
    UpdateComponentErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateComponentErrorCode>(code, message, path)
{
}