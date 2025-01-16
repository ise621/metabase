using System;

namespace Metabase.GraphQl.OpenIdConnect.Tokens;

public sealed record RevokeTokenInput(
    Guid TokenId
);