using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class AddInstitutionMethodDeveloperError(
    AddInstitutionMethodDeveloperErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddInstitutionMethodDeveloperErrorCode>(code, message, path)
{
}