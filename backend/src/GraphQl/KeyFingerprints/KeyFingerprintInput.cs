using System;

namespace Metabase.GraphQl.KeyFingerprints;
public sealed record KeyFingerprintInput(
    Guid InstitutionId,
    Guid UserId,
    string KeyFingerprint
);