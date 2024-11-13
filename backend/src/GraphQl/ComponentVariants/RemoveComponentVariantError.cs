using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentVariants;

public sealed class RemoveComponentVariantError(
    RemoveComponentVariantErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveComponentVariantErrorCode>(code, message, path)
{
}