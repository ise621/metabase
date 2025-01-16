using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class CreateApplicationError
    : UserErrorBase<CreateApplicationErrorCode>
{
    public CreateApplicationError(
        CreateApplicationErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}