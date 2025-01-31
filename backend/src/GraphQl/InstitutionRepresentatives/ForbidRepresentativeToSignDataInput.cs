using System;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed record ForbidRepresentativeToSignDataInput(
    Guid UserId,
    Guid InstitutionId
);