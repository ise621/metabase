using System;

namespace Metabase.GraphQl.Users;

public sealed record ForbidRepresentativeToSignDataInput(
    Guid UserId,
    Guid InstitutionId
);