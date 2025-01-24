using System.Collections.Generic;

namespace Metabase.GraphQl.Methods;

public sealed class UpdateMethodError(
    UpdateMethodErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateMethodErrorCode>(code, message, path)
{
}