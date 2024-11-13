using System.Collections.Generic;

namespace Metabase.GraphQl.DataFormats;

public sealed class CreateDataFormatError(
    CreateDataFormatErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateDataFormatErrorCode>(code, message, path)
{
}