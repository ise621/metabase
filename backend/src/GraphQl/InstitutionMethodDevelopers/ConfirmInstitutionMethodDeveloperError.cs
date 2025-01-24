using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class ConfirmInstitutionMethodDeveloperError(
    ConfirmInstitutionMethodDeveloperErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ConfirmInstitutionMethodDeveloperErrorCode>(code, message, path)
{
}