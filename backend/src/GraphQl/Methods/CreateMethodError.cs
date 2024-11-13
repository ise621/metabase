using System.Collections.Generic;

namespace Metabase.GraphQl.Methods;

public sealed class CreateMethodError(
    CreateMethodErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateMethodErrorCode>(code, message, path)
{
}