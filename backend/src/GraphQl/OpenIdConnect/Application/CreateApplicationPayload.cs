using Metabase.Data;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class CreateApplicationPayload
    : ApplicationPayload<CreateApplicationError>
{
    public CreateApplicationPayload(
        OpenIdApplication application
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