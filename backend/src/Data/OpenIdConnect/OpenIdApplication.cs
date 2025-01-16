using System;
using System.Collections.Generic;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.Data;

public class OpenIdApplication : OpenIddictEntityFrameworkCoreApplication<Guid, OpenIdAuthorization, OpenIdToken>
{
    public ICollection<InstitutionApplication> InstitutionEdges { get; } =
        new List<InstitutionApplication>();

    public ICollection<Institution> Institutions { get; } = new List<Institution>();
}