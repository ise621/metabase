using System.Collections.Generic;

namespace Metabase.GraphQl.KeyFingerprints;

public class VerifyKeyFingerprintPayload
{
    public VerifyKeyFingerprintPayload(
        bool valid
    )
    {
        Valid = valid;
    }

    public VerifyKeyFingerprintPayload(
        IReadOnlyCollection<VerifyKeyFingerprintError> errors
    )
    {
        Errors = errors;
    }

    public VerifyKeyFingerprintPayload(
        VerifyKeyFingerprintError error
    )
        : this([error])
    {
    }

    public bool? Valid { get; }
    public IReadOnlyCollection<VerifyKeyFingerprintError>? Errors { get; }
}