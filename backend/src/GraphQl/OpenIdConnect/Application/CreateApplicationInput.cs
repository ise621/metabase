using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed record CreateApplicationInput(
    Guid AssociatedInstitutionId,
    string ClientId,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUri,
    IReadOnlyList<string> Permissions
);