using System;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed record CreateApplicationInput(
    Guid AssociatedInstitutionId,
    string ClientId,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUri,
    string Permissions
);