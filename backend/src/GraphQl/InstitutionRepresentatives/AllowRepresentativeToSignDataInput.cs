using System;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed record AllowRepresentativeToSignDataInput(
    Guid UserId,
    Guid InstitutionId
);