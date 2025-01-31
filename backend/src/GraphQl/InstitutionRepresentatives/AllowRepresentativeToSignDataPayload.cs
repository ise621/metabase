using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public class AllowRepresentativeToSignDataPayload
{
    public AllowRepresentativeToSignDataPayload(
        InstitutionRepresentative institutionRepresentative
    )
    {
        InstitutionRepresentative = institutionRepresentative;
    }

    public AllowRepresentativeToSignDataPayload(
        IReadOnlyCollection<AllowRepresentativeToSignDataError> errors
    )
    {
        Errors = errors;
    }

    public AllowRepresentativeToSignDataPayload(
        InstitutionRepresentative institutionRepresentative,
        IReadOnlyCollection<AllowRepresentativeToSignDataError> errors
    )
    {
        InstitutionRepresentative = institutionRepresentative;
        Errors = errors;
    }

    public AllowRepresentativeToSignDataPayload(
        AllowRepresentativeToSignDataError error
    )
        : this([error])
    {
    }

    public InstitutionRepresentative? InstitutionRepresentative { get; }
    public IReadOnlyCollection<AllowRepresentativeToSignDataError>? Errors { get; }
}