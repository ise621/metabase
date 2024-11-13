using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentVariants;

public sealed class AddComponentVariantError(
    AddComponentVariantErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddComponentVariantErrorCode>(code, message, path)
{
}