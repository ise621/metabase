using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class CreateApplicationPayload
    : ApplicationPayload<CreateApplicationError>
{
    public CreateApplicationPayload(
        OpenIddictEntityFrameworkCoreApplication application
    )
        : base(application)
    {
    }

    public CreateApplicationPayload(
        CreateApplicationError error
    )
        : base(error)
    {
    }
}