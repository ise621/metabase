using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class ConfirmInstitutionRepresentativeError(
    ConfirmInstitutionRepresentativeErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ConfirmInstitutionRepresentativeErrorCode>(code, message, path)
{
}