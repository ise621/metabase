using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class SwitchInstitutionOperatingStatePayload
{
    public SwitchInstitutionOperatingStatePayload(
        Institution institution
    )
    {
        Institution = institution;
    }

    public SwitchInstitutionOperatingStatePayload(
        IReadOnlyCollection<SwitchInstitutionOperatingStateError> errors
    )
    {
        Errors = errors;
    }

    public SwitchInstitutionOperatingStatePayload(
        SwitchInstitutionOperatingStateError error
    )
        : this([error])
    {
    }

    public Institution? Institution { get; }
    public IReadOnlyCollection<SwitchInstitutionOperatingStateError>? Errors { get; }
}