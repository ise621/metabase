using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.KeyFingerprints;

[SuppressMessage("Naming", "CA1707")]
public enum VerifyKeyFingerprintErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNKNOWN_INSTITUTION,
    UNKNOWN_USER,
    UNKNOWN_REPRESENTATIVE,
    UNKNOWN_FINGERPRINT,
}