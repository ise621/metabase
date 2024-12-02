using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class DeleteApplicationError
    : UserErrorBase<DeleteApplicationErrorCode>
{
    public DeleteApplicationError(
        DeleteApplicationErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}