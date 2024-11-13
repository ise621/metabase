using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public abstract class InstitutionPayload<TInstitutionError>
    : Payload
    where TInstitutionError : IUserError
{
    protected InstitutionPayload(
        Institution institution
    )
    {
        Institution = institution;
    }

    protected InstitutionPayload(
        IReadOnlyCollection<TInstitutionError> errors
    )
    {
        Errors = errors;
    }

    protected InstitutionPayload(
        TInstitutionError error
    )
        : this([error])
    {
    }

    protected InstitutionPayload(
        Institution institution,
        IReadOnlyCollection<TInstitutionError> errors
    )
    {
        Institution = institution;
        Errors = errors;
    }

    protected InstitutionPayload(
        Institution institution,
        TInstitutionError error
    )
        : this(
            institution,
            [error]
        )
    {
    }

    public Institution? Institution { get; }
    public IReadOnlyCollection<TInstitutionError>? Errors { get; }
}