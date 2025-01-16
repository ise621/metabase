using System;

namespace Metabase.GraphQl.OpenIdConnect.Authorizations;

public sealed record DeleteAuthorizationInput(
    Guid AuthorizationId
);