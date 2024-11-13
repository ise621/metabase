using Metabase.Data;
using Metabase.Enumerations;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionRepresentativeEdge(
    InstitutionRepresentative association
    )
        : Edge<User, UserByIdDataLoader>(association.UserId)
{
    public InstitutionRepresentativeRole Role { get; } = association.Role;
}