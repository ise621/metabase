using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public class ApplicationPayload<TApplicationError> : Payload
    where TApplicationError : IUserError
{
    protected ApplicationPayload(
        OpenIdApplication application
    )
    {
        Application = application;
    }

    protected ApplicationPayload(
        IReadOnlyCollection<TApplicationError> errors
    )
    {
        Errors = errors;
    }

    protected ApplicationPayload(
        TApplicationError error
    )
        : this(new[] { error })
    {
    }

    protected ApplicationPayload(
        OpenIdApplication application,
        IReadOnlyCollection<TApplicationError> errors
    )
    {
        Application = application;
        Errors = errors;
    }

    protected ApplicationPayload(
        OpenIdApplication application,
        TApplicationError error
    )
        : this(
            application,
            new[] { error }
        )
    {
    }

    public OpenIdApplication? Application { get; }
    public IReadOnlyCollection<TApplicationError>? Errors { get; }
}