using System.Collections.Generic;

namespace Metabase.GraphQl.KeyFingerprints;

public class VerifyKeyFingerprintError
    : UserErrorBase<VerifyKeyFingerprintErrorCode>
{
    public VerifyKeyFingerprintError(
        VerifyKeyFingerprintErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}