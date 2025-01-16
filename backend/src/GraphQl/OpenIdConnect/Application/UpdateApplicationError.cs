using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class UpdateApplicationError
    : UserErrorBase<UpdateApplicationErrorCode>
{
    public UpdateApplicationError(
        UpdateApplicationErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}