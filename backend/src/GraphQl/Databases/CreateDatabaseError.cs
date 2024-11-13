using System.Collections.Generic;

namespace Metabase.GraphQl.Databases;

public sealed class CreateDatabaseError(
    CreateDatabaseErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateDatabaseErrorCode>(code, message, path)
{
}