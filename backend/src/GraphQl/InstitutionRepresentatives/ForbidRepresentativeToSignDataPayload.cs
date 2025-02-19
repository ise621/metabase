using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public class ForbidRepresentativeToSignDataPayload
{
    public ForbidRepresentativeToSignDataPayload(
        InstitutionRepresentative institutionRepresentative
    )
    {
        InstitutionRepresentative = institutionRepresentative;
    }

    public ForbidRepresentativeToSignDataPayload(
        IReadOnlyCollection<ForbidRepresentativeToSignDataError> errors
    )
    {
        Errors = errors;
    }

    public ForbidRepresentativeToSignDataPayload(
        InstitutionRepresentative institutionRepresentative,
        IReadOnlyCollection<ForbidRepresentativeToSignDataError> errors
    )
    {
        InstitutionRepresentative = institutionRepresentative;
        Errors = errors;
    }

    public ForbidRepresentativeToSignDataPayload(
        ForbidRepresentativeToSignDataError error
    )
        : this([error])
    {
    }

    public InstitutionRepresentative? InstitutionRepresentative { get; }
    public IReadOnlyCollection<ForbidRepresentativeToSignDataError>? Errors { get; }
}