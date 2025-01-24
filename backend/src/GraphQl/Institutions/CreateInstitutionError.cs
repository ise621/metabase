using System.Collections.Generic;

namespace Metabase.GraphQl.Institutions;

public sealed class CreateInstitutionError(
    CreateInstitutionErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateInstitutionErrorCode>(code, message, path)
{
}