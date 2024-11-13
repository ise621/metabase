using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class AddInstitutionRepresentativePayload
{
    public AddInstitutionRepresentativePayload(
        InstitutionRepresentative institutionRepresentative
    )
    {
        RepresentedInstitutionEdge = new UserRepresentedInstitutionEdge(institutionRepresentative);
        InstitutionRepresentativeEdge = new InstitutionRepresentativeEdge(institutionRepresentative);
    }

    public AddInstitutionRepresentativePayload(
        IReadOnlyCollection<AddInstitutionRepresentativeError> errors
    )
    {
        Errors = errors;
    }

    public AddInstitutionRepresentativePayload(
        AddInstitutionRepresentativeError error
    )
        : this([error])
    {
    }

    public UserRepresentedInstitutionEdge? RepresentedInstitutionEdge { get; }
    public InstitutionRepresentativeEdge? InstitutionRepresentativeEdge { get; }
    public IReadOnlyCollection<AddInstitutionRepresentativeError>? Errors { get; }
}