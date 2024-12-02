using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Tokens;

public sealed class RevokeTokenPayload
{
    public RevokeTokenPayload()
    {
    }

    public RevokeTokenPayload(
        RevokeTokenError error
    )
    {
        Errors = new[] { error };
    }

    public IReadOnlyCollection<RevokeTokenError>? Errors { get; }
}