namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed record CreateApplicationInput(
    string ClientId,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUri,
    string Permissions
);