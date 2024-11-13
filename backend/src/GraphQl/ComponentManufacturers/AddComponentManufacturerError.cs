using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class AddComponentManufacturerError(
    AddComponentManufacturerErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddComponentManufacturerErrorCode>(code, message, path)
{
}