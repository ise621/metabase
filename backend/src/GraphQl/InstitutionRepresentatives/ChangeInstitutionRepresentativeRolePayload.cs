using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class ChangeInstitutionRepresentativeRolePayload
{
    public ChangeInstitutionRepresentativeRolePayload(
        InstitutionRepresentative institutionRepresentative
    )
    {
        RepresentedInstitutionEdge = new UserRepresentedInstitutionEdge(institutionRepresentative);
        InstitutionRepresentativeEdge = new InstitutionRepresentativeEdge(institutionRepresentative);
    }

    public ChangeInstitutionRepresentativeRolePayload(
        IReadOnlyCollection<ChangeInstitutionRepresentativeRoleError> errors
    )
    {
        Errors = errors;
    }

    public ChangeInstitutionRepresentativeRolePayload(
        ChangeInstitutionRepresentativeRoleError error
    )
        : this([error])
    {
    }

    public UserRepresentedInstitutionEdge? RepresentedInstitutionEdge { get; }
    public InstitutionRepresentativeEdge? InstitutionRepresentativeEdge { get; }
    public IReadOnlyCollection<ChangeInstitutionRepresentativeRoleError>? Errors { get; }
}