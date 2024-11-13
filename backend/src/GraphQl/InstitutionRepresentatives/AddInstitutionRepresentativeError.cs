using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class AddInstitutionRepresentativeError(
    AddInstitutionRepresentativeErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddInstitutionRepresentativeErrorCode>(code, message, path)
{
}