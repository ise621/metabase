using System.Collections.Generic;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public sealed class DeleteApplicationPayload
{
    public DeleteApplicationPayload()
    {
    }

    public DeleteApplicationPayload(
        DeleteApplicationError error
    )
    {
        Errors = new[] { error };
    }

    public IReadOnlyCollection<DeleteApplicationError>? Errors { get; }
}