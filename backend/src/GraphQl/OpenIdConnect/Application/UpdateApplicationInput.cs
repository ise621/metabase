using System;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed record UpdateApplicationInput(
    Guid ApplicationId,
    string ClientId,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUri,
    string Permissions
);