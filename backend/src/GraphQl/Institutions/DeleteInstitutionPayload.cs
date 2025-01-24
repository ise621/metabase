using System.Collections.Generic;

namespace Metabase.GraphQl.Institutions;

public sealed class DeleteInstitutionPayload
{
    public DeleteInstitutionPayload()
    {
    }

    public DeleteInstitutionPayload(
        DeleteInstitutionError error
    )
    {
        Errors = [error];
    }

    public IReadOnlyCollection<DeleteInstitutionError>? Errors { get; }
}