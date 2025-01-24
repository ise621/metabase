using System.Collections.Generic;

namespace Metabase.GraphQl.Institutions;

public sealed class UpdateInstitutionError(
    UpdateInstitutionErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateInstitutionErrorCode>(code, message, path)
{
}