using System;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed record DeleteApplicationInput(
    Guid ApplicationId
);