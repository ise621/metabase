using System;

namespace Metabase.GraphQl.Users;

public sealed record AllowRepresentativeToSignDataInput(
    Guid UserId,
    Guid InstitutionId
);