using System;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.Data;

public class OpenIdToken : OpenIddictEntityFrameworkCoreToken<Guid, OpenIdApplication, OpenIdAuthorization>
{
}