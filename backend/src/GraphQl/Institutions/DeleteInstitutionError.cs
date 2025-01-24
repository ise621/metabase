using System.Collections.Generic;

namespace Metabase.GraphQl.Institutions;

public sealed class DeleteInstitutionError(
    DeleteInstitutionErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<DeleteInstitutionErrorCode>(code, message, path)
{
}