using Metabase.Data;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class UpdateApplicationPayload
    : ApplicationPayload<UpdateApplicationError>
{
    public UpdateApplicationPayload(
        OpenIdApplication application
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