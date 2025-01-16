using System;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.Data;

public class OpenIdAuthorization : OpenIddictEntityFrameworkCoreAuthorization<Guid, OpenIdApplication, OpenIdToken>
{
}