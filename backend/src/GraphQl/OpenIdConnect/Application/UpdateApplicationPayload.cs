using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class UpdateApplicationPayload
    : ApplicationPayload<UpdateApplicationError>
{
    public UpdateApplicationPayload(
        OpenIddictEntityFrameworkCoreApplication application
    )
        : base(application)
    {
    }

    public UpdateApplicationPayload(
        UpdateApplicationError error
    )
        : base(error)
    {
    }
}