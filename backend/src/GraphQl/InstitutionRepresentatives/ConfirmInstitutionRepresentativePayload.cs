using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class ConfirmInstitutionRepresentativePayload
{
    public ConfirmInstitutionRepresentativePayload(
        InstitutionRepresentative institutionRepresentative
    )
    {
        RepresentedInstitutionEdge = new UserRepresentedInstitutionEdge(institutionRepresentative);
        InstitutionRepresentativeEdge = new InstitutionRepresentativeEdge(institutionRepresentative);
    }

    public ConfirmInstitutionRepresentativePayload(
        IReadOnlyCollection<ConfirmInstitutionRepresentativeError> errors
    )
    {
        Errors = errors;
    }

    public ConfirmInstitutionRepresentativePayload(
        ConfirmInstitutionRepresentativeError error
    )
        : this([error])
    {
    }

    public UserRepresentedInstitutionEdge? RepresentedInstitutionEdge { get; }
    public InstitutionRepresentativeEdge? InstitutionRepresentativeEdge { get; }
    public IReadOnlyCollection<ConfirmInstitutionRepresentativeError>? Errors { get; }
}