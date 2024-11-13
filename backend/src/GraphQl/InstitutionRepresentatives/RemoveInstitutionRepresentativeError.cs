using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class RemoveInstitutionRepresentativeError(
    RemoveInstitutionRepresentativeErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveInstitutionRepresentativeErrorCode>(code, message, path)
{
}