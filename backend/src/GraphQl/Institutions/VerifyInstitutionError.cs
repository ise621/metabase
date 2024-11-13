using System.Collections.Generic;

namespace Metabase.GraphQl.Institutions;

public sealed class VerifyInstitutionError(
    VerifyInstitutionErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<VerifyInstitutionErrorCode>(code, message, path)
{
}