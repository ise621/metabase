using System.Collections.Generic;

namespace Metabase.GraphQl.KeyFingerprints;

public class AddKeyFingerprintPayload
{
    public AddKeyFingerprintPayload(
        string keyFingerprint
    )
    {
        KeyFingerprint = keyFingerprint;
    }

    public AddKeyFingerprintPayload(
        IReadOnlyCollection<AddKeyFingerprintError> errors
    )
    {
        Errors = errors;
    }

    public AddKeyFingerprintPayload(
        AddKeyFingerprintError error
    )
        : this([error])
    {
    }

    public string? KeyFingerprint { get; }
    public IReadOnlyCollection<AddKeyFingerprintError>? Errors { get; }
}