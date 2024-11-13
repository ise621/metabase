using System.Collections.Generic;

namespace Metabase.GraphQl.Institutions;

public sealed class SwitchInstitutionOperatingStateError(
    SwitchInstitutionOperatingStateErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<SwitchInstitutionOperatingStateErrorCode>(code, message, path)
{
}