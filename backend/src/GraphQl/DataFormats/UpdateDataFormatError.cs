using System.Collections.Generic;

namespace Metabase.GraphQl.DataFormats;

public sealed class UpdateDataFormatError(
    UpdateDataFormatErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateDataFormatErrorCode>(code, message, path)
{
}