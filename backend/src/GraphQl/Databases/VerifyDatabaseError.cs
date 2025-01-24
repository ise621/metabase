using System.Collections.Generic;

namespace Metabase.GraphQl.Databases;

public sealed class VerifyDatabaseError(
    VerifyDatabaseErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<VerifyDatabaseErrorCode>(code, message, path)
{
}