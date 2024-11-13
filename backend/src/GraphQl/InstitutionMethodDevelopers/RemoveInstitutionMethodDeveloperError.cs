using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class RemoveInstitutionMethodDeveloperError(
    RemoveInstitutionMethodDeveloperErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveInstitutionMethodDeveloperErrorCode>(code, message, path)
{
}