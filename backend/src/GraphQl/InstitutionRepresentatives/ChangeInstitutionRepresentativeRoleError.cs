using System.Collections.Generic;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class ChangeInstitutionRepresentativeRoleError(
    ChangeInstitutionRepresentativeRoleErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<ChangeInstitutionRepresentativeRoleErrorCode>(code, message, path)
{
}