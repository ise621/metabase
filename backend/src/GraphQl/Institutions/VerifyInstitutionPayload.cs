using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class VerifyInstitutionPayload
{
    public VerifyInstitutionPayload(
        Institution institution
    )
    {
        Institution = institution;
    }

    public VerifyInstitutionPayload(
        IReadOnlyCollection<VerifyInstitutionError> errors
    )
    {
        Errors = errors;
    }

    public VerifyInstitutionPayload(
        VerifyInstitutionError error
    )
        : this([error])
    {
    }

    public Institution? Institution { get; }
    public IReadOnlyCollection<VerifyInstitutionError>? Errors { get; }
}