using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class ConfirmComponentManufacturerError(
    ConfirmComponentManufacturerErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ConfirmComponentManufacturerErrorCode>(code, message, path)
{
}