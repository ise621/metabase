using Metabase.GraphQl.OpenIdConnect.Application;
using System.Collections.Generic;

namespace Metabase.GraphQl.KeyFingerprints;

public class AddKeyFingerprintError
    : UserErrorBase<AddKeyFingerprintErrorCode>
{
    public AddKeyFingerprintError(
        AddKeyFingerprintErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}