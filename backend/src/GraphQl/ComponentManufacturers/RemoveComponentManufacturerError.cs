using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class RemoveComponentManufacturerError(
    RemoveComponentManufacturerErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveComponentManufacturerErrorCode>(code, message, path)
{
}