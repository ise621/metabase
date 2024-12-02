namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed record UpdateApplicationInput(
    string Id,
    string ClientId,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUri,
    string Permissions
);