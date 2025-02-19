using Metabase.Data;
using Metabase.Enumerations;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.Users;

public sealed class UserRepresentedInstitutionEdge(
    InstitutionRepresentative association
    )
        : Edge<Institution, InstitutionByIdDataLoader>(association.InstitutionId)
{
    public InstitutionRepresentativeRole Role { get; } = association.Role;
    public DataSigningPermission DataSigningPermission { get; } = association.DataSigningPermission;
}